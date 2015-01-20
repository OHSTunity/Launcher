using Starcounter;                                  // Most stuff relating to the database, JSON and communication is in this namespace
using Starcounter.Templates;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Web;
using Starcounter.Advanced.XSON;
using PolyjuiceNamespace;

[Launcher_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Launcher : Page {

    /// <summary>
    /// Creates an emulated Society Objects tree.
    /// </summary>
    static void InitSocietyObjects() {

        PolyjuiceNamespace.Polyjuice.Init();
    }

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {

        InitSocietyObjects();

        JuicyTiles.JuicyTilesSetupHandlers.Setup();

        // Dashboard
        Handle.GET("/", () => {

            // Check if there is any App that would like to occupy "/" location.
            Response otherAppIndex;
            X.GET("/index", out otherAppIndex);

            if ((otherAppIndex != null) && (otherAppIndex.Resource != null)) {
                return otherAppIndex;
            } else { // if not proceed with Launcher's one
                return X.GET<Launcher>("/launcher");
            }

        });

        Handle.GET("/launcher", () =>
        {
            Launcher launcher;
            if (Session.Current == null)
            {
                launcher = new Launcher()
                {
                    Html = "/LauncherTemplate.html"
                };

                launcher.appFilter.Add(new appFilterElementJson() { appName = "Super CRM" });
                launcher.appFilter.Add(new appFilterElementJson() { appName = "Skyper" });
                launcher.appFilter.Add(new appFilterElementJson() { appName = "Noise" });
                launcher.appFilter.Add(new appFilterElementJson() { appName = "Map" });
                launcher.Session = new Session();
            }
            else
            {
               return (Launcher)Session.Current.Data;
               
            }

            Response icons;
            X.GET("/launcher/app-icon", out icons, null, 0, HandlerOptions.ApplicationLevel); // thumbnails only
            if (icons != null) {
                launcher.launchpad.icons = icons;
            }

            Response names;
            X.GET("/launcher/app-name", out names, null, 0, HandlerOptions.ApplicationLevel);
            if (names != null) {
                launcher.launchpad.names = names;
            }

            Response menuResp;
            X.GET("/launcher/menu", out menuResp, null, 0, HandlerOptions.ApplicationLevel);

            if (menuResp != null) {
                launcher.menu = menuResp;
            }

            Response userResp;
            X.GET("/launcher/user", out userResp, null, 0, HandlerOptions.ApplicationLevel);

            if (userResp != null) {
                launcher.user = userResp;
            }

            return launcher;
        });

        // Not actually a merger anymore but linker of sibling Json parts.
        Handle.MergeResponses((Request req, List<Response> responses) => {

            var mainResponse = responses[0];
            Int32 mainResponseId = 0;

            // Searching for the current application in responses.
            for (Int32 i = 0; i < responses.Count; i++) {

                if (responses[i].AppName == StarcounterEnvironment.AppName) {

                    mainResponse = responses[i];
                    mainResponseId = i;
                    break;
                }
            }

            var json = mainResponse.Resource as Json;

            if ((json != null) && (mainResponse.AppName != StarcounterConstants.LauncherAppName)) {

                json.SetAppName(mainResponse.AppName);

                for (Int32 i = 0; i < responses.Count; i++) {

                    if (mainResponseId != i) {

                        ((Json)responses[i].Resource).SetAppName(responses[i].AppName);
                        json.AddStepSibling((Json)responses[i].Resource);
                    }
                }
            }

            return mainResponse;
        });

        Handle.GET("/launcher/dashboard", () =>
        {
            Launcher launcher = X.GET<Launcher>("/launcher");

            String uri = "/launcher/dashboard";
            Response resp;

            X.GET(uri, out resp, null, 0, HandlerOptions.ApplicationLevel);
            if (resp != null) {
                launcher.results = resp;
            }

            return launcher;

        });

        Handle.GET("/launcher/workspace/{?}/{?}", (String appName, String uri) => {
            return WorkspaceResponse(appName, uri);
        });

        Handle.GET("/launcher/workspace/{?}", (String appName) => {
            return WorkspaceResponse(appName, null);
        });

        // Merges HTML partials according to provided URLs.
        Handle.GET("/polyjuice-merger?{?}", (String s) =>
        {
            StringBuilder sb = new StringBuilder();

            String[] allPartialInfos = s.Split(new char[] { '&' });

            foreach (String appNamePlusPartialUrl in allPartialInfos)
            {
                String[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                Response resp;
                X.GET(a[1], out resp);
                sb.Append(resp.Body);
            }

            return sb.ToString();
        });

        // Merges HTML partials according to provided URLs.
        Handle.GET("/launchpad/polyjuice-merger?{?}", (String s) =>
        {
            StringBuilder sb = new StringBuilder();

            String[] allPartialInfos = s.Split(new char[] { '&' });

            foreach (String appNamePlusPartialUrl in allPartialInfos)
            {
                String[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                Response resp;

                if (string.IsNullOrEmpty(a[1]))
                    continue;

                X.GET(a[1], out resp);
                sb.Append("<launchpad-tile appname=\""+ a[0] +"\">");
                sb.Append(resp.Body);
                sb.Append("</launchpad-tile>");
            }

            return sb.ToString();
        });

        Handle.GET("/launcher/search?query={?}", (string query) => {
            Launcher launcher = X.GET<Launcher>("/launcher");

            string uri = "/launcher/search?query=" + HttpUtility.UrlEncode(query);
            Response resp;

            X.GET(uri, out resp, null, 0, HandlerOptions.ApplicationLevel);
            if (resp != null) {
                launcher.results = resp;
            }

            launcher.searchBar.query = query;
            return launcher;
        });
        // + dummy responses from launcher itself  
        
        StarcounterEnvironment.PolyjuiceAppsFlag = true;
    }

    static Response WorkspaceResponse(String appName, String uri)
    {
        Launcher launcher = X.GET<Launcher>("/launcher");
        Launcher.workspacesElementJson foundWorkspace = null;
        for (var i = 0; i < launcher.workspaces.Count; i++)
        {
            if (appName == launcher.workspaces[i].appName)
            {
                foundWorkspace = (Launcher.workspacesElementJson)launcher.workspaces[i];
                break;
            }
        }

        if (foundWorkspace == null)
        {
            foundWorkspace = (Launcher.workspacesElementJson)launcher.workspaces.Add();
            foundWorkspace.appName = appName;
        }
        if (uri == null)
        {
            foundWorkspace.master = X.GET<Json>("/" + appName );
        }
        else
        {
            foundWorkspace.master = X.GET<Json>("/" + appName + "/" + uri);
        }


        return launcher;
    }
}

[Launcher_json.searchBar]
partial class SearchBar : Json {
    void Handle(Input.query query) {
        Response resp;
        string uri = "/launcher/search?query=" + HttpUtility.UrlEncode(query.Value);

        X.GET(uri, out resp, null, 0, HandlerOptions.ApplicationLevel);
        searchEngineResultPageUrl = uri;
        if (resp != null) {
            ((Launcher)this.Parent).results = resp;
        }
    }
}
