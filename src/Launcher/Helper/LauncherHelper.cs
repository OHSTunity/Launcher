using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Starcounter;
using Starcounter.Advanced.XSON;
using Starcounter.Internal;
using Colab.Common;
using WorkSpace = Launcher.WorkSpace;

namespace Launcher.Helper {

    public static class LauncherHelper {

        private static int ConvertOrDefault(String str, int defaulvalue)
        {
            int res;
            if (!Int32.TryParse(str, out res))
                res = defaulvalue;
            return res;
        }

        private static Response GetPartials(String common_uri)
        {
            return Self.GET(UriMapping.MappingUriPrefix + common_uri, () => {
                var p = new Page();
                return p;
            });
        }

        private const string WRAPINWORKSPACE = "X-WrapInWorkspace";
        
        /// <summary>
        /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
        /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
        /// </summary>
        public static void Init() {
            Application application = Application.Current;

            Starcounter.HTMLComposition.Register();

            JsonResponseMerger.RegisterMergeCallback(OnJsonMerge);
            
            application.Use((Request req) => {
                string uri = req.Uri;

                if (uri.Contains("/mobile") && !uri.Equals("/launcher/mobile", StringComparison.InvariantCultureIgnoreCase))
                {
                    req.Headers[WRAPINWORKSPACE] = "M";
                    return null;
                }
                
                // Checking if we should process this request.
                if (("/" == uri) ||
                    (uri.Equals("/launcher/main", StringComparison.InvariantCultureIgnoreCase)) ||
                    (SettingsHelper.IfBypassUrl(uri))) {
                    return null;
                }

                // Tag the request so we know in the response filter that the response from 
                // this request should be wrapped in a workspace.
                req.Headers[WRAPINWORKSPACE] = "T";
                return null;
            });

            // Response filter. If the request is tagged and the response contains json, we wrap it in a workspace.
            application.Use((Request request, Response response) => {
                if (!string.IsNullOrEmpty(request.Headers[WRAPINWORKSPACE]) && response.Resource is Json)
                    return WrapInWorkspace(request, (Json)response.Resource);

                //Colab context specific
                if (response.Resource is Json)
                    ContextHandler.SetNonContext(request.Uri); //Always save uri for last call without context

                return null;
            });
            
            Handle.GET("/launcher/main", (Request req) => {
                var session = Session.Current;
                LauncherPage launcher;

                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                if (!(session.Data is LauncherPage))
                {
                    session.Data = launcher = new LauncherPage()
                    {
                        Html = "/Launcher/viewmodels/LauncherTemplate.html"
                    };

                    launcher.Session = session;

                    launcher.user = GetPartials("/user");

                    launcher.menu = GetPartials("/menu"); 

                    /*Special colab stuff*/
                    launcher.sidepanel = GetPartials("/sidepanel");

                    launcher.contextpanel = GetPartials("/contextpanel");

                    launcher.contextbar = GetPartials("/contextbar");
                }
                else
                {
                    launcher = session.Data as LauncherPage;
                }
               
                launcher.uri = req.Uri;
                MarkWorkspacesInactive(launcher.workspaces);

                if (session.PublicViewModel != launcher)
                    session.PublicViewModel = launcher;

                return launcher;
            });

            Handle.GET("/launcher", (Request req) => {
                return RequireAuthorize(req);
            });



            Handle.GET("/launcher/mobile", (Request req) =>
            {
                var session = Session.Current;
                LauncherPage launcher;

                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                if (!(session.Data is LauncherPage))
                {
                    session.Data = launcher = new LauncherPage()
                    {
                        Html = "/Launcher/viewmodels/LauncherMobileTemplate.html"
                    };

                    launcher.Session = session;

                    launcher.user = GetPartials("/mobile/user");

                    launcher.menu = GetPartials("/mobile/menu");

                    /*Special colab stuff*/
                    launcher.contextpanel = GetPartials("/mobile/contextpanel");
                }
                else
                {
                    launcher = session.Data as LauncherPage;
                }

                launcher.uri = req.Uri;
                MarkWorkspacesInactive(launcher.workspaces);

                if (session.PublicViewModel != launcher)
                    session.PublicViewModel = launcher;

                return launcher;
            });

            Handle.GET("/launcher/mobile/dashboard", (Request req) =>
            {
                LauncherPage launcher = Self.GET<LauncherPage>("/launcher/mobile");
                return new Page();
            });

            UriMapping.Map("/launcher/mobile/dashboard", UriMapping.MappingUriPrefix + "/mobile/dashboard");


            Handle.GET("/launcher/container_dashboard", (Request req) =>
            {
                return RequireAuthorize(req, (LauncherPage lp) =>
                {
                    return new Dashboard()
                    {
                        DashboardPartials = Self.GET("/launcher/dashboard"),
                        LaunchpadPartials = Self.GET(UriMapping.MappingUriPrefix + "/app-name",
                        () =>
                        {
                            var p = new Page();
                            return p;
                        })
                    };
                });
            });
 
            Handle.GET("/launcher/dashboard", (Request req) => 
            {
                return RequireAuthorize(req, (LauncherPage lp) =>
                {

                    return new Page();

                });
            });
            UriMapping.Map("/launcher/dashboard", UriMapping.MappingUriPrefix + "/dashboard");


            Handle.GET("/launcher/signin", (Request req) =>
            {
                LauncherPage launcher = Self.GET<LauncherPage>("/launcher/main");
                return new Page();
            });
            UriMapping.Map("/launcher/signin", UriMapping.MappingUriPrefix + "/signin");
            Handle.GET("/launcher/signin?{?}", (Request req, String uri) =>
            {
                LauncherPage launcher = Self.GET<LauncherPage>("/launcher/main");
                return new Page();
            });
            UriMapping.Map("/launcher/signin?@w", UriMapping.MappingUriPrefix + "/signin/@w");

            Handle.GET("/launcher/launchpad", (Request req) => {
                return RequireAuthorize(req, (LauncherPage lp) =>
                {
                    return Self.GET(UriMapping.MappingUriPrefix + "/launchpad", () =>
                    {
                        dynamic json = new Json();
                        json.Html = "/Launcher/viewmodels/launchpad.html";
                        json.Apps = Self.GET<Json>(UriMapping.MappingUriPrefix + "/app-name", () =>
                        {
                            var p = new Page();
                            return p;
                        });
                        json.Layout = GetLaunchpadLayout();
                        return json;
                    });
                });
            });

            Handle.GET("/launcher/settings", (Request req) => {
                return RequireAuthorize(req, (LauncherPage lp) =>
                {
                    dynamic json = new Json();
                    json.Html = "/Launcher/viewmodels/settings.html";
                    json.Menu = Self.GET<Json>("/launcher/settings/menu");
                    return json;
                });
            });
            Handle.GET("/launcher/settings/menu", (Request req) =>
            {
                return new Page();
            });
            UriMapping.Map("/launcher/settings/menu", UriMapping.MappingUriPrefix + "/settings_menu");

            Handle.GET("/launcher/configs", (Request req) => {
                return RequireAuthorize(req, (LauncherPage lp) =>
                {
                    dynamic json = new Json();
                    json.Html = "/Launcher/viewmodels/configs.html";
                    json.Menu = Self.GET<Json>("/launcher/configs/menu");
                    return json;
                });
            });
            Handle.GET("/launcher/configs/menu", (Request req) =>
            {
                return new Page();
            });
            UriMapping.Map("/launcher/configs/menu", UriMapping.MappingUriPrefix + "/configs_menu");




            Handle.GET("/launcher/search", (Request req) =>
            {
                return null;
               /* return Db.Scope(() =>
                {
                    return new AdvancedSearchPage()
                    {
                    };
                });*/
            });
            
            Handle.GET("/launcher/search?query={?}&count={?}", (Request req, string query, string count) => {
                return null;
                /*return Db.Scope(() =>
                {
                    return new AdvancedSearchPage()
                    {
                        Query = query,
                    };
                });*/
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

        static string GetLaunchpadLayout()
        {
            var setup = Starcounter.Layout.GetSetup("/launcher/launchpad");
            if (setup != null)
            {
                return setup.Value;
            }
            else
            {
                return "{\"width\": \"1000\", \"items\":[]}";
            }
        }
       
        static void MarkWorkspacesInactive(Arr<WorkSpace> workspaces) {
            foreach (var layoutInfo in workspaces) {
                layoutInfo.ActiveWorkspace = false;
                layoutInfo.AutoRefreshBoundProperties = false;
            }
        }

        static dynamic RequireAuthorize(Request req, Func<LauncherPage, dynamic> func = null)
        {
            LauncherPage launcher = (LauncherPage)Self.GET("/launcher/main");
            if (launcher.CheckLogin())
            {
                if (func != null)
                    return func(launcher);
            }
            else
            {
                launcher.redirect = "/launcher/signin?originurl=" + req.Uri;
            }
            return new Page();
        }

        static Response WrapInWorkspace(Request req, Json resource) {
            LauncherPage launcher;
            if (string.Equals(req.Headers[WRAPINWORKSPACE], "M"))
                launcher = Self.GET<LauncherPage>("/launcher/mobile");
            else
                launcher = Self.GET<LauncherPage>("/launcher/main");

            launcher.uri = req.Uri;

            // First check if a workspace already exists for the app that registered the uri.
            string appName = req.HandlerAppName;
            launcher.SetTitle(appName);
            WorkSpace workspace = launcher.workspaces
                .OfType<WorkSpace>()
                .FirstOrDefault(ws => ws.AppName.Equals(appName, StringComparison.InvariantCultureIgnoreCase));

            if (workspace == null) {
                workspace = new WorkSpace() { AppName = appName };
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
            workspace.Partials.MergeJson(resource);

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
