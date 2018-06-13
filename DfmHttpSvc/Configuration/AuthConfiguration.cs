using System.Text;
using DfmHttpSvc.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
