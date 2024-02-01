using AicaDocsApi.Dto.Documents;
using FluentValidation;

namespace AicaDocsApi.Validators.Document;

public class DocumentCreatedDtoValidator : AbstractValidator<DocumentCreatedDto>
{
    public DocumentCreatedDtoValidator()
    {
        
    }
}