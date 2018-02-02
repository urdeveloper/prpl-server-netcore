using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace UrDeveloper.PrplServer
{
    public class PushManifest
    {
        HashSet<string> requestDestinations = new HashSet<string> {
            "",
            "audio",
            "document",
            "embed",
            "font",
            "image",
            "manifest",
            "object",
            "report",
            "script",
            "serviceworker",
            "sharedworker",
            "style",
            "track",
            "video",
            "worker",
            "xslt"
        };
        readonly Dictionary<Regex, Dictionary<string, PushManifestResource>> mapping = new Dictionary<Regex, Dictionary<string, PushManifestResource>>();

        string NormalizePath(string s, string basePath)
        {
            return s.StartsWith("/", StringComparison.Ordinal) ? s : Path.Combine(AddLeadingSlash(basePath), s).Replace("\\", "/");
        }

        string AddLeadingSlash(string s)
        {
            return s.StartsWith("/", StringComparison.Ordinal) ? s : "/" + s;
        }

        void ValidatePath(string s)
        {
            if (!Uri.TryCreate(s, UriKind.Relative, out Uri uriResult))
            {
                throw new Exception(string.Format("Invalid resource {0}", s));
            }
        }

        /// <summary>
        /// Create a new `PushManifest` from a JSON object which is expected to match
        /// the multi-file variant of the format described at
        /// https://github.com/GoogleChrome/http2-push-manifest.
        /// 
        /// The keys of this object are exact-match regular expression patterns that
        /// will be tested against the request URL path.
        /// 
        /// If `basePath` is set, relative paths in the push manifest (both patterns
        /// and resources) will be interpreted as relative to this directory.
        /// Typically it should be set to the path from the server file root to the
        /// push manifest file.
        ///
        /// Throws an exception if the given object does not match the manifest
        /// format, if a resource is not a valid URI path, or if `type` is not one of
        /// the valid request destinations
        /// (https://fetch.spec.whatwg.org/#concept-request-destination).
        ///
        /// Note that this class does not validate that resources exist on disk, since
        /// we can't assume if or how the server maps resources to disk.
        /// </summary>
        /// <param name="manifestJson">Manifest json. Returns null if the manifest is not valid.</param>
        /// <param name="basePath">Base path.</param>
        public PushManifest(string manifestJson, string basePath = "/")
        {
            var manifest = JsonConvert.DeserializeObject<PushManifestData>(manifestJson);

            foreach (var pattern in manifest.Keys)
            {
                var resources = new Dictionary<string, PushManifestResource>();
                foreach (var resource in manifest[pattern].Keys)
                {
                    ValidatePath(resource);

                    var type = manifest[pattern][resource].Type ?? "";

                    if (!requestDestinations.Contains(type))
                    {
                        throw new Exception(string.Format("Invalid Type {0}", type));
                    }

                    resources.Add(NormalizePath(resource, basePath), new PushManifestResource { Type = type });
                }

                if (resources.Any())
                {
                    string normalizedPattern;

                    if (pattern.StartsWith("^", StringComparison.Ordinal))
                    {
                        normalizedPattern = pattern;
                    }
                    else
                    {
                        normalizedPattern = "^" + NormalizePath(pattern, basePath);
                        if (!normalizedPattern.EndsWith("^", StringComparison.Ordinal))
                        {
                            normalizedPattern += "$";
                        }
                    }

                    mapping.Add(new Regex(normalizedPattern), resources);
                }
            }

        }

        /// <summary>
        /// Generate `Link: rel=preload` headers for each push resource associated
        /// with `path`.
        /// 
        /// A cooperating HTTP/2 server may intercept these headers and intiate a
        /// server push for each resource.
        /// 
        /// See https://w3c.github.io/preload/#server-push-http-2.
        /// </summary>
        /// <returns>The headers.</returns>
        /// <param name="path">Path.</param>
        public List<string> LinkHeaders(string path)
        {
            var result = new List<string>();

            var normalizedPath = AddLeadingSlash(path);

            var matchedMappings = mapping.Where(m => m.Value.Any() && m.Key.IsMatch(normalizedPath)).Select(m1 => m1.Value);

            foreach (var map in matchedMappings)
            {
                foreach (var resource in map)
                {
                    var header = "<" + resource.Key + ">; rel=preload";
                    if (!string.IsNullOrEmpty(resource.Value.Type))
                    {
                        header += "; as=" + resource.Value.Type;
                    }

                    result.Add(header);
                }
            }

            return result.ToList();
        }
    }
}
