using AicaDocsApi.Database;
using AicaDocsApi.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<DocumentDb>(opt => opt.UseInMemoryDatabase("Documents"));
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Aica Docs Api", Version = "0.0.1",
            Contact = new()
            {
                Name = "Lilian Rosa Rojas Rodríguez | Eduardo Alejandro González Martell", Email = "eduardoprofe666@gmail.com",
                Url = new("https://github.com/EduardoProfe666/AicaDocsApi")
            },
            License = new() { Name = "MIT License" },
            Description = """
                          # 🚀 Aica Docs Api
                          ### Here goes the description
                          """
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapGeneralEndpoints();
app.MapDocumentEndpoints();


app.Run();