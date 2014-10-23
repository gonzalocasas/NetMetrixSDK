using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Phone.Controls;

namespace NetMetrixSdk
{
    public static class NetMetrix
    {
        private static readonly Tracker tracker = new Tracker { CookieStore = new CookieStore() };

        public static string Host { get; set; }
        public static string OfferId { get; set; }
        public static string AppId { get; set; }
        public static string Referer { get; set; }

        static NetMetrix()
        {
            Host = Debugger.IsAttached ? "wemfbox-test.ch" : "wemfbox.ch";

            var root = XDocument.Load(@"Properties\NetMetrix.xml").Root;
            if (root == null) throw new InvalidOperationException("No NetMetrix.xml configuration file found");

            var offer = root.Element("OfferID");
            if (offer == null || string.IsNullOrWhiteSpace(offer.Value)) throw new InvalidOperationException("OfferID missing in NetMetrix.xml");
            OfferId = offer.Value;

            var app = root.Element("AppID");
            if (app == null || string.IsNullOrWhiteSpace(app.Value)) throw new InvalidOperationException("AppID missing in NetMetrix.xml");
            AppId = app.Value;

            var referer = root.Element("Referer");
            if (referer == null || string.IsNullOrWhiteSpace(referer.Value)) throw new InvalidOperationException("Referer missing in NetMetrix.xml");
            Referer = referer.Value;
        }

        public static Tracker Tracker
        {
            get { return tracker; }
        }

        public static void EnableNavigationTracker(PhoneApplicationFrame appFrame)
        {
            appFrame.Navigated += (sender, args) =>
            {
                // Content is null when navigating outside the app
                if (args.Content == null) return;

                var section = GetSectionName(args.Content);
                Tracker.Track(section);
            };
        }

        private static string GetSectionName(object content)
        {
            try
            {
                var attribute =
                    (NetMetrixAttribute) Attribute.GetCustomAttribute(content.GetType(), typeof (NetMetrixAttribute));
                return attribute != null ? attribute.Section : Tracker.DefaultSection;
            }
            catch
            {
                return Tracker.DefaultSection;
            }
        }
    }
}
