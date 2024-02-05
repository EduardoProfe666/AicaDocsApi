using AicaDocsApi.Dto.Nomenclators;
using FluentValidation;

namespace AicaDocsApi.Validators.Nomenclators;

public class NomenclatorUpdateDtoValidator : AbstractValidator<NomenclatorUpdateDto>
{
    public NomenclatorUpdateDtoValidator()
    {
        RuleFor(e => e.Name)
            .MaximumLength(64)
            .WithMessage("Name must have a maximum length of 64 characters")
            .NotEmpty()
            .WithMessage("Name must not be empty");
    }
}