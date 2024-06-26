using AccountsService.Dtos;
using FluentValidation;

namespace AccountsService.Validations
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage("Firstname field is required")
                .NotNull().WithMessage("Firstname cannot be null")
                .MaximumLength(30).WithMessage("Firstname field cannot exceed 30 characters");

            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage("Lastname field is required")
                .NotNull().WithMessage("Lastname cannot be null")
                .MaximumLength(30).WithMessage("Lastname field cannot exceed 30 characters");

            RuleFor(r => r.PhoneNumber)
                .NotNull().WithMessage("Phone number cannot be null.")
                .NotEmpty().WithMessage("Phone number cannot be empty.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");

            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password cannot be empty.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d").WithMessage("Password must contain at least one numeric character.")
                .Matches(@"[\W_]").WithMessage("Password must contain at least one special character.");
        }
    }
}
