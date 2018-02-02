using System;
using System.IO;
using Microsoft.AspNetCore.Rewrite;

namespace UrDeveloper.PrplServer
{
    public class RewriteAppRoute : IRule
    {
        string _entryPoint;

        public RewriteAppRoute(string entryPoint)
        {
            _entryPoint = entryPoint.StartsWith("/", StringComparison.InvariantCulture) ? entryPoint : "/" + entryPoint;
        }

        public void ApplyRule(RewriteContext context)
        {
            if (Path.HasExtension(context.HttpContext.Request.Path.Value)) return;

            context.Result = RuleResult.SkipRemainingRules;

            //Reserve the original path for later use
            context.HttpContext.Features.Set<ApplicationRouteFeature>(new ApplicationRouteFeature(context.HttpContext.Request.Path));

            context.HttpContext.Request.Path = _entryPoint;
        }
    }
}
