using FluentValidation;
using UserService.Protos;

namespace UserService.Validations
{
    public class GetUserRequestValidator :AbstractValidator<GetUserRequest>
    {
        public GetUserRequestValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email address is required");
        }
    }
}
