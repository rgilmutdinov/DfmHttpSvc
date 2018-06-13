using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DfmHttpSvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool isConsole = Debugger.IsAttached || args.Contains("--console");

            RunWebHost(isConsole);
        }

        public static void RunWebHost(bool isConsole)
        {
            string contentRoot = Directory.GetCurrentDirectory();
            if (!isConsole)
            {
                string modulePath = Process.GetCurrentProcess().MainModule.FileName;
                contentRoot = Path.GetDirectoryName(modulePath);
            }

            IWebHostBuilder hostBuilder = new WebHostBuilder();
            string environment = hostBuilder.GetSetting(WebHostDefaults.EnvironmentKey);

            IConfigurationRoot hostingConfiguration = new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile("hosting.json", true)
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{environment}.json", true)
                .Build();

            hostBuilder
                .UseKestrel()
                .UseConfiguration(hostingConfiguration)
                .UseContentRoot(contentRoot)
                .ConfigureLogging(log =>
                {
                    log.AddConsole();
                    log.AddDebug();
                })
                .UseIISIntegration()
                .UseStartup<Startup>();

            IWebHost host = hostBuilder.Build();

            if (isConsole)
            {
                host.Run();
            }
            else
            {
                host.RunAsService();
            }
        }

    }
}
