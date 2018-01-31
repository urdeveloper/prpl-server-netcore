using System;
using System.Collections.Generic;
using Xunit;

namespace UrDeveloper.PrplServer.Test
{
    public class Capabilities
    {
        public Capabilities()
        {
        }

        void AssertBrowserCapabilities(string userAgent, params BrowserCapability[] caps)
        {
            var actual = BrowserCapabilities.GetCapabilities(userAgent);
            var expected = new HashSet<BrowserCapability>(caps);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void UnknownBrowserHasNoCapabilities()
        {
            AssertBrowserCapabilities("unknown browser");
        }

        [Fact]
        public void ChromeHasAllTheCapabilities()
        {
            AssertBrowserCapabilities(
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.31 Safari/537.36",
                    BrowserCapability.es2015, BrowserCapability.push, BrowserCapability.serviceworker, BrowserCapability.modules);
        }

        [Fact]
        public void ChromeHeadlessHasAllTheCapabilities()
        {
            AssertBrowserCapabilities(
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_6) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/61.0.3163.31 Safari/537.36",
                    BrowserCapability.es2015, BrowserCapability.push, BrowserCapability.serviceworker, BrowserCapability.modules);
        }

        [Fact]
        public void EdgeEs2015SupportIsPredicatedOnMinoBrowserVersion()
        {
            AssertBrowserCapabilities(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.14986",
                    BrowserCapability.push);

            AssertBrowserCapabilities(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063",
                    BrowserCapability.es2015, BrowserCapability.push);

        }

        [Fact]
        public void SafariPushCapabilityIsPredicatedOnMacOSVersion()
        {
            AssertBrowserCapabilities(
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.1 Safari/603.1.30",
                    BrowserCapability.es2015, BrowserCapability.modules);

            AssertBrowserCapabilities(
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.1 Safari/603.1.30",
                    BrowserCapability.es2015, BrowserCapability.push, BrowserCapability.modules);
        }

        [Fact]
        public void VersionAtLeastChecksAllRequiredPart()
        {
            Assert.True(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 3, 2, 1 }));
            Assert.True(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 3, 2, 1, 4 }));
            Assert.True(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 4, 1, 0 }));
            Assert.True(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 0 }, new int[] { 3, 2 }));
            Assert.False(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 2, 2, 1 }));
            Assert.False(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 3, 1, 1 }));
            Assert.False(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 3, 1, 0 }));
            Assert.False(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 3, 2 }));
            Assert.False(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { 3, 2 }));
            Assert.False(BrowserCapabilities.VersionAtLeast(new int[] { 3, 2, 1 }, new int[] { }));
        }
    }
}