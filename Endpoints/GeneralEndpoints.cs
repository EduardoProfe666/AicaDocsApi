namespace AicaDocsApi.Endpoints;

public static class GeneralEndpoints
{
    public static void MapGeneralEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("")
            .WithTags(["General"])
            .WithOpenApi();
        
        // ---------------- Endpoint Declarations --------------------//
        
        group.MapGet("/", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();
        group.MapGet("/docs", () => Results.Redirect("/swagger/index.html")).ExcludeFromDescription();
        
    }
}