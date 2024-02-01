namespace AicaDocsApi.Models;

public class Download
{
    public int Id { get; set; }
    public DateTimeOffset DateOfDownload { get; set; } = DateTimeOffset.Now;
    public required Format Format { get; set; }
    public required string Username { get; set; }

    public required int DocumentId { get; set; }
    public required int ReasonId { get; set; }
    public Nomenclator? Reason { get; set; }
}