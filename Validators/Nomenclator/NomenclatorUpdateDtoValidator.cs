using AicaDocsApi.Dto.Nomenclators;
using FluentValidation;

namespace AicaDocsApi.Validators.Nomenclator;

public class NomenclatorUpdateDtoValidator : AbstractValidator<NomenclatorUpdateDto>
{
    public NomenclatorUpdateDtoValidator()
    {
        RuleFor(e => e.Name)
            .MaximumLength(64)
            .WithMessage("Name must have a maximum length of 64 caracters")
            .NotEmpty()
            .WithMessage("Name must not be empty");
    }
}