using FluentValidation;
using UserService.Protos;

namespace UserService.Validations
{
    public class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
    {
        public DeleteUserRequestValidator()
        {
            RuleFor(r => r.Username)
                .NotEmpty().WithMessage("Username field is required");

        }
    }
}
