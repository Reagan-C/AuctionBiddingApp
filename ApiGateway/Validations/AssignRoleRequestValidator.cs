using FluentValidation;
using UserService.Protos;

namespace UserService.Validations
{
    public class AssignRoleRequestValidator : AbstractValidator<AssignRoleRequest>
    {
        public AssignRoleRequestValidator()
        {
            RuleFor(r => r.Email)
               .NotEmpty().WithMessage("Email address is required")
               .EmailAddress().WithMessage("Invalid email format");
        }
    }
}
