using AicaDocsApi.Models;

namespace AicaDocsApi.Dto.Downloads;

public class DownloadViewDto(Download download)
{
    public required int Id { get; set; } = download.Id;
    public required DateTimeOffset DateOfDownload { get; set; } = download.DateOfDownload;
    public required Format Format { get; set; } = download.Format;
    public required string Username { get; set; } = download.Username;

    public required int DocumentId { get; set; } = download.DocumentId;
    public required int ReasonId { get; set; } = download.ReasonId;
}