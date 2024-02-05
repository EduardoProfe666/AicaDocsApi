using AicaDocsApi.Database;
using AicaDocsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Validators.Utils;

public class ValidateUtils
{
    private readonly AicaDocsDb _db;

    public ValidateUtils(AicaDocsDb db)
    {
        _db = db;
    }

    public async Task<bool> ValidateNomenclatorId(int? id, TypeOfNomenclator type, CancellationToken ct)
    {
        var nomenclator = await _db.Nomenclators.FirstOrDefaultAsync(n => n.Id == id, cancellationToken: ct);
        return nomenclator is not null && nomenclator.Type == type;

    }
}