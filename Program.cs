using Polyjuice;
using Starcounter;
using System;
using System.Text;

namespace LauncherGui {
    public class Program {

        public static void Main() {

            // Merges HTML partials according to provided URLs.
            Handle.GET("/launcher/launchpad/polyjuice/htmlmerger?{?}", (String s) => {
                StringBuilder sb = new StringBuilder();

                String[] allPartialInfos = s.Split(new char[] { '&' });

                foreach (String appNamePlusPartialUrl in allPartialInfos) {
                    String[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                    Response resp;

                    if (string.IsNullOrEmpty(a[1]))
                        continue;

                    X.GET(a[1], out resp);
                    sb.Append("<launchpad-tile appname=\"" + a[0] + "\">");
                    sb.Append(resp.Body);
                    sb.Append("</launchpad-tile>");
                }

                return sb.ToString();
            });

            LauncherHelper.Init();
        }

    }
}
