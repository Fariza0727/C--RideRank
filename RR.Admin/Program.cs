using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using System.IO;
using System.Text;

namespace RR.Admin
{
    public class Program
    {

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
                .WriteTo.File(new JsonFormatter(","), string.Concat(hostingContext.HostingEnvironment.ContentRootPath, "/Logs/log.json"), rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                , true);
    }
}

