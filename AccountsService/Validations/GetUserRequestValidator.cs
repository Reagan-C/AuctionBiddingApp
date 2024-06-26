using AccountsService.Dtos;
using FluentValidation;

namespace AccountsService.Validations
{
    public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
    {
        public GetUserRequestValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email address is required");
        }
    }
}
