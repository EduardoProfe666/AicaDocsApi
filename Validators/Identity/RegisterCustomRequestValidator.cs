using AicaDocsApi.Dto.Identity;
using FluentValidation;

namespace AicaDocsApi.Validators.Identity;

public class RegisterCustomRequestValidator: AbstractValidator<RegisterCustomRequest>
{
    public RegisterCustomRequestValidator()
    {
        RuleFor(e => e.FullName)
            .NotEmpty()
            .WithMessage("Fullname must be not empty")
            .MaximumLength(64)
            .WithMessage("Fullname must have a maximum length of 64 characters");

        RuleFor(e => e.Role)
            .IsInEnum()
            .WithMessage("Role must be valid")
            .NotNull()
            .WithMessage("Role must not be null");
    }
}