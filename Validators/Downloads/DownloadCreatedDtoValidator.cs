using AicaDocsApi.Dto.Downloads;
using FluentValidation;

namespace AicaDocsApi.Validators.Downloads;

public class DownloadCreatedDtoValidator : AbstractValidator<DownloadCreatedDto>
{
    public DownloadCreatedDtoValidator()
    {
        RuleFor(e => e.Format)
            .IsInEnum()
            .WithMessage("Format must be valid")
            .NotNull()
            .WithMessage("Format cannot be null");
        
        RuleFor(e => e.DocumentId)
            .GreaterThan(0)
            .WithMessage("Document id must be greater than 0")
            .NotNull()
            .WithMessage("Document id cannot be null");
        
        RuleFor(e => e.ReasonId)
            .GreaterThan(0)
            .WithMessage("Reason id must be greater than 0")
            .NotNull()
            .WithMessage("Reason id cannot be null");

            


    }
}