using AicaDocsApi.Dto.Documents;
using FluentValidation;

namespace AicaDocsApi.Validators.Documents;

public class DocumentCreatedDtoValidator : AbstractValidator<DocumentCreatedDto>
{
    public DocumentCreatedDtoValidator()
    {
        RuleFor(e => e.Title)
            .MaximumLength(64)
            .WithMessage("Title must have a maximum length of 64 characters")
            .NotEmpty()
            .WithMessage("Title must not be empty");

        RuleFor(e => e.Code)
            .MaximumLength(64)
            .WithMessage("Code must have a maximum length of 64 characters")
            .NotEmpty()
            .WithMessage("Code must not be empty");

        RuleFor(e => e.Edition)
            .ExclusiveBetween(0, 32001)
            .WithMessage("Edition must be greater than 0 and less than 32001")
            .NotNull()
            .WithMessage("Edition cannot be null");

        RuleFor(e => e.Pages)
            .ExclusiveBetween(0, 32001)
            .WithMessage("Pages must be greater than 0 and less than 32001")
            .NotNull()
            .WithMessage("Pages cannot be null");

        RuleFor(e => e.DateOfValidity)
            .NotNull()
            .WithMessage("Date of validity cannot be null");

        RuleFor(e => e.TypeId)
            .GreaterThan(0)
            .WithMessage("Type id must be greater than 0")
            .NotNull()
            .WithMessage("Type id cannot be null");

        RuleFor(e => e.ProcessId)
            .GreaterThan(0)
            .WithMessage("Process id must be greater than 0")
            .NotNull()
            .WithMessage("Process id cannot be null");

        RuleFor(e => e.ScopeId)
            .GreaterThan(0)
            .WithMessage("Scope id must be greater than 0")
            .NotNull()
            .WithMessage("Scope id cannot be null");

        RuleFor(e => e.Pdf)
            .NotEmpty()
            .WithMessage("Pdf file cannot be null")
            .Must(e => e.ContentType == "application/pdf")
            .WithMessage("Only pdf files allowed (*.pdf)");

        RuleFor(e => e.Pdf.Length)
            .LessThanOrEqualTo(20 * 1024 * 1024)
            .WithMessage("File length must not exceed 20 MB");

        RuleFor(e => e.Word)
            .NotEmpty()
            .WithMessage("Word file cannot be null")
            .Must(e => e.ContentType is "application/vnd.openxmlformats-officedocument.wordprocessingml.document" or "application/msword")
            .WithMessage("Only word files allowed (*.docx)");

        RuleFor(e => e.Word.Length)
            .LessThanOrEqualTo(20 * 1024 * 1024)
            .WithMessage("File length must not exceed 20 MB");
    }
}