using Starcounter;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PolyjuiceNamespace;

namespace LauncherNamespace {

    public static class LauncherHelper {

        /// <summary>
        /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
        /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
        /// </summary>
        public static void Init() {

            JuicyTiles.JuicyTilesSetupHandlers.Setup();

            Handle.AddFilterToMiddleware((Request req) => {

                String uri = req.Uri;

                // Checking if we should process this request.
                if ("/" == uri ||
                    Handle.IsSystemHandlerUri(uri) ||
                    uri.StartsWith("/" + StarcounterEnvironment.AppName, StringComparison.InvariantCultureIgnoreCase)) {

                    return null;
                }

                // Getting application name.
                Int32 splitPoint = uri.Length;
                for (Int32 i = 2; i < uri.Length; i++) {
                    if (uri[i] == '/') {
                        splitPoint = i;
                        break;
                    }
                }

                String appName = uri.Substring(1, splitPoint - 1);

                return WrapInLauncher(req, appName);
            });

            Handle.GET("/launcher", () => {

                Launcher launcher;

                if (Session.Current == null) {

                    launcher = new Launcher() {
                        Html = "/Launcher/LauncherTemplate.html"
                    };

                    launcher.Session = new Session(SessionOptions.PatchVersioning);

                    launcher.launchpad.icons = Self.GET<Json>("/polyjuice/app-icon", () => {
                        var p = new Page();
                        return p;
                    });

                    launcher.launchpad.names = Self.GET<Json>("/polyjuice/app-name", () => {
                        var p = new Page();
                        return p;
                    });

                    launcher.menu = Self.GET<Json>("/polyjuice/menu", () => {
                        var p = new Page() {
                            Html = "/Launcher/LauncherMenu.html"
                        };
                        return p;
                    });

                    launcher.user = Self.GET("/polyjuice/user", () => {
                        var p = new Page();
                        return p;
                    });

                    return launcher;

                } else {

                    return (Launcher) Session.Current.Data;
                }
            });

            Handle.GET("/launcher/dashboard", () => {

                Launcher launcher = Self.GET<Launcher>("/launcher");

                launcher.results = Self.GET<Json>("/polyjuice/dashboard", () => {
                    var p = new Page();
                    return p;
                });

                return launcher;
            });

            Handle.GET("/launcher/search?query={?}", (string query) => {
                Launcher launcher = Self.GET<Launcher>("/launcher");

                string uri = "/polyjuice/search?query=" + HttpUtility.UrlEncode(query);

                launcher.results = Self.GET<Json>(uri, () => {
                    var p = new Page();
                    return p;
                });

                launcher.searchBar.query = query;

                return launcher;
            });
            // + dummy responses from launcher itself  
            // Merges HTML partials according to provided URLs.
            Handle.GET(StarcounterConstants.PolyjuiceHtmlMergerPrefix + "{?}", (String s) =>
            {

                StringBuilder sb = new StringBuilder();

                String[] allPartialInfos = s.Split(new char[] { '&' });

                foreach (String appNamePlusPartialUrl in allPartialInfos)
                {

                    String[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                    if (String.IsNullOrEmpty(a[1]))
                        continue;

                    Response resp = Self.GET(a[1]);
                    sb.Append("<imported-template-scope scope=\"{{" + a[0] + "}}\">");
                    sb.Append("<template><juicy-tile-group name=\"" + a[0] + "\"></juicy-tile-group></template>");
                    sb.Append(resp.Body);
                    sb.Append("</imported-template-scope>");
                }

                return sb.ToString();
            }, new HandlerOptions()
            {
                ProxyDelegateTrigger = true,
                AllowNonPolyjuiceHandler = true,
                ReplaceExistingDelegate = true
            });

            PolyjuiceNamespace.Polyjuice.Map("/launcher", "/");
        }

        static Response WrapInLauncher(Request req, String appName) {
            Launcher launcher = Self.GET<Launcher>("/launcher");

            // Call proxed request
            Response resp = Self.CallUsingExternalRequest(req, () =>
            {
                // check if there is already workspaces array item for given appname
                Json foundWorkspace = null;
                for (var i = 0; i < launcher.workspaces.Count; i++)
                {
                    if ((launcher.workspaces[i] as LauncherPage).appName.ToLower() == appName.ToLower())
                    {
                        foundWorkspace = launcher.workspaces[i];
                        break;
                    }
                }

                // if not create new LauncherPage for this appname
                if (foundWorkspace == null)
                {
                    var p = new LauncherPage();
                    // move serializer magic to here:
                    // set partial ID, find juicy tiles, build HTML path, set appname, etc.
                    // p.appName = mainApp.AppName;
                    p.appName = appName;
                    // p.partialId = mainApp.Html;
                    // p.juicyTilesSetup = Self.GET("/launcher/juicytilessetup?" + p.partialId);
                    // p.listOfAppsRunning = appnames;

                    // and add it to the array
                    launcher.workspaces.Add(p);
                    foundWorkspace = p;
                }
                return foundWorkspace;
            });

            return launcher;
        }

    }
}
