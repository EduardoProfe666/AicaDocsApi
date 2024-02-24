using AicaDocsApi.Tests.Mock;

namespace AicaDocsApi.Tests.Test.Downloads;

public class PostDownload : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostDownload(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
}