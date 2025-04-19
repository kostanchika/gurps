using FluentValidation;
using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Validators.Auth
{
    public class RegisterValidator : AbstractValidator<RegisterDto>
    {
        public RegisterValidator()
        {
            RuleFor(r => r.Login)
                .MinLengthWithMessage(ValidationRulesExtension.Login.MinLength, "Login")
                .MaxLengthWithMessage(ValidationRulesExtension.Login.MaxLength, "Login");

            RuleFor(r => r.Username)
                .MinLengthWithMessage(ValidationRulesExtension.Username.MinLength, "Username")
                .MaxLengthWithMessage(ValidationRulesExtension.Username.MaxLength, "Username");

            RuleFor(r => r.Password)
                .MinLengthWithMessage(ValidationRulesExtension.Password.MinLength, "Password")
                .MaxLengthWithMessage(ValidationRulesExtension.Password.MaxLength, "Password");

            RuleFor(r => r.Email)
                .EmailAddress()
                .WithMessage("Incorrect Email address format");
        }
    }
}
