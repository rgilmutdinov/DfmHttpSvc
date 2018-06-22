using System.Collections.Generic;
using System.Text;
using DfmHttpSvc.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace DfmHttpSvc.Configuration
{
    public static class AuthConfiguration
    {
        public static IServiceCollection AddTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            AuthOptions authOptions = GetAuthOptions(configuration);

            services.AddSingleton(typeof(AuthOptions), authOptions);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => {
                    options.RequireHttpsMetadata      = false;
                    options.TokenValidationParameters = CreateTokenValidationParameters(authOptions);
                });

            return services;
        }

        public static void UseQueryStringTokenValidation(this IApplicationBuilder app)
        {
            // get access token (bearer) from query string parameters
            // it needs for file downloads
            app.Use(async (context, next) =>
            {
                if (context.Request.QueryString.HasValue)
                {
                    if (string.IsNullOrWhiteSpace(context.Request.Headers["Authorization"].ToString()))
                    {
                        Dictionary<string, StringValues> queryString = 
                            QueryHelpers.ParseQuery(context.Request.QueryString.Value);

                        string token = queryString["accessToken"].ToString();

                        if (!string.IsNullOrWhiteSpace(token))
                        {
                            context.Request.Headers.Add("Authorization", new[] { $"Bearer {token}" });
                        }
                    }
                }

                await next();
            });
        }

        private static TokenValidationParameters CreateTokenValidationParameters(AuthOptions authOptions)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer           = true,
                ValidateAudience         = true,
                ValidateLifetime         = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer      = authOptions.Issuer,
                ValidAudience    = authOptions.Audience,
                IssuerSigningKey = authOptions.SecurityKey
            };
        }

        private static AuthOptions GetAuthOptions(IConfiguration configuration)
        {
            string issuer         = configuration.GetValue("Token:Issuer", string.Empty);
            string audience       = configuration.GetValue("Token:Audience", string.Empty);
            string secretKey      = configuration.GetValue("Token:SecretKey", string.Empty);
            int    expirationDays = configuration.GetValue("Token:ExpirationDays", 5);

            return new AuthOptions
            {
                Issuer         = issuer,
                Audience       = audience,
                SecurityKey    = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
                ExpirationDays = expirationDays
            };
        }
    }
}
