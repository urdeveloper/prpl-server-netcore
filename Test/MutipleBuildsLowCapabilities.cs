using System;
using System.Net;
using System.Net.Http;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using System.Threading.Tasks;

namespace UrDeveloper.PrplServer.Test
{
    public class MutipleBuildsLowCapabilities
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public MutipleBuildsLowCapabilities()
        {
            _server = TestUtils.CreateServer();
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ServesEntryPointFromRootAsync()
        {
            var resp = await _server.Get("/");
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Contains("fallback entrypoint", resp.Data);
        }

        [Fact]
        public async Task ServesEntryPointForAppRoute()
        {
            var resp = await _server.Get("/foo/bar");
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Contains("fallback entrypoint", resp.Data);
        }

        [Fact]
        public async Task ServesAFragmentResource()
        {
            var resp = await _server.Get("/fallback/fragment.html");
            Assert.Equal(HttpStatusCode.OK, resp.Code);
            Assert.Contains("fallback fragment", resp.Data);
        }

        [Fact]
        public async Task Serves404ForMissingFile()
        {
            var resp = await _server.Get("/foo.png");
            Assert.Equal(HttpStatusCode.NotFound,resp.Code);

        }
    }
}
