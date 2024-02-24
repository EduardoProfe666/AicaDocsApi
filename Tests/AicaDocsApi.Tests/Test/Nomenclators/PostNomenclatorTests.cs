using System.Net;
using AicaDocsApi.Models;
using AicaDocsApi.Tests.Mock;

namespace AicaDocsApi.Tests.Test.Nomenclators;

public class PostNomenclatorTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PostNomenclatorTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }
    
}