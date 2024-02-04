using AicaDocsApi.Dto.Nomenclators;
using FluentValidation;

namespace AicaDocsApi.Validators.Nomenclator;

public class FilterNomenclatorDtoValidator: AbstractValidator<FilterNomenclatorDto>
{
    public FilterNomenclatorDtoValidator()
    {
        RuleFor(a => a.Type)
            .IsInEnum()
            .WithMessage("Type of nomenclator must be valid");
    }
}