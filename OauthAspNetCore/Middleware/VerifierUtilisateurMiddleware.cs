using FirebaseAdmin.Auth;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace OauthAspNetCore.Middleware
{
    public class VerifierUtilisateurMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifierUtilisateurMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authorizeAttribute = context.GetEndpoint()?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>();
            if (authorizeAttribute != null)
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Erreur : Token manquant.");
                    return;
                }

                if (!await ValidateTokenAsync(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Erreur : Token non valide.");
                    return;
                }
            }

            // Token valide, continuez le pipeline d'authentification
            await _next(context);
        }

        private async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var utilisateur = await FirebaseAuth.DefaultInstance.GetUserAsync(decoded.Uid);

                // Vérifiez l'état d'activation de l'utilisateur
                if (utilisateur.Disabled)
                {
                    // L'utilisateur est désactivé
                    return false;
                }
                return true; // Token valide
            }
            catch (Exception ex)
            {
                return false; // Token invalide
            }
        }
    }
}
