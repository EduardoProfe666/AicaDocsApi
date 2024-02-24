using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace AicaDocsApi.Tests.Mock;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddScoped(_ => Mocker.BuildMockAicaDocsDb());
            services.AddScoped(_ => Mocker.BuildMockIBlobService());
        });
    }
}