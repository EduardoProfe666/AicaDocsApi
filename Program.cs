using AicaDocsApi.Database;
using AicaDocsApi.Endpoints;
using dotenv.net;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();


var connection = ConstructConnectionString();
builder.Services.AddDbContext<AicaDocsDb>(x => x.UseNpgsql(connection));
// builder.Services.AddDbContext<AicaDocsDb>(x => x.UseSqlite("DataSource=db.dat"));
// builder.Services.AddDbContext<AicaDocsDb>(opt => opt.UseInMemoryDatabase("AicaDocs"));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Aica Docs Api", Version = "0.2",
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

string ConstructConnectionString()
{
    DotEnv.Load();
    var server = Environment.GetEnvironmentVariable("SERVER");
    var port = Environment.GetEnvironmentVariable("PORT");
    var database = Environment.GetEnvironmentVariable("DATABASE");
    var user = Environment.GetEnvironmentVariable("USER");
    var password = Environment.GetEnvironmentVariable("PASSWORD");
    return $"Server={server};Port={port};Database={database};User Id={user};password={password}";
}