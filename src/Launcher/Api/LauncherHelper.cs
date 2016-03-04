using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Starcounter;
using Starcounter.Internal;
using Starcounter.Extensions;
using Starcounter.Advanced.XSON;
using System.Reflection;
using Starcounter.Internal.XSON;

namespace Launcher {

    public static class LauncherHelper {

        /// <summary>
        /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
        /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
        /// </summary>
        public static void Init() {
            Application application = Application.Current;
            
            Layout.Register();

            JsonResponseMerger.RegisterMergeCallback(OnJsonMerge);

            application.Use((Request req) => {
                string uri = req.Uri;

                // Checking if we should process this request.
                if (("/" == uri) || (uri.StartsWith("/launcher/", StringComparison.InvariantCultureIgnoreCase)) || (uri.Equals("/launcher", StringComparison.InvariantCultureIgnoreCase))) {
                    return null;
                }
                return WrapInLauncher(req);
            });

            Handle.GET("/launcher", (Request req) => {
                var session = Session.Current;
                LauncherPage launcher;

                if (session != null && session.Data != null) {
                    launcher = (LauncherPage)Session.Current.Data;
                    launcher.uri = req.Uri;
                    return launcher;
                }

                if (session == null) {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                launcher = new LauncherPage() {
                    Html = "/Launcher/viewmodels/LauncherTemplate.html"
                };

                launcher.Session = session;

                launcher.launchpad.names = Self.GET<Json>(UriMapping.MappingUriPrefix + "/app-name", () => {
                    var p = new Page();
                    return p;
                });
                var setup = Layout.GetSetup("/launcher/launchpad");

                if (setup == null) {
                    // launcher.launchpad.layout = null
                    // workaround for https://github.com/Starcounter/Starcounter/issues/3072
                    // set default value 
                    // consider moving to HTML, or pre-populatind default layouts
                    //launcher.launchpad.layout = new Json("{\"width\": \"1000\", \"items\":[]}");
                    launcher.launchpad.layout = "{\"width\": \"1000\", \"items\":[]}";
                } else {
//                    dynamic setupJson = new Json(setup.Value);
                    launcher.launchpad.layout = setup.Value; //setupJson;
                }

                launcher.user = Self.GET(UriMapping.MappingUriPrefix + "/user", () => {
                    var p = new Page();
                    return p;
                });

                launcher.menu = Self.GET<Json>(UriMapping.MappingUriPrefix + "/menu", () => {
                    var p = new Page() {
                        Html = "/Launcher/viewmodels/LauncherMenu.html"
                    };
                    return p;
                });

                launcher.uri = req.Uri;
                return launcher;
            });

            Handle.GET("/launcher/dashboard", (Request req) => {

                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

                launcher.currentPage = Self.GET(UriMapping.MappingUriPrefix + "/dashboard", () => {
                    var p = new Page();

                    return p;
                });

                launcher.uri = req.Uri;
                return launcher;
            });

            Handle.GET("/launcher/settings", (Request req) => {

                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

                launcher.currentPage = Self.GET<SettingsPage>(UriMapping.MappingUriPrefix + "/settings", () => {
                    var p = new SettingsPage() {
                        Html = "/Launcher/viewmodels/SettingsPage.html"

                    };
                    return p;
                });

                launcher.uri = req.Uri;
                return launcher;
            });

            Handle.GET("/launcher/search?query={?}", (Request req, string query) => {
                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

                string uri = UriMapping.MappingUriPrefix + "/search?query=" + HttpUtility.UrlEncode(query);

                launcher.currentPage = Self.GET<ResultPage>(uri, () => {
                    var p = new ResultPage() {
                        Html = "/Launcher/viewmodels/ResultPage.html"
                    };
                    return p;
                });

                launcher.uri = req.Uri;
                launcher.searchBar.query = query;

                return launcher;
            });

            // + dummy responses from launcher itself
            // Merges HTML partials according to provided URLs.
            Handle.GET(StarcounterConstants.HtmlMergerPrefix + "{?}", (String s) => {

                StringBuilder sb = new StringBuilder();

                String[] allPartialInfos = s.Split(new char[] { '&' });

                foreach (String appNamePlusPartialUrl in allPartialInfos) {

                    String[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                    if (String.IsNullOrEmpty(a[1]))
                        continue;

                    Response resp = Self.GET(a[1]);
                    sb.Append("<imported-template-scope scope=\"" + a[0] + "\">");
                    sb.Append("<template><juicy-tile-group name=\"" + a[0] + "\"></juicy-tile-group></template>");
                    sb.Append(resp.Body);
                    sb.Append("</imported-template-scope>");
                }

                return sb.ToString();
            }, new HandlerOptions() {
                SkipHandlersPolicy = true,
                ReplaceExistingHandler = true
            });
        }

        static Response WrapInLauncher(Request req) {
            LauncherPage launcher = Self.GET<LauncherPage>("/launcher");
            launcher.uri = req.Uri;

            // Call proxied request
            Response resp = Self.CallUsingExternalRequest(req, () => { return new LauncherWrapperPage(); });
            var page = resp.Resource as LauncherWrapperPage;
            bool createWorkspace = true;
            for (var i = 0; i < launcher.workspaces.Count; i++) {
                if ((launcher.workspaces[i] as LauncherWrapperPage).AppName.ToLower() == page.AppName.ToLower()) {
                    createWorkspace = false;
                    break;
                }
            }

            if (createWorkspace)
                launcher.workspaces.Add(page);

            return launcher;
        }

        static Json OnJsonMerge(Request request, string callingAppName, IEnumerable<Json> partialJsons) {
            bool returnNewSibling = false;
            LayoutInfo layoutInfo = null;
            Layout layout;
            string html = null;
            string partialUrl;
            var publicViewModel = (Session.Current != null) ? Session.Current.PublicViewModel : null;

            // TODO:
            // https://github.com/Starcounter/Starcounter/issues/3118 needs to be solved.
            
            // First look for any responses already added. The same set of siblings can be merged
            // several times due to different URI's are on the same level in the viewmodel.
            foreach (Json partialJson in partialJsons) {
                if (partialJson == publicViewModel)
                    return null;

                if (partialJson is LauncherWrapperPage) {
                    // Some special handling of empty page created from a substituehandler. 
                    // Needed to be able to create or reuse a workspace for the correct app.
                    ((LauncherWrapperPage)partialJson).AppName = callingAppName;
                } else if (partialJson is LayoutInfo) {
                    layoutInfo = (LayoutInfo)partialJson; // Reusing existing instance
                    layoutInfo.AppsResponded.Clear(); 
                }
            }

            if (layoutInfo == null) {
                returnNewSibling = true;
                layoutInfo = new LayoutInfo();
            }

            foreach (Json partialJson in partialJsons) {
                var appItem = layoutInfo.AppsResponded.Add();
                appItem.StringValue = partialJson.GetAppName();

                partialUrl = partialJson.GetHtmlPartialUrl();
                if (!string.IsNullOrEmpty(partialUrl)) {
                    if (html != null)
                        html += "&";
                    html += partialJson.GetAppName() + "=" + partialUrl;

                    if (callingAppName.Equals(partialJson.GetAppName(), StringComparison.CurrentCultureIgnoreCase)) {
                        layoutInfo.PartialId = partialUrl;
                    }
                }
            }
            
            layoutInfo.AppName = callingAppName;
            layoutInfo.Path = request.Uri;
            if (!string.IsNullOrEmpty(html))
                layoutInfo.MergedHtml = StarcounterConstants.HtmlMergerPrefix + html;

            layout = Db.SQL<Layout>("SELECT l FROM Starcounter.Layout l WHERE l.Key=?", layoutInfo.PartialId).First;
            if (layout != null)
                layoutInfo.Layout = layout.Value;

            if (returnNewSibling)
                return layoutInfo;

            // We used an existing instance. We dont want to add it as a sibling again.
            return null;
        }
    }
}
