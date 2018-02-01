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
            app.Use(async (context, next) =>
            {
                // To help test caching behavior, if the request URL includes this
                // magic string, we'll set the cache-control header to something
                // custom before calling prpl-handler. This is how we allow users to
                // take over control of the cache-control header.
                if (context.Request.Query.ContainsKey("custom-cache"))
                {
                    context.Response.Headers.Add("Cache-Control", "custom-cache");
                }

                await next.Invoke();
            });

            app.UsePrpl("static");
        }
    }
}