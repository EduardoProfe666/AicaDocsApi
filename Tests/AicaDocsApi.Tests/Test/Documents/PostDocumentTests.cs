using System.Net;
using System.Text;
using System.Text.Json;
using AicaDocsApi.Dto.Documents.Filter;
using AicaDocsApi.Dto.FilterCommons;
using AicaDocsApi.Models;
using AicaDocsApi.Responses;
using AicaDocsApi.Tests.Mock;
using AicaDocsApi.Tests.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Xunit.Abstractions;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace AicaDocsApi.Tests.Test.Documents;

public class PostDocumentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public PostDocumentTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
    }
    
     [Fact]
    public async Task TestCreated()
    {
        var client = _factory.CreateClient();
        var env = _factory.Services.GetRequiredService<IWebHostEnvironment>();
        
        using var document = new MultipartFormDataContent();
        document.Add(new StringContent("Lily"), "title");
        document.Add(new StringContent("Lilita"), "code");
        document.Add(new StringContent("1"), "edition");
        document.Add(new StringContent(DateTimeOffset.Now.ToString("O")), "dateOfValidity");
        document.Add(new StringContent("10"), "typeId");
        document.Add(new StringContent("1"), "processId");
        document.Add(new StringContent("7"), "scopeId");

        var basePath = env.ContentRootPath;
        basePath = Path.Combine(basePath, "Tests","AicaDocsApi.Tests");
        var pathPdf = Path.Combine(basePath, "Files", "test.pdf");
        var pathWord = Path.Combine(basePath, "Files", "test.docx");
        
        var pdfContent = new ByteArrayContent(await File.ReadAllBytesAsync(pathPdf));
        pdfContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        var docxContent = new ByteArrayContent(await File.ReadAllBytesAsync(pathWord));
        docxContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        
        document.Add(pdfContent, "pdf", "test.pdf");
        document.Add(docxContent, "word", "test.docx");
        
        
        var result = await client.PostAsync($"document", document);

        Assert.True(result.StatusCode == HttpStatusCode.Created);
        
        var content = await result.Content.ReadAsStringAsync();
        Assert.Empty(content);

    }

    
    [Fact]
    public async Task TestBadRequest()
    {
        var client = _factory.CreateClient();
        var env = _factory.Services.GetRequiredService<IWebHostEnvironment>();
        
        using var document = new MultipartFormDataContent();
        document.Add(new StringContent("Lily"), "title");
        document.Add(new StringContent("Lilita"), "code");
        document.Add(new StringContent("0"), "edition");
        document.Add(new StringContent(DateTimeOffset.Now.ToString("O")), "dateOfValidity");
        document.Add(new StringContent("10"), "typeId");
        document.Add(new StringContent("1"), "processId");
        document.Add(new StringContent("7"), "scopeId");

        var basePath = env.ContentRootPath;
        basePath = Path.Combine(basePath, "Tests","AicaDocsApi.Tests");
        var pathPdf = Path.Combine(basePath, "Files", "test.pdf");
        var pathWord = Path.Combine(basePath, "Files", "test.docx");
        
        var pdfContent = new ByteArrayContent(await File.ReadAllBytesAsync(pathPdf));
        pdfContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        var docxContent = new ByteArrayContent(await File.ReadAllBytesAsync(pathWord));
        docxContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        
        document.Add(pdfContent, "pdf", "test.pdf");
        document.Add(docxContent, "word", "test.pdf");
        
        
        var result = await client.PostAsync($"document", document);
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