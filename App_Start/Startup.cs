using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode; // Explicit alias

[assembly: OwinStartup(typeof(Van_Rise_Intern_App.Startup))]
namespace Van_Rise_Intern_App
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var jwtSecret = WebConfigurationManager.AppSettings["JwtSecret"];
            var key = Encoding.ASCII.GetBytes(jwtSecret);

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active, // No more ambiguity
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                },
                Provider = new OAuthBearerAuthenticationProvider
                {
                    OnRequestToken = context =>
                    {
                        Console.WriteLine("Token requested from: " + context.Request.Uri);
                        Console.WriteLine("Received Token: " + context.Token);
                        return Task.FromResult(0);
                    },
                    OnValidateIdentity = context =>
                    {
                        Console.WriteLine("Validating token...");
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}
