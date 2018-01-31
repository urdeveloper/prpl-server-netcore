using System;
using System.Collections.Generic;

namespace UrDeveloper.PrplServer
{
    public class BrowserPredicates: Dictionary<BrowserCapability,Func<UAParser.ClientInfo,bool>>
    {
        public BrowserPredicates AddCapabilty(BrowserCapability cap, Func<UAParser.ClientInfo,bool> predicate) {
            Add(cap,predicate);
            return this;
        }
    }
}
