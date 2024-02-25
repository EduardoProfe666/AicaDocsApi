using System.Net;
using System.Text;
using System.Text.Json;
using AicaDocsApi.Dto.Nomenclators;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;

namespace AicaDocsApi.Tests.Test.Nomenclators;

public class PostNomenclatorTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostNomenclatorTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task TestCreated()
    {
        var client = _factory.CreateClient();

        var nomenclatorCreatedDto = new NomenclatorCreatedDto()
        {
            Name = "Lily",
            Type = TypeOfNomenclator.ProcessOfDocument
            
        };
        var content1 = new StringContent(JsonSerializer.Serialize(nomenclatorCreatedDto), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"nomenclator", content1);

        Assert.True(result.StatusCode == HttpStatusCode.Created);

        var content = await result.Content.ReadAsStringAsync();
        Assert.Empty(content);
    }
    

    [Fact]
    public async Task TestBadRequest()
    {
        var client = _factory.CreateClient();
        var nomenclatorCreatedDto = new NomenclatorCreatedDto()
        {
            Name = "Lilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylily",
            Type = TypeOfNomenclator.ScopeOfDocument
        };
        var content1 = new StringContent(JsonSerializer.Serialize(nomenclatorCreatedDto), Encoding.UTF8,
            "application/json");
        var result = await client.PostAsync($"nomenclator", content1);

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