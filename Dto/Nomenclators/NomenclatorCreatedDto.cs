using AicaDocsApi.Models;

namespace AicaDocsApi.Dto.Nomenclators;

public class NomenclatorCreatedDto
{
    public required string Name { get; set; }
    public required TypeOfNomenclator Type { get; set; }
}