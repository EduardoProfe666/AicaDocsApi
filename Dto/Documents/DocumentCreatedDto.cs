using AicaDocsApi.Models;
using Spire.Pdf;

namespace AicaDocsApi.Dto.Documents;

public class DocumentCreatedDto
{ 
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required int Edition { get; set; }
    public required DateTimeOffset DateOfValidity { get; set; }
    
    public required int TypeId { get; set; }
    public required int ProcessId { get; set; }
    public required int ScopeId { get; set; }

    public required IFormFile Pdf { get; set; }

    public Document ToNewDocument()
    {
        using var pdfStream = Pdf.OpenReadStream();
        var pdf = new PdfDocument();
        pdf.LoadFromStream(pdfStream);
        var pages = pdf.Pages.Count;

        return new Document
        {
            Code = Code, Edition = (short)Edition, Pages = (short)pages, Title = Title,
            DateOfValidity = DateOfValidity.UtcDateTime,
            TypeId = TypeId, ProcessId = ProcessId, ScopeId = ScopeId
        };
    }
}