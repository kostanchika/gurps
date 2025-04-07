using FluentValidation;
using UsersService.Application.DTOs.Auth;

namespace UsersService.Application.Validators.Auth
{
    public class ResetPasswordValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordValidator()
        {
            RuleFor(r => r.NewPassword)
                .MinLengthWithMessage(ValidationRulesExtension.Password.MinLength, "NewPassword")
                .MaxLengthWithMessage(ValidationRulesExtension.Password.MaxLength, "NewPassword");
        }
    }
}
