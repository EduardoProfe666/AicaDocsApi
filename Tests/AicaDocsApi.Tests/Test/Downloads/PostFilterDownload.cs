using System.Net;
using System.Text;
using System.Text.Json;
using AicaDocsApi.Dto.Downloads;
using AicaDocsApi.Dto.Downloads.Filter;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;
using Xunit.Abstractions;

namespace AicaDocsApi.Tests.Test.Downloads;

public class PostFilterDownload : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public PostFilterDownload(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }
    
      [Fact]
    public async Task TestOk()
    {
        var client = _factory.CreateClient();

        var download = new FilterDownloadDto()
        {
            Format = Format.Pdf,
            DateDownload = null,
            Username = null,
            DocumentId = 1,
            ReasonId = 4,
            PaginationParams = new PaginationParams
            {
                PageNumber = 1,
                PageSize = 10
            },
            SortBy = SortByDownload.Id,
            SortOrder = SortOrder.Asc,
            DateComparator = DateComparator.EqualLess
        };
        var content1 = new StringContent(JsonSerializer.Serialize(download), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"download/filter", content1);

        Assert.True(result.StatusCode == HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.NotEmpty(content);
        var downloadsList =
            JsonSerializer.Deserialize<ApiResponse<FilterResponse<Download>>>(content, JsonSerializerOp.GetOptions())!
                .Data;
        Assert.NotNull(downloadsList);
        Assert.NotEmpty(downloadsList.Data);
        
        Assert.True(downloadsList.Data.Count() == 2);
        Assert.NotNull(downloadsList.Data.FirstOrDefault(d => d.Id == 1));
        Assert.NotNull(downloadsList.Data.FirstOrDefault(d => d.Id == 4));
        
        Assert.True(downloadsList.TotalPages == 1);

        
        
    }

    
    [Fact]
    public async Task TestBadRequest()
    {
        var client = _factory.CreateClient();
        var download = new FilterDownloadDto()
        {
            Format = Format.Pdf,
            DateDownload = DateTimeOffset.Now,
            Username = "Lily",
            DocumentId = 0,
            ReasonId = 4,
            PaginationParams = new PaginationParams
            {
                PageNumber = -1,
                PageSize = 10
            },
            SortBy = SortByDownload.Id,
            SortOrder = SortOrder.Asc,
            DateComparator = DateComparator.EqualLess
        };
        var content1 = new StringContent(JsonSerializer.Serialize(download), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"download/filter", content1);

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