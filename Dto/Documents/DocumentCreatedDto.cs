using SautinSoft;

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

    public required IFormFile Word { get; set; }


    public Models.Document ToNewDocument()
    {
        using var pdfStream = Pdf.OpenReadStream();
        var pdfFocus = new PdfFocus();
        pdfFocus.OpenPdf(pdfStream);
        var pages = pdfFocus.PageCount;
        pdfFocus.ClosePdf();

        return new Models.Document
        {
            Code = Code.Trim(), Edition = (short)Edition, Pages = (short)pages, Title = Title.Trim(),
            DateOfValidity = DateOfValidity.UtcDateTime,
            TypeId = TypeId, ProcessId = ProcessId, ScopeId = ScopeId
        };
    }
}