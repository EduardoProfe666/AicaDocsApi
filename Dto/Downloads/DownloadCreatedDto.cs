using AicaDocsApi.Models;

namespace AicaDocsApi.Dto.Downloads;

public class DownloadCreatedDto
{
    public required Format Format { get; set; }
    public required string Username { get; set; }
    public required int DocumentId { get; set; }
    public required int ReasonId { get; set; }
}