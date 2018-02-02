using System;
using System.Net;
using System.Linq;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace UrDeveloper.PrplServer.Test
{
    public class MutipleBuildsHighCapabilities
    {
        readonly TestServer _server;
        const string CHROME_UA = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_4) " +
                    "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

        public MutipleBuildsHighCapabilities()
        {
            _server = TestUtils.CreateServer();
        }

        [Fact]
        public async Task ServesEntryPointFromRootAsync()
        {
            var resp = await _server.Get("/", CHROME_UA);
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Contains("es2015 entrypoint", resp.Data);
        }

        [Fact]
        public async Task ServesEntryPointForAppRoute()
        {
            var resp = await _server.Get("/foo/bar", CHROME_UA);
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Contains("es2015 entrypoint", resp.Data);
        }

        [Fact]
        public async Task ServesAFragmentResource()
        {
            var resp = await _server.Get("/es2015/fragment.html", CHROME_UA);
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Contains("es2015 fragment", resp.Data);
        }

        [Fact]
        public async Task Serves404ForMissingFile()
        {
            var resp = await _server.Get("/foo.png", CHROME_UA);
            Assert.Equal(HttpStatusCode.NotFound, resp.Code);
        }

        [Fact]
        public async Task SetsPushHeadersForFragment()
        {
            var resp = await _server.Get("/es2015/fragment.html", CHROME_UA);
            Assert.Equal("</es2015/baz.html>; rel=preload; as=document", resp.Headers.GetHeader("link"));
        }

        [Fact]
        public async Task SetsPushHeadersForExplicitEntrypoint()
        {
            var resp = await _server.Get("/es2015/index.html", CHROME_UA);
            var linkHeader = resp.Headers.GetHeader("link");
            Assert.Equal("</es2015/fragment.html>; rel=preload; as=document," +
                         "</es2015/serviceworker.js>; rel=preload; as=script", linkHeader);
        }

        [Fact]
        public async Task SetsPushHeadersForApplicationRoute()
        {
            var resp = await _server.Get("/foo/bar", CHROME_UA);
            var linkHeader = resp.Headers.GetHeader("link");
            Assert.Equal("</es2015/foo.html>; rel=preload; as=document," +
                         "</es2015/fragment.html>; rel=preload; as=document," +
                         "</es2015/serviceworker.js>; rel=preload; as=script", linkHeader);
        }

        [Fact]
        public async Task SetsServiceWorkerAllowedHeader()
        {
            var resp = await _server.Get("/es2015/service-worker.js", CHROME_UA);
            Assert.Equal("/", resp.Headers.GetHeader("service-worker-allowed"));
        }

        [Fact]
        public async Task AutomaticallyUnregisterMissingServiceWorkers()
        {
            var resp = await _server.Get("/service-worker.js", CHROME_UA);
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Equal("/", resp.Headers.GetHeader("service-worker-allowed"));
            Assert.Contains("registration.unregister", resp.Data);
        }

        [Fact]
        public async Task SetsDefaultCacheHeaderOnStaticFile()
        {
            var resp = await _server.Get("/es2015/fragment.html", CHROME_UA);
            Assert.Equal("max-age=60", resp.Headers.GetHeader("cache-control"));
        }

        [Fact]
        public async Task SetsZeroCacheHeaderOnEntrypoint()
        {
            var resp = await _server.Get("/foor/bar", CHROME_UA);
            Assert.Equal("max-age=0", resp.Headers.GetHeader("cache-control"));
        }

        [Fact]
        public async Task DoesNotSetCacheHeaderIfAlreadySet()
        {
            var resp = await _server.Get("/foo/bar?custom-cache", CHROME_UA);
            Assert.Equal("custom-cache", resp.Headers.GetHeader("cache-control"));
        }

        [Fact]
        public async Task SendsEtagResponseHeader()
        {
            var resp = await _server.Get("/es2015/fragment.html", CHROME_UA);
            Assert.NotNull(resp.Headers.GetHeader("etag"));
        }

        [Fact]
        public async Task RespectsETagRequestHeader()
        {
            var resp = await _server.Get("/es2015/fragment.html", CHROME_UA);
            var tag = resp.Headers.GetHeader("etag");
            resp = await _server.Get("/es2015/fragment.html", CHROME_UA, new Dictionary<string, string> { { "If-None-Match", tag } });
            Assert.Equal(HttpStatusCode.NotModified, resp.Code);
            Assert.Equal("", resp.Data);
        }
    }
}
