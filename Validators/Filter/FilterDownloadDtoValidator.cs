using AicaDocsApi.Dto.Documents.Filter;
using FluentValidation;

namespace AicaDocsApi.Validators.Filter;

public class FilterDownloadDtoValidator : AbstractValidator<FilterDocumentDto>
{
    public FilterDownloadDtoValidator()
    {
        
    }
}