using AicaDocsApi.Database;
using AicaDocsApi.Endpoints;
using AicaDocsApi.Models;
using AicaDocsApi.Utils.BlobServices;
using AicaDocsApi.Utils.BlobServices.Minio;
using AicaDocsApi.Validators.Utils;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Validations Services
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationRulesToSwagger();
builder.Services.AddScoped<ValidateUtils>();

// Identity Services
builder.Services.AddIdentityCore<User>()
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<AicaDocsDb>()
    .AddApiEndpoints();
builder.Services.AddAuthorization().AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorizationBuilder().AddPolicy("api", p =>
{
    p.RequireAuthenticatedUser();
    p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
})
.AddPolicy("Admin", p =>
{
    p.RequireAuthenticatedUser();
    p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
    p.RequireRole("Admin");
})
.AddPolicy("Worker", p =>
{
    p.RequireAuthenticatedUser();
    p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
    p.RequireRole("Worker");
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;
});

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
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapCustomIdentityApi();
app.MapGeneralEndpoints();
app.MapDocumentEndpoints();
app.MapDownloadEndpoints();
app.MapNomenclatorEndpoints();

// Default Roles, an Admin and a Worker
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    
    // Default Roles
    string[] roleNames = { "Admin", "Worker" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Admin
    var emailAdmin = "aicadocsadmin@admin.cu";
    var fullnameAdmin = "Admin";
    var passwordAdmin = "AicaDocs_Admin1!";

    var adminPrincipalExist = await userManager.FindByEmailAsync(emailAdmin);
    if (adminPrincipalExist is null)
    {
        var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<User>>();
        var emailStore = (IUserEmailStore<User>)userStore;
        var user = new User();
        user.FullName = fullnameAdmin;
        await userStore.SetUserNameAsync(user, emailAdmin, CancellationToken.None);
        await emailStore.SetEmailAsync(user, emailAdmin, CancellationToken.None);
        await userManager.CreateAsync(user, passwordAdmin);
        await userManager.AddToRoleAsync(user,"Admin");
    }
    
    // Worker
    var emailWorker = "aicadocsworker@worker.cu";
    var fullnameWorker = "Worker";
    var passwordWorker = "AicaDocs_Worker1!";

    var workerPrincipalExist = await userManager.FindByEmailAsync(emailWorker);
    if (workerPrincipalExist is null)
    {
        var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<User>>();
        var emailStore = (IUserEmailStore<User>)userStore;
        var user = new User();
        user.FullName = fullnameWorker;
        await userStore.SetUserNameAsync(user, emailWorker, CancellationToken.None);
        await emailStore.SetEmailAsync(user, emailWorker, CancellationToken.None);
        await userManager.CreateAsync(user, passwordWorker);
        await userManager.AddToRoleAsync(user,"Worker");
    }

}


app.Run();
