using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace UrDeveloper.PrplServer
{
    public class PrplMiddleware
    {
        readonly RequestDelegate _next;
        readonly PrplConfiguration _config;
        readonly string _root;
        readonly Build[] _builds;

        public PrplMiddleware(RequestDelegate next, string root, IOptions<PrplConfiguration> options, IHostingEnvironment env)
        {
            _next = next;
            _root = Path.Combine(env.ContentRootPath, root);
            _config = options.Value;
            _builds = LoadBuilds();
        }

        public Task Invoke(HttpContext context)
        {
            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }

        Build[] LoadBuilds()
        {
            return new Build[] { };
        }
    }
}