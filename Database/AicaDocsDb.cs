using AicaDocsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Database;

public class AicaDocsDb : DbContext
{
    public AicaDocsDb(DbContextOptions<AicaDocsDb> options)
        : base(options) { }

    public DbSet<Document> Documents => Set<Document>();

}