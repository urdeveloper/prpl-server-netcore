using System;
using System.Collections.Generic;

namespace UrDeveloper.PrplServer
{
    public class PushManifestResource
    {
        public string Type { get; set; }
    }

    public class PushManifestData : Dictionary<string, Dictionary<string, PushManifestResource>>
    {
        public PushManifestData()
        {
        }
    }
}
