using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace MeetingScheduler.UI
{
    public class Program
    {
        private static HttpClient httpClient;

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder().Build();
            //CreateHostBuilder(args).Build().Run();
            var logger = LogManager.Setup()
                                   .RegisterNLogWeb(config)
                                   .LoadConfigurationFromFile("nlog.config")
                                   .GetCurrentClassLogger();
            try
            {
                //logger.Debug("init main");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }


        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(true)
                    .UseIISIntegration()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseStartup<Startup>()
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        //logging.ClearProviders();
                        //logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    });
                })
                .UseNLog();
        }
    }
}
