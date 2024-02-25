using System.Net;
using System.Text;
using System.Text.Json;
using AicaDocsApi.Dto.Downloads;
using AicaDocsApi.Dto.Nomenclators;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;

namespace AicaDocsApi.Tests.Test.Downloads;

public class PostDownload : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostDownload(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
     [Fact]
    public async Task TestOk()
    {
        var client = _factory.CreateClient();

        var download = new DownloadCreatedDto()
        {
            Format = Format.Pdf,
            Username = "Lily",
            DocumentId = 1,
            ReasonId = 4
        };
        var content1 = new StringContent(JsonSerializer.Serialize(download), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"download", content1);

        Assert.True(result.StatusCode == HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.NotEmpty(content);
        
        var dl =
            JsonSerializer.Deserialize<ApiResponse<string>>(content, JsonSerializerOp.GetOptions())!
                .Data;
        Assert.NotNull(dl);
        Assert.NotEmpty(dl);
    }

    [Fact]
    public async Task TestNotFound()
    {
        var client = _factory.CreateClient();
        var download = new DownloadCreatedDto()
        {
            Format = Format.Word,
            Username = "Lily",
            DocumentId = 500,
            ReasonId = 4
        };
        var content1 = new StringContent(JsonSerializer.Serialize(download), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"download", content1);

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

    [Fact]
    public async Task TestBadRequest()
    {
        var client = _factory.CreateClient();
        var download = new DownloadCreatedDto()
        {
            Format = Format.Pdf,
            Username = "Lily",
            DocumentId = 0,
            ReasonId = 4
        };
        var content1 = new StringContent(JsonSerializer.Serialize(download), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"download", content1);

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