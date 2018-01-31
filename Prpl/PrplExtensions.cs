using System;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;

namespace UrDeveloper.PrplServer
{
    public static class PrplExtensions
    {
        static Build[] LoadBuilds(string root, PrplConfiguration config)
        {
            return config.Builds.Select(b => new Build
            {
                Name = b.Name,
                Requirements = new HashSet<BrowserCapability>(b.BrowserCapabilities ?? new BrowserCapability[] { }),
                BuildDir = Path.Combine(root, b.Name),
                EntryPoint = b.EntryPoint,
                ServerRoot = root
            }).OrderBy(bb => bb).ToArray();
        }

        public static IApplicationBuilder UsePrpl(this IApplicationBuilder appBuilder, string root = "")
        {

            var env = appBuilder.ApplicationServices.GetService<IHostingEnvironment>();
            var config = appBuilder.ApplicationServices.GetService<IOptions<PrplConfiguration>>().Value;
            var absRoot = Path.Combine(env.ContentRootPath, root);
            var builds = LoadBuilds(absRoot, config);

            foreach (var build in builds)
            {
                appBuilder.UseWhen(context =>
                {
                    var clientBrowserCapabilities = BrowserCapabilities.GetCapabilities(context.Request.Headers["user-agent"]);
                    return build.CanServ(clientBrowserCapabilities);
                }, app =>
                {
                    var fullEntryPoint = build.Name + "/" + build.EntryPoint;

                    app.UseDefaultFiles(new DefaultFilesOptions()
                    {
                        DefaultFileNames = new List<string> { fullEntryPoint },
                        FileProvider = new PhysicalFileProvider(build.BuildDir),
                        RequestPath = new PathString("")
                    });

                    var rewriterOptions = new RewriteOptions
                    {
                        StaticFileProvider = new PhysicalFileProvider(build.BuildDir)
                    };

                    rewriterOptions.AddRewrite(".*(?<!\\..*?)$", fullEntryPoint, true);
                    app.UseRewriter(rewriterOptions);

                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(build.BuildDir),
                        RequestPath = new PathString("/" + build.Name),
                        OnPrepareResponse = (ctx) =>
                        {
                            if (ctx.File.Name.EndsWith("service-worker.js", StringComparison.Ordinal))
                            {
                                // A service worker may only register with a scope above its own path if
                                // permitted by this header.
                                // https://www.w3.org/TR/service-workers-1/#service-worker-allowed
                                ctx.Context.Response.Headers["service-worker-allowed"] = "/";
                            }

                            // Don't set the Cache-Control header if it's already set. This way another
                            // middleware can control caching, and we won't touch it.
                            if (!ctx.Context.Response.Headers["Cache-Control"].Any())
                            {
                                ctx.Context.Response.Headers["Cache-Control"] = ctx.File.Name == build.EntryPoint ? "max-age=0"
                                    : config.CacheControl;
                            }
                        }
                    });

                    app.UseStatusCodePages(new StatusCodePagesOptions
                    {
                        HandleAsync = async (ctx) =>
                        {
                            if (ctx.HttpContext.Response.StatusCode == 404)
                            {
                                if (config.UnregisterMissingServiceWorkers && ctx.HttpContext.Request.Path.Value.EndsWith("service-worker.js", StringComparison.Ordinal))
                                {
                                    ctx.HttpContext.Response.StatusCode = 200;

                                    ctx.HttpContext.Response.ContentType = "application/javascript";

                                    // A service worker may only register with a scope above its own path if
                                    // permitted by this header.
                                    // https://www.w3.org/TR/service-workers-1/#service-worker-allowed
                                    ctx.HttpContext.Response.Headers["service-worker-allowed"] = "/";

                                    await ctx.HttpContext.Response.WriteAsync("self.addEventListener('install', () => self.skipWaiting());" +
                                             "self.addEventListener('activate', () => self.registration.unregister());");
                                }
                            }
                        }
                    });

                });
            }

            return appBuilder;
        }
    }
}