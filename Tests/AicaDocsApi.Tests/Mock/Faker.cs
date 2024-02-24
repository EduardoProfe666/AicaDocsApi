using AicaDocsApi.Models;

namespace AicaDocsApi.Tests.Mock;

public static class Faker
{
    public static List<Nomenclator> GetFakeNomenclatorList()
    {
        return new List<Nomenclator>()
        {
            new() { Id = 1, Name = "P1", Type = TypeOfNomenclator.ProcessOfDocument },
            new() { Id = 2, Name = "P2", Type = TypeOfNomenclator.ProcessOfDocument },
            new() { Id = 3, Name = "P3", Type = TypeOfNomenclator.ProcessOfDocument },
            new() { Id = 4, Name = "R1", Type = TypeOfNomenclator.ReasonOfDownload },
            new() { Id = 5, Name = "R2", Type = TypeOfNomenclator.ReasonOfDownload },
            new() { Id = 6, Name = "R3", Type = TypeOfNomenclator.ReasonOfDownload },
            new() { Id = 7, Name = "S1", Type = TypeOfNomenclator.ScopeOfDocument },
            new() { Id = 8, Name = "S2", Type = TypeOfNomenclator.ScopeOfDocument },
            new() { Id = 9, Name = "S3", Type = TypeOfNomenclator.ScopeOfDocument },
            new() { Id = 10, Name = "T1", Type = TypeOfNomenclator.TypeOfDocument },
            new() { Id = 11, Name = "T2", Type = TypeOfNomenclator.TypeOfDocument },
            new() { Id = 12, Name = "T3", Type = TypeOfNomenclator.TypeOfDocument }
        };
    }

    public static List<Document> GetFakeDocumentList()
    {
        return new List<Document>()
        {
            new()
            {
                Id = 1, Title = "Tit1A", Code = "Cod1A", Edition = 1,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 1,
                ScopeId = 7, TypeId = 10, Pages = 1
            },
            new()
            {
                Id = 2, Title = "Tit2A", Code = "Cod2A", Edition = 2,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 1,
                ScopeId = 7, TypeId = 10, Pages = 1
            },
            new()
            {
                Id = 3, Title = "Tit3A", Code = "Cod3A", Edition = 3,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 1,
                ScopeId = 7, TypeId = 10, Pages = 1
            },
            new()
            {
                Id = 4, Title = "Tit1B", Code = "Cod1B", Edition = 1,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 2,
                ScopeId = 7, TypeId = 11, Pages = 3
            },
            new()
            {
                Id = 5, Title = "Tit2B", Code = "Cod2B", Edition = 2,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 2,
                ScopeId = 7, TypeId = 11, Pages = 4
            },
            new()
            {
                Id = 6, Title = "Tit3B", Code = "Cod3B", Edition = 3,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 2,
                ScopeId = 7, TypeId = 11, Pages = 4
            },
            new()
            {
                Id = 7, Title = "Tit1C", Code = "Cod1C", Edition = 1,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 1,
                ScopeId = 8, TypeId = 11, Pages = 4
            },
            new()
            {
                Id = 8, Title = "Tit1D", Code = "Cod1D", Edition = 1,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 1,
                ScopeId = 9, TypeId = 11, Pages = 3
            },
            new()
            {
                Id = 9, Title = "Tit1E", Code = "Cod1E", Edition = 1,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 1,
                ScopeId = 8, TypeId = 12, Pages = 3
            },
            new()
            {
                Id = 10, Title = "Tit1F", Code = "Cod1F", Edition = 1,
                DateOfValidity = DateTimeOffset.Now, ProcessId = 3,
                ScopeId = 7, TypeId = 11, Pages = 2
            }
        };
    }

    public static List<Download> GetFakeDownloadList()
    {
        return new List<Download>()
        {
            new()
            {
                Id = 1, Format = Format.Pdf, Username = "Usuario1", DocumentId = 1, ReasonId = 4,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 2, Format = Format.Word, Username = "Usuario1", DocumentId = 3, ReasonId = 5,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 3, Format = Format.Word, Username = "Usuario1", DocumentId = 5, ReasonId = 6,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 4, Format = Format.Pdf, Username = "Usuario2", DocumentId = 1, ReasonId = 4,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 5, Format = Format.Word, Username = "Usuario2", DocumentId = 2, ReasonId = 4,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 6, Format = Format.Pdf, Username = "Usuario2", DocumentId = 7, ReasonId = 5,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 7, Format = Format.Word, Username = "Usuario3", DocumentId = 8, ReasonId = 5,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 8, Format = Format.Pdf, Username = "Usuario4", DocumentId = 10, ReasonId = 6,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 7, Format = Format.Word, Username = "Usuario5", DocumentId = 4, ReasonId = 5,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 7, Format = Format.Word, Username = "Usuario5", DocumentId = 7, ReasonId = 4,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 8, Format = Format.Pdf, Username = "Usuario6", DocumentId = 8, ReasonId = 5,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 9, Format = Format.Word, Username = "Usuario6", DocumentId = 2, ReasonId = 6,
                DateOfDownload = DateTimeOffset.Now
            },
            new()
            {
                Id = 10, Format = Format.Pdf, Username = "Usuario7", DocumentId = 5, ReasonId = 5,
                DateOfDownload = DateTimeOffset.Now
            },
        };
    }
}