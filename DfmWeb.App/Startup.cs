using DfmWeb.App.Configuration;
using DfmWeb.App.Sessions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace DfmWeb.App
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            HostingEnvironment = hostingEnvironment;
            Configuration      = configuration;
        }

        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration      Configuration      { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SessionManager>();

            services.Configure<FormOptions>(options => {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // multipart body limit
            });

            services.AddTokenAuthentication(Configuration);
            services.AddRouting(options => options.LowercaseUrls = true);

            services
                .AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)))
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddSwaggerDocumentation();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseSwaggerDocumentation();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    HotModuleReplacementEndpoint = "/__webpack_hmr"
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "catch-all",
                    template: "{*url}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
