using System;
namespace UrDeveloper.PrplServer
{
    public enum BrowserCapability
    {
        // ECMAScript 2015 (aka ES6).
        es2015,
        // HTTP/2 Server Push.
        push,
        // Service Worker API.
        serviceworker,
        // JavaScript modules.
        modules
    }
}