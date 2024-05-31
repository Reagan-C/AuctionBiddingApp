using FluentValidation;
using UserService.Protos;

namespace UserService.Validations
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Name field is required")
                .NotNull().WithMessage("Name cannot be null")
                .MaximumLength(30).WithMessage("Name field cannot exceed 30 characters");

            RuleFor(r => r.PhoneNumber)
                .NotNull().WithMessage("Phone number cannot be null.")
                .NotEmpty().WithMessage("Phone number cannot be empty.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
        }
    }
}
