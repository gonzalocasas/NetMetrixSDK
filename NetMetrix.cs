using System;
using System.Diagnostics;
using Microsoft.Phone.Controls;

namespace NetMetrixReporter
{
    public static class NetMetrix
    {
        private static readonly Tracker tracker = new Tracker { CookieStore = new CookieStore() };

        public static string Host { get; set; }
        public static string OfferId { get; set; }
        public static string AppId { get; set; }

        static NetMetrix()
        {
            Host = "wemfbox.ch";
            DebugHostFilter();
        }

        // The net-metrix host is defined statically, but overriden with the test host if DEBUG
        [Conditional("DEBUG")]
        private static void DebugHostFilter()
        {
            Host = "wemfbox-test.ch";
        }

        public static Tracker Tracker
        {
            get { return tracker; }
        }
    }
}
