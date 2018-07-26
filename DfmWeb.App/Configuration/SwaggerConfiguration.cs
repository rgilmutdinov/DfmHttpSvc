using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DfmWeb.App.Configuration.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Swashbuckle.AspNetCore.Swagger;

namespace DfmWeb.App.Configuration
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new Info
                    {
                        Version = "v1",
                        Title = "DfmWeb",
                        Description = "ASP.NET Core Web API for Doc-Filing Manager Web application",
                        TermsOfService = "Terms Of Service",
                        Contact = new Contact { Name = "CompEd Assistant", Email = "assistenza@comped.it", Url = "http://www.comped.it/it/contattaci" },
                        License = new License { Name = "License: Proprietary" }
                    }
                );

                // write routes in lowercase
                c.DocumentFilter<SwaggerLowercaseRouteFilter>();

                // allow file upload from swagger ui
                c.OperationFilter<SwaggerFileUploadFilter>();

                // allow input json in forms
                c.OperationFilter<SwaggerJsonFromFormFilter>();

                c.AddSecurityDefinition(
                    "Bearer",
                    new ApiKeyScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        In = "header",
                        Name = HeaderNames.Authorization,
                        Type = "apiKey"
                    }
                );

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> {
                    { "Bearer", Enumerable.Empty<string>() }
                });

                // Set the comments path for the Swagger JSON and UI.
                string basePath = AppContext.BaseDirectory;
                string xmlPath = Path.Combine(basePath, "DfmWeb.xml");
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static void UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "dfmweb api v1");
            });
        }
    }
}
