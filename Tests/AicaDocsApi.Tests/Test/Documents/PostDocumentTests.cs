using AicaDocsApi.Tests.Mock;

namespace AicaDocsApi.Tests.Test.Documents;

public class PostDocumentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostDocumentTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
}