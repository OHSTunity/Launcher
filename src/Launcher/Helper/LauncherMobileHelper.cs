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
        public static void Init()
        {
            Application application = Application.Current;


            Handle.GET("/launcher/mobile", (Request req) =>
            {
                var session = Session.Current;
                LauncherMobilePage launcher;

                if (session != null && session.Data != null)
                {
                    launcher = (LauncherMobilePage)Session.Current.Data;
                    launcher.uri = req.Uri;
                    MarkWorkspacesInactive(launcher.workspaces);

                    if (session.PublicViewModel != launcher)
                        session.PublicViewModel = launcher;

                    return launcher;
                }

                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                launcher = new LauncherMobilePage()
                {
                };

                launcher.Session = session;

                launcher.user = Self.GET(UriMapping.MappingUriPrefix + "/mobile/user", () =>
                {
                    var p = new Page();
                    return p;
                });

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

            Handle.GET("/launcher/mobile/dashboard", (Request req) =>
            {
                return new Page();
            });
            UriMapping.Map("/launcher/mobile/dashboard", UriMapping.MappingUriPrefix + "/mobile/dashboard");

        }

        static void MarkWorkspacesInactive(Arr<LayoutInfo> workspaces) {
            foreach (var layoutInfo in workspaces) {
                layoutInfo.ActiveWorkspace = false;
                layoutInfo.AutoRefreshBoundProperties = false;
            }
        }

        public static Response WrapInMobileWorkspace(Request req, Json resource)
        {
            LauncherMobilePage launcher = Self.GET<LauncherMobilePage>("/launcher/mobile");
            launcher.uri = req.Uri;

            // First check if a workspace already exists for the app that registered the uri.
            string appName = req.HandlerAppName;
            LayoutInfo workspace = launcher.workspaces
                .OfType<LayoutInfo>()
                .FirstOrDefault(ws => ws.AppName.Equals(appName, StringComparison.InvariantCultureIgnoreCase));

            if (workspace == null)
            {
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
    }
}
