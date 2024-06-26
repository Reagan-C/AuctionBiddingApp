using AccountsService.Dtos;
using FluentValidation;

namespace AccountsService.Validations
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
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
        }
    }
}
