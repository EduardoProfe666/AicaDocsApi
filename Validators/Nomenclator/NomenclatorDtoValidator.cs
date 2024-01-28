using System.ComponentModel.DataAnnotations;
using AicaDocsApi.Dto.Nomenclators;
using FluentValidation;

namespace AicaDocsApi.Validators.Nomenclator;

public class NomenclatorDtoValidator : AbstractValidator<NomenclatorDto>
{
    public NomenclatorDtoValidator()
    {
        RuleFor(p => p.Name)
            .MaximumLength(64)
            .NotEmpty()
            .WithMessage("Name must not be empty and with a maximum length of 64");
    }
}