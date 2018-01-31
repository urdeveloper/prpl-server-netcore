using System.Net;
using System.Net.Http.Headers;

namespace UrDeveloper.PrplServer.Test
{
    public class Response
    {
        public HttpStatusCode Code { get; set; }
        public string Data { get; set; }
        public HttpResponseHeaders Headers { get; set; }
    }
}