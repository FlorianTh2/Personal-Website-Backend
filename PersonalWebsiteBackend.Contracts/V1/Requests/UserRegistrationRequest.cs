﻿using System.ComponentModel.DataAnnotations;

namespace PersonalWebsiteBackend.Contracts.V1.Requests
{
    public class UserRegistrationRequest
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}