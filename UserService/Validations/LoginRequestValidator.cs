using FluentValidation;
using UserService.Protos;

namespace UserService.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(r => r.Email)
               .NotEmpty().WithMessage("Email address is required");


            RuleFor(r => r.Password)
                .NotEmpty().WithMessage("Password cannot be empty.");
        }
    }
}
