using System;
using System.Linq;
using System.Net;
using System.Threading;

namespace NetMetrixSdk
{
    public class Tracker
    {
        public const string DefaultSection = "general";
        private DateTime lastRequest = DateTime.MinValue;

        public CookieStore CookieStore { get; set; }

        protected virtual Uri BaseDomain
        {
            get { return new Uri(string.Format("http://{1}.{0}", NetMetrix.Host, NetMetrix.OfferId)); }
        }

        protected virtual string CookieStorageKey
        {
            get { return "NetMetrixReporter-Cookie-" + NetMetrix.Host; }
        }

        /// <summary>
        /// <para>Registers a view on net-metrix system.</para>
        /// <para>Tracking calls must be separated by at least 2 seconds between each other,
        /// otherwise they are considered as double counting by Net-Metrix.</para>
        /// <para>This call automatically filters calls that are too close together.</para>
        /// </summary>
        /// <param name="section">Name of the section to track. Defaults to 'general'.</param>
        public void Track(string section = DefaultSection)
        {
            var now = DateTime.Now;
            if (now - lastRequest <= TimeSpan.FromSeconds(2))
            {
                return;
            }

            lastRequest = now;

            var netmetrixUri = new Uri(BaseDomain, string.Format("/cgi-bin/ivw/CP/apps/{0}/windowsphone/phone/{1}", NetMetrix.AppId, section));
            ThreadPool.QueueUserWorkItem(i => BeginRequest(netmetrixUri));
        }

        private IAsyncResult BeginRequest(Uri uri, CookieContainer cookies = null)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = "Mozilla/4.0 (compatible; Windows Phone OS; Windowsphone-phone)";
            request.CookieContainer = cookies ?? new CookieContainer();
            request.AllowAutoRedirect = false;

            // The cookie container in WP7 is fucked up, so add cookies manually to the headers
            // http://matthiasshapiro.com/2012/01/02/adding-cookies-to-a-windows-phone-7-httpwebrequest/
            string cookie = CookieStore.Load(CookieStorageKey);
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                request.Headers["Cookie"] = cookie;
            }

            return request.BeginGetResponse(HandleResponse, request);
        }

        private bool RedirectHandled(HttpWebResponse response, CookieContainer cookies)
        {
            int statusCode = (int)response.StatusCode;

            if (statusCode >= 300 && statusCode < 400)
            {
                Uri newLocation;
                var location = response.Headers["Location"];

                if (!string.IsNullOrWhiteSpace(location) && Uri.TryCreate(location, UriKind.Absolute, out newLocation))
                {
                    BeginRequest(newLocation, cookies);
                    return true;
                }
            }

            return false;
        }

        private void HandleResponse(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var response = GetResponse(result);

            if (response == null) return;

            // We add cookies to a fixed cookie domain to make sure we can access them later
            // Otherwise the redirects make it hard to guess where the cookie ended up
            request.CookieContainer.Add(BaseDomain, response.Cookies);

            if (RedirectHandled(response, request.CookieContainer)) return;

            // All redirects handled, we're ready to store cookies
            var cookies = request.CookieContainer.GetCookies(BaseDomain).Cast<Cookie>();
            var cookiePairs = (from c in cookies select string.Format("{0}={1}", c.Name, c.Value)).ToArray();

            if (cookiePairs.Any())
            {
                CookieStore.Save(CookieStorageKey, string.Join("; ", cookiePairs));
            }
        }

        private HttpWebResponse GetResponse(IAsyncResult result)
        {
            try
            {
                var request = (HttpWebRequest) result.AsyncState;
                return (HttpWebResponse) request.EndGetResponse(result);
            }
            catch (WebException)
            {
                return null;
            }
        }

    }

}
