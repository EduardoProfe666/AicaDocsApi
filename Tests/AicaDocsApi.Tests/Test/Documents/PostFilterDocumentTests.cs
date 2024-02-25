using System.Net;
using System.Text;
using System.Text.Json;
using AicaDocsApi.Dto.Documents.Filter;
using AicaDocsApi.Dto.Downloads.Filter;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;

namespace AicaDocsApi.Tests.Test.Documents;

public class PostFilterDocumentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostFilterDocumentTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
      [Fact]
    public async Task TestOk()
    {
        var client = _factory.CreateClient();

        var document = new FilterDocumentDto()
        {
            Code = null,
            Title = null,
            Edition = null,
            Pages = null,
            DateOfValidity = null,
            TypeId = 11,
            ProcessId = 1,
            ScopeId = 8,
            PaginationParams = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            },
            SortBy = SortByDocument.Id,
            SortOrder = SortOrder.Asc,
            DateComparator = DateComparator.EqualLess
        };
        var content1 = new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"document/filter", content1);

        Assert.True(result.StatusCode == HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.NotEmpty(content);
        var docList =
            JsonSerializer.Deserialize<ApiResponse<FilterResponse<Document>>>(content, JsonSerializerOp.GetOptions())!
                .Data;
        Assert.NotNull(docList);
        Assert.NotEmpty(docList.Data);
        
        Assert.True(docList.Data.Count() == 1);
        Assert.NotNull(docList.Data.FirstOrDefault(d => d.Id == 7));
        
        Assert.True(docList.TotalPages == 1);
        
    }

    
    [Fact]
    public async Task TestBadRequest()
    {
        var client = _factory.CreateClient();
        var document = new FilterDocumentDto()
        {
            Code = null,
            Title = null,
            Edition = 0,
            Pages = null,
            DateOfValidity = null,
            TypeId = -5,
            ProcessId = 11,
            ScopeId = 3,
            PaginationParams = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            },
            SortBy = SortByDocument.Id,
            SortOrder = SortOrder.Asc,
            DateComparator = DateComparator.EqualLess
        };
        var content1 = new StringContent(JsonSerializer.Serialize(document), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"document/filter", content1);
        
        Assert.NotNull(result);
        Assert.True(result.StatusCode == HttpStatusCode.BadRequest);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        var problem = JsonSerializer.Deserialize<ApiResponse>(content, JsonSerializerOp.GetOptions())!.ProblemDetails;
        Assert.NotNull(problem);

        Assert.NotNull(problem.Status);
        Assert.Equal(400, problem.Status);

        Assert.NotNull(problem.Errors);
        Assert.NotEmpty(problem.Errors);
    }
    
}