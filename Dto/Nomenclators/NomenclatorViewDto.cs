using AicaDocsApi.Models;

namespace AicaDocsApi.Dto.Nomenclators;

public class NomenclatorViewDto(Nomenclator nomenclator)
{
    public int Id { get; set; } = nomenclator.Id;
    public string Name { get; set; } = nomenclator.Name;
    public TypeOfNomenclator Type { get; set; } = nomenclator.Type;
}