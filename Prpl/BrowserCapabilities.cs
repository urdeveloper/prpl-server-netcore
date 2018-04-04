using System;
using System.Collections.Generic;
using System.Linq;

namespace UrDeveloper.PrplServer
{
    public static class BrowserCapabilities
    {

        static Dictionary<string, BrowserPredicates> _browsers;
        static readonly UAParser.Parser _uaParser;

        static BrowserCapabilities()
        {
            var chrome = new BrowserPredicates()
                .AddCapabilty(BrowserCapability.es2015, Since(49))
                .AddCapabilty(BrowserCapability.push, Since(41))
                .AddCapabilty(BrowserCapability.serviceworker, Since(45))
                .AddCapabilty(BrowserCapability.modules, Since(61));

            _browsers = new Dictionary<string, BrowserPredicates>
            {
                {"Chrome", chrome},
                {"Chromium", chrome},
                {"HeadlessChrome", chrome},
                {
                    "OPR",
                    new BrowserPredicates()
                        .AddCapabilty(BrowserCapability.es2015,Since(36))
                        .AddCapabilty(BrowserCapability.push,Since(28))
                        .AddCapabilty(BrowserCapability.serviceworker,Since(32))
                        .AddCapabilty(BrowserCapability.modules,Since(48))
                },
                {
                    "Vivaldi",
                    new BrowserPredicates()
                        .AddCapabilty(BrowserCapability.es2015,Since(1))
                        .AddCapabilty(BrowserCapability.push,Since(1))
                        .AddCapabilty(BrowserCapability.serviceworker,Since(1))
                        .AddCapabilty(BrowserCapability.modules,ua=>false)
                },
                {
                    "Mobile Safari",
                    new BrowserPredicates()
                        .AddCapabilty(BrowserCapability.es2015,Since(10))
                        .AddCapabilty(BrowserCapability.push,Since(9,2))
                        .AddCapabilty(BrowserCapability.serviceworker,Since(11,3))
                        .AddCapabilty(BrowserCapability.modules,Since(10,3))
                },
                {
                    "Safari",
                    new BrowserPredicates()
                        .AddCapabilty(BrowserCapability.es2015,Since(10))
                        .AddCapabilty(BrowserCapability.push,ua =>
                                      VersionAtLeast(new int[]{9}, new int[]{ParseInt(ua.UserAgent.Major), ParseInt(ua.UserAgent.Minor), ParseInt(ua.UserAgent.Patch)})
                                      && VersionAtLeast(new int[]{10,11}, new int[]{ParseInt(ua.OS.Major), ParseInt(ua.OS.Minor), ParseInt(ua.OS.Patch)}))
                        .AddCapabilty(BrowserCapability.serviceworker,Since(11,1))
                        .AddCapabilty(BrowserCapability.modules,Since(10,1))
                },
                {
                    // Edge versions before 15.15063 may contain a JIT bug affecting ES6
                    // constructors (https://github.com/Microsoft/ChakraCore/issues/1496).
                    "Edge",
                    new BrowserPredicates()
                        .AddCapabilty(BrowserCapability.es2015,Since(15, 15063))
                        .AddCapabilty(BrowserCapability.push,Since(12))
                        // https://developer.microsoft.com/en-us/microsoft-edge/platform/status/serviceworker/
                        .AddCapabilty(BrowserCapability.serviceworker,ua=>false)
                        .AddCapabilty(BrowserCapability.modules,ua=>false)
                },
                {
                    "Firefox",
                    new BrowserPredicates()
                        .AddCapabilty(BrowserCapability.es2015,Since(51))
                        // Firefox bug - https://bugzilla.mozilla.org/show_bug.cgi?id=1409570
                        .AddCapabilty(BrowserCapability.push,ua=>false)
                        .AddCapabilty(BrowserCapability.serviceworker,Since(44))
                        .AddCapabilty(BrowserCapability.modules,ua=>false)
                }
            };

            var regexes = System.IO.File.ReadAllText("regexes.yaml");
            _uaParser = UAParser.Parser.FromYaml(regexes);
        }

        public static HashSet<BrowserCapability> GetCapabilities(string userAgent)
        {
            var ua = _uaParser.Parse(userAgent ?? "");

            if (_browsers.TryGetValue(ua.UserAgent.Family, out BrowserPredicates predicates))
            {
                var caps = predicates.Where(p => p.Value(ua)).Select(pp => pp.Key);
                return new HashSet<BrowserCapability>(caps);
            }

            return new HashSet<BrowserCapability>();
        }

        static int ParseInt(string s)
        {
            int.TryParse(s, out int i);
            return i;
        }

        /// <summary>
        ///  Make a predicate that checks if the browser version is at least this high.
        /// </summary>
        public static bool VersionAtLeast(int[] atLeast, int[] version)
        {
            for (var i = 0; i < atLeast.Length; i++)
            {
                var r = atLeast[i];
                var v = version.Length > i ? version[i] : 0;
                if (v > r)
                {
                    return true;
                }
                if (v < r)
                {
                    return false;
                }
            }
            return true;
        }

        static Func<UAParser.ClientInfo, bool> Since(params int[] atLeast)
        {
            return (ua) => VersionAtLeast(atLeast, new int[] { ParseInt(ua.UserAgent.Major), ParseInt(ua.UserAgent.Minor), ParseInt(ua.UserAgent.Patch) });
        }
    }
}
