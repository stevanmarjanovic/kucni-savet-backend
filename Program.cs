using System.Security.Cryptography.X509Certificates;
using System.Text;
using KucniSavetBackend.Authorization.Handlers;
using KucniSavetBackend.Authorization.Requirements;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Interfaces.Services;
using KucniSavetBackend.Middleware;
using KucniSavetBackend.Repositories.RavenDB;
using KucniSavetBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ViteDevCors", policy =>
    {
        policy
            .WithOrigins("https://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IDocumentStore>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var clientCertificate = X509CertificateLoader.LoadPkcs12FromFile(
        "./Secret/cert.pfx",
        password: null
    );

    var store = new DocumentStore
    {
        Certificate = clientCertificate,
        Urls = configuration.GetSection("RavenDb:Urls").Get<string[]>(),
        Database = configuration["RavenDb:Database"]
    };

    store.Initialize();
    return store;
});

builder.Services.AddScoped<IAsyncDocumentSession>(sp =>
{
    var store = sp.GetRequiredService<IDocumentStore>();
    return store.OpenAsyncSession();
});

builder.Services.AddScoped<IHouseholdRepository, HouseholdRepository>();
builder.Services.AddScoped<IHouseholdService, HouseholdService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IChoreRepository, ChoreRepository>();
builder.Services.AddScoped<IChoreService, ChoreService>();

builder.Services.AddTransient<ExceptionMiddleware>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
            )
        };
    });
    
builder.Services.AddScoped<IAuthorizationHandler, HouseholdAuthorizationHandler>();
builder.Services.AddAuthorizationBuilder()
        .AddPolicy("CanEditHousehold", policy => policy.Requirements.Add(new HouseholdRequirement()));

builder.Services.AddHttpClient<AuthService>();
builder.Services.AddHttpClient<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("ViteDevCors");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
