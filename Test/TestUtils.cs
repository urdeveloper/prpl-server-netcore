using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Collections.Generic;

namespace UrDeveloper.PrplServer.Test
{
    public static class TestUtils
    {
        public static TestServer CreateServer()
        {
            return new TestServer(new WebHostBuilder()
                                  .UseStartup<MultipleBuildsStartup>()
                                  .ConfigureLogging((hostingContext, logging) =>
                                  {
                                      logging.AddConsole();
                                      logging.AddDebug();
                                      logging.SetMinimumLevel(LogLevel.Debug);
                                  }));
        }

        public static async Task<Response> Get(this TestServer server, string path,
                                               string ua = null, Dictionary<string, string> headers = null)
        {
            var client = server.CreateClient();

            if (!string.IsNullOrEmpty(ua))
            {
                client.DefaultRequestHeaders.Add("User-Agent", ua);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = await client.GetAsync(path);

            var resp = new Response
            {
                Code = response.StatusCode,
                Data = await response.Content.ReadAsStringAsync(),
                Headers = response.Headers
            };

            return resp;
        }

        public static string GetHeader(this HttpHeaders headers, string name)
        {

            if (headers.TryGetValues(name, out IEnumerable<string> values))
            {
                return string.Join(",", values);
            }
            return null;
        }
    }
}