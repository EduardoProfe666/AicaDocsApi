using System.Net;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AicaDocsApi.Tests.Test.Nomenclators;

public class GetNomenclatorsByTypeTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GetNomenclatorsByTypeTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TestOk()
    {
        var client = _factory.CreateClient();
        var type = (int)TypeOfNomenclator.ProcessOfDocument;
        var result = await client.GetAsync($"/nomenclator/{type}");

        Assert.NotNull(result);
        Assert.True(result.StatusCode == HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        var nomenclators =
            JsonSerializer.Deserialize<ApiResponse<IEnumerable<Nomenclator>>>(content, JsonSerializerOp.GetOptions())!
                .Data;
        Assert.NotNull(nomenclators);
        Assert.NotEmpty(nomenclators);

        Assert.NotNull(nomenclators.FirstOrDefault(c => c.Id == 1));
        Assert.NotNull(nomenclators.FirstOrDefault(c => c.Id == 2));
        Assert.NotNull(nomenclators.FirstOrDefault(c => c.Id == 3));
    }

    [Fact]
    public async Task TestBadRequest()
    {
        var client = _factory.CreateClient();
        var type = -1;
        var result = await client.GetAsync($"/nomenclator/{type}");
        
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