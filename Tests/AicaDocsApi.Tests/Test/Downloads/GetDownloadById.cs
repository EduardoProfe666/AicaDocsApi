using System.Net;
using System.Text.Json;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;

namespace AicaDocsApi.Tests.Test.Downloads;

public class GetDownloadById : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GetDownloadById(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task TestOk()
    {
        var client = _factory.CreateClient();
        var id = 1;
        var result = await client.GetAsync($"download/{id}");

        Assert.NotNull(result);
        Assert.True(result.StatusCode == HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        var download =
            JsonSerializer.Deserialize<ApiResponse<Download>>(content, JsonSerializerOp.GetOptions())!
                .Data;
        Assert.NotNull(download);

        Assert.True(download.Id == id);
        
    }

    [Fact]
    public async Task TestNotFound()
    {
        var client = _factory.CreateClient();
        var id = 13;
        var result = await client.GetAsync($"download/{id}");
        
        Assert.NotNull(result);
        Assert.True(result.StatusCode == HttpStatusCode.NotFound);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        var problem = JsonSerializer.Deserialize<ApiResponse>(content, JsonSerializerOp.GetOptions())!.ProblemDetails;
        Assert.NotNull(problem);
        
        Assert.NotNull(problem.Status);
        Assert.Equal(404, problem.Status);
        
        Assert.NotNull(problem.Errors);
        Assert.NotEmpty(problem.Errors);
    }
}
    
