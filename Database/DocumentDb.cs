using AicaDocsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Database;

public class DocumentDb : DbContext
{
    public DocumentDb(DbContextOptions<DocumentDb> options)
        : base(options) { }

    public DbSet<Document> Documents => Set<Document>();
    
}