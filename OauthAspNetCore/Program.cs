using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OauthAspNetCore.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var credentials = GoogleCredential.FromFile("./firebase-adminsdk.json");
FirebaseApp.Create(new AppOptions
{
    Credential = credentials,
});

builder.Services.AddAuthentication(options =>
{
    // Spécifie le schéma d'authentification par défaut utilisé pour les opérations d'authentification.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Authority indique l'URL de base qui représente l'émetteur du jeton (Firebase dans ce cas).
    // Utilisez l'ID du projet Firebase comme partie de l'URL.
    options.Authority = "https://securetoken.google.com/";

    // TokenValidationParameters spécifie les paramètres pour valider les jetons d'authentification.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ValidateIssuer indique si l'émetteur du jeton doit être validé.
        ValidateIssuer = true,
        // Spécifie l'ID de l'émetteur à valider. Utilisez l'URL avec l'ID du projet Firebase.
        ValidIssuer = "https://securetoken.google.com/",

        // ValidateAudience indique si l'audience du jeton doit être validée.
        ValidateAudience = true,
        // Spécifie l'ID de l'audience à valider. Utilisez l'ID du projet Firebase.
        ValidAudience = "",

        // ValidateLifetime indique si la période de validité du jeton doit être validée.
        ValidateLifetime = true
    };
});

var app = builder.Build();

app.UseCors("AllowAngularDev");

// ... autres configurations ...

app.UseRouting();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseMiddleware<VerifierUtilisateurMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
