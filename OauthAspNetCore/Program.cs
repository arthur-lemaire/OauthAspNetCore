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
    // Sp�cifie le sch�ma d'authentification par d�faut utilis� pour les op�rations d'authentification.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    // Authority indique l'URL de base qui repr�sente l'�metteur du jeton (Firebase dans ce cas).
    // Utilisez l'ID du projet Firebase comme partie de l'URL.
    options.Authority = "https://securetoken.google.com/";

    // TokenValidationParameters sp�cifie les param�tres pour valider les jetons d'authentification.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // ValidateIssuer indique si l'�metteur du jeton doit �tre valid�.
        ValidateIssuer = true,
        // Sp�cifie l'ID de l'�metteur � valider. Utilisez l'URL avec l'ID du projet Firebase.
        ValidIssuer = "https://securetoken.google.com/",

        // ValidateAudience indique si l'audience du jeton doit �tre valid�e.
        ValidateAudience = true,
        // Sp�cifie l'ID de l'audience � valider. Utilisez l'ID du projet Firebase.
        ValidAudience = "",

        // ValidateLifetime indique si la p�riode de validit� du jeton doit �tre valid�e.
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
