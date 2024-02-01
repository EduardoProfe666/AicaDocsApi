using AicaDocsApi.Dto.Downloads;
using FluentValidation;

namespace AicaDocsApi.Validators;

public class DocumentCreatedDtoValidator : AbstractValidator<DownloadCreatedDto>
{
    public DocumentCreatedDtoValidator()
    {
        
    }
}