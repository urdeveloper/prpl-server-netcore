using System;
namespace UrDeveloper.PrplServer
{
    public class BuildConfiguration
    {
        public string Name { get; set; }
        public string EntryPoint { get; set; } = "index.html";
        public BrowserCapability[] BrowserCapabilities { get; set; }
    }
}