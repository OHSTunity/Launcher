using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Starcounter;
using Starcounter.Internal;
using PolyjuiceNamespace;
using JuicyTiles;

namespace Launcher {

    public static class LauncherHelper {

        /// <summary>
        /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
        /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
        /// </summary>
        public static void Init() {
            JuicyTilesSetupHandlers handlers = new JuicyTilesSetupHandlers();

            handlers.Register();

            Handle.AddFilterToMiddleware((Request req) => {

                String uri = req.Uri;

                // Checking if we should process this request.
                if (("/" == uri) || (uri.StartsWith("/launcher/", StringComparison.InvariantCultureIgnoreCase)) || (uri.Equals("/launcher", StringComparison.InvariantCultureIgnoreCase))) {
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

                LauncherPage launcher;

                if (Session.Current == null) {

                    launcher = new LauncherPage() {
                        Html = "/Launcher/viewmodels/LauncherTemplate.html"
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
                    var setup = JuicyTilesSetup.GetSetup("/launcher/launchpad");

                    if (setup == null)
                    {
                        launcher.launchpad.juicyTilesSetup = null;
                    }
                    else
                    {
                        dynamic setupJson = new Json(setup.Value);
                        launcher.launchpad.juicyTilesSetup = setupJson;
                    }

                    launcher.menu = Self.GET<Json>("/polyjuice/menu", () => {
                        var p = new Page() {
                            Html = "/Launcher/viewmodels/LauncherMenu.html"
                        };
                        return p;
                    });

                    launcher.user = Self.GET("/polyjuice/user", () => {
                        var p = new Page();
                        return p;
                    });

                    return launcher;

                } else {

                    return (LauncherPage)Session.Current.Data;
                }
            });

            Handle.GET("/launcher/dashboard", () => {

                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

                launcher.results = Self.GET<LauncherResultsPage>("/polyjuice/dashboard", () => {
                    var p = new LauncherResultsPage();

                    return p;
                });

                return launcher;
            });

            Handle.GET("/launcher/search?query={?}", (string query) => {
                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

                string uri = "/polyjuice/search?query=" + HttpUtility.UrlEncode(query);

                launcher.results = Self.GET<LauncherResultsPage>(uri, () => {
                    var p = new LauncherResultsPage();
                    return p;
                });

                launcher.searchBar.query = query;

                return launcher;
            });
            // + dummy responses from launcher itself
            // Merges HTML partials according to provided URLs.
            Handle.GET(StarcounterConstants.PolyjuiceHtmlMergerPrefix + "{?}", (String s) => {

                StringBuilder sb = new StringBuilder();

                String[] allPartialInfos = s.Split(new char[] { '&' });

                foreach (String appNamePlusPartialUrl in allPartialInfos) {

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
            }, new HandlerOptions() {
                ProxyDelegateTrigger = true,
                AllowNonPolyjuiceHandler = true,
                ReplaceExistingHandler = true
            });
        }

        static Response WrapInLauncher(Request req, String appName) {
            LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

            // Call proxed request
            Response resp = Self.CallUsingExternalRequest(req, () => {
                // check if there is already workspaces array item for given appname
                Json foundWorkspace = null;
                for (var i = 0; i < launcher.workspaces.Count; i++) {
                    if ((launcher.workspaces[i] as LauncherWrapperPage).appName.ToLower() == appName.ToLower()) {
                        foundWorkspace = launcher.workspaces[i];
                        break;
                    }
                }

                // if not create new LauncherPage for this appname
                if (foundWorkspace == null) {
                    var p = new LauncherWrapperPage();
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
