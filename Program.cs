using System.Security.Claims;
using AicaDocsApi.Database;
using AicaDocsApi.Endpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
// builder.Services.AddAuthorizationBuilder();
// builder.Services.AddIdentityCore<IdentityUser>()
//     .AddEntityFrameworkStores<IdentityDbContext>()
//     .AddApiEndpoints();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AicaDocsDb>(opt => opt.UseInMemoryDatabase("AicaDocs"));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Aica Docs Api", Version = "0.1",
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.MapIdentityApi<IdentityUser>();
// app.MapGet("welcome", (ClaimsPrincipal cp) => $"Welcome {cp.Identity!.Name}")
//     .RequireAuthorization();

app.UseHttpsRedirection();
app.MapGeneralEndpoints();
app.MapDocumentEndpoints();


app.Run();
