namespace AicaDocsApi.Models;

public class Document
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Title { get; set; }
    public required short Edition { get; set; }
    public required short Pages { get; set; }
    public required DateOnly DateOfValidity { get; set; }

    public required int TypeId { get; set; }
    public Nomenclator? Type { get; set; }

    public required int ProcessId { get; set; }
    public Nomenclator? Process{ get; set; }
    
    public required int ScopeId { get; set; }
    public Nomenclator? Scope { get; set; }
}