using System;
namespace UrDeveloper.PrplServer
{
    public class ApplicationRouteFeature
    {
        public string Path { get; set; }

        public ApplicationRouteFeature(string path)
        {
            Path = path;
        }
    }
}
