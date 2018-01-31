using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace UrDeveloper.PrplServer.Test
{
    public class MultipleBuildsStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<PrplConfiguration>(config =>
            {
                config.Builds = new List<BuildConfiguration> 
                {
                    new BuildConfiguration
                    {
                        Name = "fallback"
                    },
                    new BuildConfiguration
                    {
                        Name = "es2015",
                        BrowserCapabilities = new BrowserCapability[]
                        { 
                            BrowserCapability.es2015 
                        }
                    }
                };
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UsePrpl("static");
        }
    }
}