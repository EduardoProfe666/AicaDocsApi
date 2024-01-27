
using AicaDocsApi.Models;

namespace AicaDocsApi.Dto;

public class DocumentUpdateDto: IDocumentDto
{
    public required string Code { get; set; }
    public required string Title { get; set; }
    public required short Edition { get; set; }
    public required short Pages { get; set; }
    public required DateTimeOffset DateOfValidity { get; set; }
    public Document ToNewDocument()
    {
        return new Document
        {
            Code = Code, Edition = Edition, Pages = Pages, Title = Title,
            DateOfValidity = DateOnly.FromDateTime(DateOfValidity.DateTime)
        };
    }
}