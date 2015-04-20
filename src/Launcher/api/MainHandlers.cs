using System;
using System.Text;
using Polyjuice;
using Starcounter;

namespace Launcher {
    internal class MainHandlers {
        public void Register() {
            // Merges HTML partials according to provided URLs.
            Handle.GET("/launcher/launchpad/polyjuice/htmlmerger?{?}", (string s) => {
                StringBuilder sb = new StringBuilder();
                string[] allPartialInfos = s.Split(new char[] { '&' });

                foreach (string appNamePlusPartialUrl in allPartialInfos) {
                    string[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                    Response resp;

                    if (string.IsNullOrEmpty(a[1])) {
                        continue;
                    }   

                    //X.GET(a[1], out resp);
                    resp = Self.GET(a[1]);
                    sb.Append("<launchpad-tile appname=\"" + a[0] + "\">");
                    sb.Append(resp.Body);
                    sb.Append("</launchpad-tile>");
                }

                return sb.ToString();
            });

            Handle.GET("/launcher/removeallstyles/{?}", (string confirm) => {
                if (confirm == "true") {
                    Db.Transact(() => {
                        Db.SlowSQL("DELETE FROM JuicyTiles.JuicyTilesSetup");
                    });
                }
                
                return 200;
            });
        }
    }
}
