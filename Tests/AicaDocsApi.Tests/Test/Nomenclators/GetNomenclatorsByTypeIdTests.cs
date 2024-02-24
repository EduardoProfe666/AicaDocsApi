using System.Net;
using System.Text.Json;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;

namespace AicaDocsApi.Tests.Test.Nomenclators;

public class GetNomenclatorsByTypeIdTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GetNomenclatorsByTypeIdTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task TestOk()
    {
        var client = _factory.CreateClient();
        var type = (int)TypeOfNomenclator.ScopeOfDocument;
        var id = 7;
        var result = await client.GetAsync($"nomenclator/{type}/{id}");

        Assert.NotNull(result);
        Assert.True(result.StatusCode == HttpStatusCode.OK);

        var content = await result.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);

        var nomenclator =
            JsonSerializer.Deserialize<ApiResponse<Nomenclator>>(content, JsonSerializerOp.GetOptions())!
                .Data;
        Assert.NotNull(nomenclator);

       Assert.True(nomenclator.Id == id && (int)nomenclator.Type == type);
    }

    [Fact]
    public async Task TestBadRequest()
    {
        var client = _factory.CreateClient();
        var type = -1;
        var id = 13;
        var result = await client.GetAsync($"nomenclator/{type}/{id}");
        
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
    
    [Fact]
    public async Task TestNotFound()
    {
        var client = _factory.CreateClient();
        var type = (int)TypeOfNomenclator.ProcessOfDocument;
        var id = 13;
        var result = await client.GetAsync($"nomenclator/{type}/{id}");
        
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
    
