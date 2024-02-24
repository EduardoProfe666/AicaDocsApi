using AicaDocsApi.Tests.Mock;

namespace AicaDocsApi.Tests.Test.Documents;

public class PostFilterDocumentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostFilterDocumentTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
}