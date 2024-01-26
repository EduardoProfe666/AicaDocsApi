using AicaDocsApi.Models;

namespace AicaDocsApi.Dto;

public interface IDocumentDto
{
    public Document ToNewDocument();
}