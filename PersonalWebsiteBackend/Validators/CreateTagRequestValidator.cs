using PersonalWebsiteBackend.Contracts.V1.Requests;
using FluentValidation;

namespace PersonalWebsiteBackend.Validators
{
    public class CreateTagRequestValidator : AbstractValidator<CreateTagRequest>
    {
        public CreateTagRequestValidator()
        {
            RuleFor(a => a.TagName)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}