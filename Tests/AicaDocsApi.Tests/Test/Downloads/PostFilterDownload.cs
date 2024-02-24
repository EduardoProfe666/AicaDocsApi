using AicaDocsApi.Tests.Mock;

namespace AicaDocsApi.Tests.Test.Downloads;

public class PostFilterDownload : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostFilterDownload(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
}