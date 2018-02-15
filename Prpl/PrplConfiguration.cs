using System;
using System.Collections.Generic;

namespace UrDeveloper.PrplServer
{
    public class PrplConfiguration
    {
        /// <summary>
        /// The Cache-Control header to send for all requests except the entrypoint.
        /// Defaults to `max-age=60`.
        /// </summary>
        /// <value>The cache control.</value>
        public string CacheControl { get; set; } = "max-age=60";

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:UrDeveloper.PrplServer.Configuration"/> forward errors.
        /// </summary>
        /// <value>
        /// If <c>true</c>, when a 404 or other HTTP error occurs, the next middleware
        /// will be called with the error, so that it can be handled by
        /// downstream error handling middleware.
        ///
        /// If <c>false</c> (or if there was no `next` function because Express is not
        /// being used), a minimal `text/plain` error will be returned.
        ///
        /// Defaults to <c>false</c>.
        /// </value>
        public bool ForwardErrors { get; set; } = false;

        /// <summary>
        /// Serves a tiny self-unregistering service worker for any request path
        /// ending with `service-worker.js` that would otherwise have had a 404 Not
        /// Found response.
        //
        /// This can be useful when the location of a service worker has changed, as
        /// it will prevent clients from getting stuck with an old service worker
        /// indefinitely.
        //
        /// This problem arises because when a service worker updates, a 404 is
        /// treated as a failed update. It does not cause the service worker to be
        /// unregistered. See https://github.com/w3c/ServiceWorker/issues/204 for more
        /// discussion of this problem.
        /// </summary>
        /// <value>Defaults to <c>true</c>.</value>
        public bool UnregisterMissingServiceWorkers { get; set; } = true;

        public string EntryPoint { get; set; }

        public List<BuildConfiguration> Builds { get; set; }

        public bool UseNodeModules { get; set; }

        public string BowerPath { get; set; }
    }
}
