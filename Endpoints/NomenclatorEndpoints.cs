namespace AicaDocsApi.Endpoints;

public static class NomenclatorEndpoints
{
    public static void MapNomenclatorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/nomenclator")
            .WithOpenApi()
            .WithTags(["Nomenclators"]);
        
    }
}