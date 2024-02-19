namespace AicaDocsApi.Dto.FilterCommons;

public class FilterResponse<T>
{
    public required IEnumerable<T> Data { get; set; }
    public required int TotalPages { get; set; }
}