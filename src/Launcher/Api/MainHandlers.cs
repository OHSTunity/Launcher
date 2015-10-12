using System;
using System.Text;
using Starcounter;

namespace Launcher {

    internal class MainHandlers {
        public void Register() {
            // Merges HTML partials according to provided URLs.
            Handle.GET("/launcher/launchpad/sc/htmlmerger?{?}", (string s) => {
                StringBuilder sb = new StringBuilder();
                string[] allPartialInfos = s.Split(new char[] { '&' });

                foreach (string appNamePlusPartialUrl in allPartialInfos) {
                    string[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                    Response resp;

                    if (string.IsNullOrEmpty(a[1])) {
                        continue;
                    }

                    //Self.GET(a[1], out resp);
                    resp = Self.GET(a[1]);
                    sb.Append("<launchpad-tile appname=\"" + a[0] + "\">");
                    sb.Append(resp.Body);
                    sb.Append("</launchpad-tile>");
                }

                return sb.ToString();
            });
        }
    }
}
