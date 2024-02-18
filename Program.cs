using AicaDocsApi.Database;
using AicaDocsApi.Endpoints;
using AicaDocsApi.Utils.BlobServices;
using AicaDocsApi.Utils.BlobServices.Minio;
using AicaDocsApi.Validators.Utils;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

// Validations Services
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddScoped<ValidateUtils>();

// Db Services
builder.Services.AddDbContext<AicaDocsDb>(x =>
    x.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));
// builder.Services.AddDbContext<AicaDocsDb>(opt => opt.UseInMemoryDatabase("AicaDocs"));

// Blob Services
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.AddScoped<IBlobService, MinioBlobService>();


// General Configuration Services
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Aica Docs Api", Version = "1.0.1",
            Contact = new()
            {
                Name = "Lilian Rosa Rojas Rodríguez | Eduardo Alejandro González Martell",
                Email = "eduardoprofe666@gmail.com",
                Url = new("https://github.com/EduardoProfe666/AicaDocsApi")
            },
            License = new() { Name = "MIT License" },
            Description = """
                          # 📝 Aica Docs Api
                          A simple API built with Asp.Net Core 8 that manages the
                          control documentation of the system of Quality Assurance
                          in AICA+ Pharmaceutical Laboratories.
                          """
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapGeneralEndpoints();
app.MapDocumentEndpoints();
app.MapDownloadEndpoints();
app.MapNomenclatorEndpoints();
app.MapPaginationEndpoints();

app.Run();
