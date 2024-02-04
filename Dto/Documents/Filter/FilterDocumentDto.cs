using AicaDocsApi.Dto.FilterCommons;

namespace AicaDocsApi.Dto.Documents.Filter;

public class FilterDocumentDto
{
    public required string? Code { get; set; }
    public required string? Title { get; set; }
    public required short? Edition { get; set; }
    public required short? Pages { get; set; }
    public required DateTimeOffset? DateOfValidity { get; set; }
    public required int? TypeId { get; set; }
    public required int? ProcessId { get; set; }
    public required int? ScopeId { get; set; }

    public PaginationParams PaginationParams { get; set; } = new PaginationParams();
    public SortByDocument SortBy { get; set; } = SortByDocument.Id;
    public SortOrder SortOrder { get; set; } = SortOrder.Asc;
    public DateComparator DateComparator { get; set; } = DateComparator.Equal;
}