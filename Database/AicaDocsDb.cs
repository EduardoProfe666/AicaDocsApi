using AicaDocsApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AicaDocsApi.Database;

public class AicaDocsDb : IdentityDbContext<User, IdentityRole, string>
{
    public AicaDocsDb(DbContextOptions<AicaDocsDb> options)
        : base(options) { }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<Download> Downloads => Set<Download>();
    public DbSet<Nomenclator> Nomenclators => Set<Nomenclator>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Document
        modelBuilder.Entity<Nomenclator>()
            .HasMany<Document>()
            .WithOne()
            .HasForeignKey(e => e.TypeId)
            .IsRequired();
        modelBuilder.Entity<Nomenclator>()
            .HasMany<Document>()
            .WithOne()
            .HasForeignKey(e => e.ProcessId)
            .IsRequired();
        modelBuilder.Entity<Nomenclator>()
            .HasMany<Document>()
            .WithOne()
            .HasForeignKey(e => e.ScopeId)
            .IsRequired();
        
        // Download
        modelBuilder.Entity<Nomenclator>()
            .HasMany<Download>()
            .WithOne()
            .HasForeignKey(e => e.ReasonId)
            .IsRequired();
        modelBuilder.Entity<Document>()
            .HasMany<Download>()
            .WithOne()
            .HasForeignKey(e => e.DocumentId)
            .IsRequired();
    }

}