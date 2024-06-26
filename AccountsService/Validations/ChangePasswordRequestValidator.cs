using AccountsService.Dtos;
using FluentValidation;

namespace AccountsService.Validations
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(p => p.OldPassword)
                .NotEmpty().WithMessage("Please input your old password");

            RuleFor(p => p.NewPassword)
                .NotEmpty().WithMessage("Password cannot be empty.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one numeric character.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");

            RuleFor(cp => cp.ConfirmNewPassword)
                .Equal(cp => cp.NewPassword).WithMessage("Passwords do not match");
        }
    }
}
