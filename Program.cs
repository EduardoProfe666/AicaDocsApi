using AicaDocsApi.Database;
using AicaDocsApi.Endpoints;
using AicaDocsApi.Utils;
using AicaDocsApi.Validators.Utils;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddScoped<ValidateUtils>();

builder.Services.AddDbContext<AicaDocsDb>(x =>
    x.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));
// builder.Services.AddDbContext<AicaDocsDb>(opt => opt.UseInMemoryDatabase("AicaDocs"));

var minioInfo = builder.Configuration.GetSection("Minio");
builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(minioInfo["endpoint"])
    .WithCredentials(minioInfo["accessKey"], minioInfo["secretKey"])
    .WithSSL(false));
builder.Services.AddSingleton(new BucketNameProvider(minioInfo["bucket"]!));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Aica Docs Api", Version = "0.9",
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

app.Run();