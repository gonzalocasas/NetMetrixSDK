using System;
using System.Diagnostics;
using Microsoft.Phone.Controls;

namespace NetMetrixSdk
{
    public static class NetMetrix
    {
        private static readonly Tracker tracker = new Tracker { CookieStore = new CookieStore() };

        public static string Host { get; set; }
        public static string OfferId { get; set; }
        public static string AppId { get; set; }

        static NetMetrix()
        {
            Host = Debugger.IsAttached ? "wemfbox-test.ch" : "wemfbox.ch";
        }

        public static Tracker Tracker
        {
            get { return tracker; }
        }

        public static void EnableNavigationTracker(PhoneApplicationFrame appFrame)
        {
            appFrame.Navigated += (sender, args) =>
            {
                Tracker.Track();
            };
        }
    }
}
