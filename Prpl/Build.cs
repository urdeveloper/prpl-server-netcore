using System;
using System.IO;
using System.Collections.Generic;

namespace UrDeveloper.PrplServer
{
    public class Build:IComparable
    {
        public string Name { get; set; }

        public int ConfigOrder { get; set; }

        public HashSet<BrowserCapability> Requirements {get;set;}

        public string EntryPoint { get; set; }

        public string BuildDir { get; set; }

        public string ServerRoot { get; set; }

        public Build()
        {
            //var pushManifestPath = Path.Combine(BuildDir, "push-manifest.json");
        }


        /// <summary>
        /// Return whether all requirements of this build are met by the given client
        /// browser capabilities.
        /// </summary>
        /// <param name="client">Client Browser Capabilities.</param>
        public bool CanServ(HashSet<BrowserCapability> client) => Requirements.SetEquals(client) || Requirements.IsProperSubsetOf(client);

        /// <summary>
        /// Order builds with more capabililties first -- a heuristic that assumes
        /// builds with more features are better.Ties are broken by the order the
        //// build appeared in the original configuration file.
        /// </summary>
        public int CompareTo(object obj)
        {
            var that = (Build)obj;

            if (Requirements.Count != that.Requirements.Count)
            {
                return that.Requirements.Count - Requirements.Count;
            }

            return ConfigOrder - that.ConfigOrder;
        }
    }
}
