namespace AicaDocsApi.Dto.Documents;

public class DocumentCreatedDto
{ 
    public required string Title { get; set; }
    public required string Code { get; set; }
    public required short Edition { get; set; }
    public required short Pages { get; set; }
    public required DateTimeOffset DateOfValidity { get; set; }
    
    public required int TypeId { get; set; }
    public required int ProcessId { get; set; }
    public required int ScopeId { get; set; }

    public Models.Document ToNewDocument()
    {
        return new Models.Document
        {
            Code = Code, Edition = Edition, Pages = Pages, Title = Title,
            DateOfValidity = DateOnly.FromDateTime(DateOfValidity.DateTime),
            TypeId = TypeId, ProcessId = ProcessId, ScopeId = ScopeId
        };
    }
}