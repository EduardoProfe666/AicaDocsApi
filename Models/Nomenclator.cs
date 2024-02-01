namespace AicaDocsApi.Models;

public class Nomenclator
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required TypeOfNomenclator Type { get; set; }
}