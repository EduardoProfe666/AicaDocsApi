using AicaDocsApi.Models;

namespace AicaDocsApi.Dto.Documents;

public class DocumentViewDto(Document doc)
{
    public int Id { get; set; } = doc.Id;
    public string Code { get; set; } = doc.Code;
    public string Title { get; set; } = doc.Title;
    public short Edition { get; set; } = doc.Edition;
    public short Pages { get; set; } = doc.Pages;
    public DateOnly DateOfValidity { get; set; } = doc.DateOfValidity;

    public int TypeId { get; set; } = doc.TypeId;
    public int ProcessId{ get; set; } = doc.ProcessId;
    public int ScopeId { get; set; } = doc.ScopeId;
}