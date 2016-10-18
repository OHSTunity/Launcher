using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Starcounter;
using Starcounter.Advanced.XSON;
using Starcounter.Internal;
using Colab.Common;

namespace Launcher.Helper {

    public static class LauncherMobileHelper {
        private static int ConvertOrDefault(String str, int defaulvalue)
        {
            int res;
            if (!Int32.TryParse(str, out res))
                res = defaulvalue;
            return res;
        }

        /// <summary>
        /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
        /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
        /// </summary>
        public static void Init() {
            Application application = Application.Current;


            Handle.GET("/launcher/mobile", (Request req) => {
                var session = Session.Current;
                LauncherMobilePage launcher;

                if (session != null && session.Data != null) {
                    launcher = (LauncherMobilePage)Session.Current.Data;
                    launcher.uri = req.Uri;
                    MarkWorkspacesInactive(launcher.workspaces);

                    if (session.PublicViewModel != launcher)
                        session.PublicViewModel = launcher;

                    return launcher;
                }

                if (session == null) {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                launcher = new LauncherMobilePage() {
                };

                launcher.Session = session;

                launcher.launchpad.names = Self.GET<Json>(UriMapping.MappingUriPrefix + "/app-name", () => {
                    var p = new Page();
                    return p;
                });
                // var setup = Starcounter.HTMLComposition.GetUsingKey("/launcher/launchpad");
                var setup = Starcounter.Layout.GetSetup("/launcher/launchpad");

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

                launcher.user = Self.GET(UriMapping.MappingUriPrefix + "/mobile/user", () => {
                    var p = new Page();
                    return p;
                });
                
                /*
                launcher.menu = Self.GET<Json>(UriMapping.MappingUriPrefix + "/menu", () => {
                    var p = new Page() {
                        Html = "/Launcher/viewmodels/LauncherMenu.html"
                    };
                    return p;
                });

                launcher.sidebar = Self.GET(UriMapping.MappingUriPrefix + "/sidebar", () =>
                {
                    var p = new Page();
                    return p;
                });
                */

                launcher.contextbar = Self.GET(UriMapping.MappingUriPrefix + "/mobile/contextbar", () =>
                {
                    var p = new Page();
                    return p;
                });

                launcher.uri = req.Uri;
                MarkWorkspacesInactive(launcher.workspaces);

                if (session.PublicViewModel != launcher)
                    session.PublicViewModel = launcher;

                return launcher;
            });

            Handle.GET("/launcher/mobile/dashboard", (Request req) => {

                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

                launcher.currentPage = Self.GET(UriMapping.MappingUriPrefix + "/dashboard", () => {
                    var p = new Page();

                    return p;
                });

                launcher.uri = req.Uri;
                return launcher;
            });

            Handle.GET("/launcher/mobile/settings", (Request req) => {
                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");
                if (!(launcher.currentPage is SettingsPage))
                {
                    launcher.currentPage = new SettingsPage()
                    {
                        AppMenu = Self.GET(UriMapping.MappingUriPrefix + "/settings_menu", () =>
                        {
                            return new Page();
                        })
                    };
                }
                launcher.uri = req.Uri;
                return launcher;
            });

            Handle.GET("/launcher/mobile/search?query={?}&count={?}", (Request req, string query, string count) => {
                LauncherPage launcher = Self.GET<LauncherPage>("/launcher");

               // string uri = UriMapping.MappingUriPrefix + "/search?query=" + HttpUtility.UrlEncode(query);
                if (!(launcher.currentPage is AdvancedSearchPage))
                {
                    launcher.currentPage = new AdvancedSearchPage();
                }
                var asp = launcher.currentPage as AdvancedSearchPage;
                asp.Query = query;
                asp.Count = ConvertOrDefault(count, 10);
                asp.StartAt = ConvertOrDefault(count, 0);
                

                launcher.uri = req.Uri;
                launcher.searchBar.query = query;

                return null;
            });

            // + dummy responses from launcher itself
            // Merges HTML partials according to provided URLs.
            Handle.GET(StarcounterConstants.HtmlMergerPrefix + "{?}", (string s) => {

                StringBuilder sb = new StringBuilder();

                string[] allPartialInfos = s.Split(new char[] { '&' });

                foreach (string appNamePlusPartialUrl in allPartialInfos) {

                    string[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                    if (a.Length < 2 || string.IsNullOrEmpty(a[1]))
                        continue;

                    Response resp = Self.GET(HttpUtility.UrlDecode(a[1]));

                    sb.Append("<imported-template-scope scope=\"" + a[0] + "\">");
                    sb.Append("<template><juicy-tile-group name=\"" + a[0] + "\"></juicy-tile-group></template>");

                    if (resp != null) {
                        sb.Append(resp.Body);
                    } else {
                        sb.Append("<template></template>");
                    }
                    sb.Append("</imported-template-scope>");
                }

                string html = sb.ToString();

                if (string.IsNullOrEmpty(html)) {
                    html = "<template></template>";
                }

                return html;
            }, new HandlerOptions() {
                SkipHandlersPolicy = true,
                ReplaceExistingHandler = true
            });
        }

        static void MarkWorkspacesInactive(Arr<LayoutInfo> workspaces) {
            foreach (var layoutInfo in workspaces) {
                layoutInfo.ActiveWorkspace = false;
                layoutInfo.AutoRefreshBoundProperties = false;
            }
        }

        public static Response WrapInMobileWorkspace(Request req, Json resource) {
            LauncherMobilePage launcher = Self.GET<LauncherMobilePage>("/launcher/mobile");
            launcher.uri = req.Uri;

            // First check if a workspace already exists for the app that registered the uri.
            string appName = req.HandlerAppName;
            LayoutInfo workspace = launcher.workspaces
                .OfType<LayoutInfo>()
                .FirstOrDefault(ws => ws.AppName.Equals(appName, StringComparison.InvariantCultureIgnoreCase));

            if (workspace == null) {
                workspace = new LayoutInfo() { AppName = appName };
                launcher.workspaces.Add(workspace);
            }
            workspace.ActiveWorkspace = true;
            workspace.AutoRefreshBoundProperties = true;

            //Colab specific Context handlers
            if (resource is IContextApp && !String.IsNullOrEmpty((resource as IContextApp).ContextId))
            {
                ContextHandler.SetContext((resource as IContextApp).ContextId, appName);
            }
            else
            {
                ContextHandler.SetNonContext(req.Uri); //Always save uri for last call without context
            }

            // Doing a manual merge of the workspace and the resource from the response to attach the
            // resource to the workspace.
            workspace.MergeJson(resource);

            return launcher;
        }

        static Json OnJsonMerge(Request request, string callingAppName, IEnumerable<Json> partialJsons) {
            bool returnNewSibling = false;
            LayoutInfo layoutInfo = null;
            Starcounter.HTMLComposition composition;
            string html = null;
            string partialUrl;
            string layoutNamespace = "LauncherLayoutInfo";
            var publicViewModel = (Session.Current != null) ? Session.Current.PublicViewModel : null;
            
            // First look for any responses already added. The same set of siblings can be merged
            // several times due to different URI's are on the same level in the viewmodel.
            foreach (Json partialJson in partialJsons) {
                if (partialJson == publicViewModel)
                    return null;

                if (partialJson is LayoutInfo && partialJson.GetAppName() == layoutNamespace) {
                    if (layoutInfo == null) {
                        layoutInfo = (LayoutInfo)partialJson; // Reusing first existing instance
                        layoutInfo.AppsResponded.Clear();
                    } else {
                        // Workaround for merged siblings containing more than one sibling
                        // from the same app.
                        ((LayoutInfo)partialJson).ActiveWorkspace = layoutInfo.ActiveWorkspace;
                    }
                }
            }

            if (layoutInfo == null)
            {
                returnNewSibling = true;
                StarcounterEnvironment.RunWithinApplication(layoutNamespace, () => {
                    layoutInfo = new LayoutInfo();
                });
            }

            foreach (Json partialJson in partialJsons) {
                var appItem = layoutInfo.AppsResponded.Add();
                appItem.StringValue = partialJson.GetAppName();

                if (partialJson.Template == null) {
                    continue;
                }

                partialUrl = partialJson["Html"] as string;
                if (!string.IsNullOrEmpty(partialUrl)) {
                    if (html != null)
                        html += "&";
                    html += partialJson.GetAppName() + "=" + partialUrl;
                }
            }
            
            layoutInfo.AppName = callingAppName;
            layoutInfo.Path = request.Uri;
            if (!string.IsNullOrEmpty(html))
            {
                layoutInfo.MergedHtml = StarcounterConstants.HtmlMergerPrefix + html;
                layoutInfo.PartialId = layoutInfo.MergedHtml;
            } 

            composition = Starcounter.HTMLComposition.GetUsingKey(layoutInfo.PartialId);
            if (composition != null)
                layoutInfo.Composition = composition.Value;

            if (returnNewSibling)
                return layoutInfo;

            // We used an existing instance. We dont want to add it as a sibling again.
            return null;
        }
    }
}
