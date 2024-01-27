namespace AicaDocsApi.Endpoints;

public static class GeneralEndpoints
{
    public static void MapGeneralEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("")
            .WithTags(["General"])
            .WithOpenApi();

        app.MapGet("/", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();
        app.MapGet("/docs", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();
        
    }
}