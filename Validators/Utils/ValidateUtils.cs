using AicaDocsApi.Database;
using AicaDocsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Validators.Utils;

public static class ValidateUtils
{
    public static async Task<bool> ValidateNomenclatorId(int id, TypeOfNomenclator type, AicaDocsDb db, CancellationToken ct)
    {
        var nomenclator = await db.Nomenclators.FirstOrDefaultAsync(n => n.Id == id, cancellationToken: ct);
        return nomenclator is not null && nomenclator.Type == type;

    }
}