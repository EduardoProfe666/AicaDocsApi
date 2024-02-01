namespace AicaDocsApi.Endpoints;

public static class DownloadEndpoints
{
    public static void MapDownloadEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/download")
            .WithOpenApi()
            .WithTags(["Downloads"]);
    }
}