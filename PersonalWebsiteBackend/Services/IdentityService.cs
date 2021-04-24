using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalWebsiteBackend.Data;
using PersonalWebsiteBackend.Domain;
using PersonalWebsiteBackend.Options;

namespace PersonalWebsiteBackend.Services
{
    // information sources:
    //    - https://www.youtube.com/watch?v=ARvsBUBioT0&list=PLUOequmGnXxOgmSDWU7Tl6iQTsOtyjtwU&index=11&ab_channel=NickChapsas
    //    - https://stackoverflow.com/a/47568572/11244995
    //    - https://jasonwatmore.com/post/2020/05/25/aspnet-core-3-api-jwt-authentication-with-refresh-tokens
    //    - https://github.com/cornflourblue/aspnet-core-3-registration-login-api/blob/master/Controllers/UsersController.cs
    //        - there is a website for this repo
    public class IdentityService : IIdentityService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly DataContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters, DataContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _roleManager = roleManager;
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
                return new AuthenticationResult
                {
                    Errors = new[] {"User with this email address already exists"}
                };

            var newUser = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await _userManager.CreateAsync(newUser, password);

            if (!createdUser.Succeeded)
                return new AuthenticationResult
                {
                    Errors = createdUser.Errors.Select(a => a.Description)
                };

            // intentionally didnt use createdUser since 1. it has another type 2. all fields (like id) are added
            // to the newUser via the _userManager.CreateAsync(/* */) method
            return await GenerateAuthenticationResultForUserAsync(newUser);
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new AuthenticationResult
                {
                    Errors = new[] {"User does not exist"}
                };

            var userHadValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userHadValidPassword)
                return new AuthenticationResult
                {
                    Errors = new[] {"User/password combination is wrong"}
                };

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        // does not work?
        //    - pay attention to the Clock skew (= "Zeitversatz") parameter in mvc-installer
        //    - Intention: the value of clockskew is added to the expiry-date of the token to include
        //        e.g. the "travel-time" between to parties
        //    - clockskew-parameter is set to 0, at least for testing purposes, to get the expected
        //        token-invalidation according to the proper expiry-date
        //
        // you can always check the token on jwt.io if something is wrong with the token
        //
        // a jwt != a refreshtoken
        // a refreshtoken != a jwt
        //
        // core here:
        //    1. if jwt is expired send expired jwt+valid refresh
        //    2. it generated a complete new jwt + a complete new refreshtoken and sends both back
        //        (it does NOT just take given jwt and sets new expiry date)
        //    3. if an old jwt + the old refreshtoken is send again to refresh it does not work
        //        (1 refreshtoken can only be used once)
        //
        // here: we dont refresh a jwt if this jwt is not expired but this can be done
        //
        // if the refreshtoken should be invalidated if
        //    - the user logouts
        //    - user changes email
        //    - maybe when user changes pw?
        //    - user has lost his phone (-> we can invalidate all tokens of this user)
        //
        // all the if-statements should be gathered and summed up with one single error msg: token is invalid
        //    -> reason: dont tell the user the actual reason
        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            // claimsPrincipal can also be called "validated Token" or vice versa
            var claimsPrincipal = GetPrincipalFromToken(token);

            if (claimsPrincipal == null)
            {
                return new AuthenticationResult {Errors = new[] {"Invalid Token"}};
            }

            var expiryDateUnix =
                long.Parse(claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult {Errors = new[] {"This token hasn't expired yet"}};
            }

            var jti = claimsPrincipal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            // returns the stored refreshtoken for this jwt from the database
            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token does not exist"}};
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token has expired"}};
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token has been invalidated"}};
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token has been used"}};
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult {Errors = new[] {"This refresh token does not match this JWT"}};
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            // the id parameter == userid
            var user = await _userManager.FindByIdAsync(claimsPrincipal.Claims.Single(x => x.Type == "id").Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // set ValidateLifetime = false in order to successfully validate an expired token and get the Principal
                // That makes sense, since an expired token should not validate
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                // we have to check for proper secruity-algorithm since
                // e.g. in a jwt, you can specify secruityAlgorithm: none
                if (!IsJwtWithValidSecrurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        // helper method to check for proper secruity-algorithm
        // reason: (first way to describe the reason)
        //    - in a jwt-token we can specify the SecruityAlgorithm
        //    - it is possible to set SecruityAlgorithm to: none
        //    - so we have to check for proper SecruityAlrotihm
        // reason: (second way to describe the reason)
        //    - in mvc-installer we defined token but didnt specify a specific secruity-algorithm
        private bool IsJwtWithValidSecrurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        // create jwt and refresh-token for the client
        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

            // specify the content of the jwt
            // add "normal" claims to the token
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // id means userId (id == userid)
                new Claim("id", user.Id)
            };
            
            // add userClaims to the token
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // add roleClaims to the token
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                // add roleClaim to the token
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                // Problem: if you look ad db-schema of identity: roles can have roleClaims (roleClaim != claim)
                // -> we want to add these roleClaims to the token too
                var role = await _roleManager.FindByNameAsync(userRole);
                if(role == null) continue;
                var roleClaims = await _roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if(claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }
            
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            // create refresh token
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMonths(1)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }
    }
}