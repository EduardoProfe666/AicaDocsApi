using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Models;

namespace AicaDocsApi.Dto.Downloads.Filter;

public class FilterDownloadDto
{
    public required Format? Format { get; set; }
    public required DateTimeOffset? DateDownload { get; set; }
    public required string? Username { get; set; }

    public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    public SortByDownload SortBy { get; set; } = SortByDownload.Id;
    public SortOrder SortOrder { get; set; } = SortOrder.Asc;
    public DateComparator DateComparator { get; set; } = DateComparator.Equal;
}
