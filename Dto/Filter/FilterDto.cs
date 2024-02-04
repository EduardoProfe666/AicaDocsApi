namespace AicaDocsApi.Dto.Filter;

public class FilterDto
{
    public required string? Code { get; set; }
    public required string? Title { get; set; }
    public required short? Edition { get; set; }
    public required short? Pages { get; set; }
    public required DateTimeOffset? DateOfValidity { get; set; }
    public required DateComparator? DateComparator { get; set; }
    public required int? TypeId { get; set; }
    public required int? ProcessId { get; set; }
    public required int? ScopeId { get; set; }
    
    public required PaginationParams PaginationParams { get; set; }
    public required SortByDocument SortByDocument { get; set; } = SortByDocument.Id;
    public required SortOrder SortOrder { get; set; } = SortOrder.Asc;
}