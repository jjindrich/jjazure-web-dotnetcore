﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace jjwebcore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var settings = config.Build();
                        config.AddAzureAppConfiguration(options =>
                        {
                            // ENV ConnectionStrings__AppConfig
                            options.Connect(settings["ConnectionStrings:AppConfig"])
                                .UseFeatureFlags();
                        });
                    });
                    //webBuilder.ConfigureLogging((hostingContext, logging) => { logging.AddConsole(); });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
