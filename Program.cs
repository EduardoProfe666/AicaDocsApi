using System.Net;
using System.Reactive.Linq;
using AicaDocsApi.Database;
using AicaDocsApi.Endpoints;
using AicaDocsApi.Utils;
using AicaDocsApi.Validators.Utils;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

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
            Title = "Aica Docs Api", Version = "0.7",
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

app.MapGet("/buckets", async (IMinioClient minioClient) =>
{
    var buckets = await minioClient.ListBucketsAsync();
    return TypedResults.Ok(buckets.Buckets);
});

app.MapGet("/bucketName", (BucketNameProvider b) => b.BucketName);

app.MapGet("/download-object", async (BucketNameProvider bucketNameProvider, IMinioClient minioClient) =>
{
    var args = new PresignedGetObjectArgs()
        .WithBucket(bucketNameProvider.BucketName)
        .WithObject("/pdf/lshort-a4.pdf")
        .WithExpiry(60 * 5);
    return TypedResults.Ok(await minioClient.PresignedGetObjectAsync(args));
});

app.MapPost("/upload-image",
    async ([FromForm] IFormFile file, BucketNameProvider bucketNameProvider, IMinioClient minioClient) =>
    {
        await using var fileStream = file.OpenReadStream();
        var poa = new PutObjectArgs()
            .WithBucket(bucketNameProvider.BucketName)
            .WithObject("image.png")
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("image/png");
        await minioClient.PutObjectAsync(poa);
        
        return TypedResults.Ok();
    }).DisableAntiforgery();

app.MapGet("/check-object/", async (BucketNameProvider bucketNameProvider, IMinioClient minioClient) =>
{
    var listObjectsArgs = new ListObjectsArgs()
        .WithBucket(bucketNameProvider.BucketName)
        .WithPrefix("c#.txt");
    var a = true;
    try
    {
        await minioClient.ListObjectsAsync(listObjectsArgs);
    }
    catch
    {
        a = false;
    }
    

    return Results.Json(new { Exists = a });
});

app.Run();