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


builder.Services.AddDbContext<AicaDocsDb>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));
// builder.Services.AddDbContext<AicaDocsDb>(x => x.UseSqlite("DataSource=db.dat"));
// builder.Services.AddDbContext<AicaDocsDb>(opt => opt.UseInMemoryDatabase("AicaDocs"));

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

app.Run();
