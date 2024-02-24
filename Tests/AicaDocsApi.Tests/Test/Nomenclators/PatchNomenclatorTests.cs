using System.Net;
using System.Text;
using System.Text.Json;
using AicaDocsApi.Database;
using AicaDocsApi.Dto.Nomenclators;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace AicaDocsApi.Tests.Test.Nomenclators;

public class PatchNomenclatorTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public PatchNomenclatorTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task TestOk()
    {
        var client = _factory.CreateClient();
        var id = 1;

        var nomenclatorCreatedDto = new NomenclatorPatchDto()
        {
            Name = "Lily"
        };
        var content1 = new StringContent(JsonSerializer.Serialize(nomenclatorCreatedDto), Encoding.UTF8,
            "application/json");
        var result = await client.PatchAsync($"nomenclator/{id}", content1);

        Assert.True(result.StatusCode == HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        Assert.Empty(content);
    }

    [Fact]
    public async Task TestNotFound()
    {
        var client = _factory.CreateClient();
        var id = 13;
        var nomenclatorCreatedDto = new NomenclatorPatchDto()
        {
            Name = "Lily"
        };
        var content1 = new StringContent(JsonSerializer.Serialize(nomenclatorCreatedDto), Encoding.UTF8,
            "application/json");
        var result = await client.PatchAsync($"nomenclator/{id}", content1);

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
        var id = 2;
        var nomenclatorCreatedDto = new NomenclatorPatchDto()
        {
            Name = "Lilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylilylily"
        };
        var content1 = new StringContent(JsonSerializer.Serialize(nomenclatorCreatedDto), Encoding.UTF8,
            "application/json");
        var result = await client.PatchAsync($"nomenclator/{id}", content1);

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