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

[Launcher_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Launcher : Page {

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {

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
                launcher = (Launcher)Session.Current.Data;
            }

            Response icons;
            X.GET("/app-icon", out icons); // thumbnails only
            if (icons != null) {
                launcher.launchpad.icons = icons;
            }

            Response names;
            X.GET("/app-name", out names);
            if (names != null) {
                launcher.launchpad.names = names;
            }

            Response menuResp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/menu", out menuResp);

            if (menuResp != null) {
                launcher.menu = menuResp;
            }

            // X.GET("/launchpad", out resp); // thumbnails only

            Response userResp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/user", out userResp);

            if (userResp != null) {
                launcher.user = userResp;
            }

            return launcher;
        });

        // Not actually a merger anymore but linker of sibling Json parts.
        Handle.MergeResponses((Request req, List<Response> responses) => {
            // Handle "/"-> "/index" request in special way, so app may replace Launcher's response
            if (req.Uri == "/index")
            {
                // responses.Sort( by Priority )
                return responses[0];
            }


            var mainResponse = responses[0];
            var json = mainResponse.Resource as Json;

            // Merge every JSON that is not response from Launcher itself
            // TODO: change it to somethind nicer than hardcoded "Launcher" name (tomalec)
            // if (json != null && mainResponse.AppName != this.AppName)
            // if (mainResponse.isMergable)
            if (json != null && mainResponse.AppName != "Launcher")
            {
                json.SetAppName(mainResponse.AppName);

                for (Int32 i = 1; i < responses.Count; i++)
                {
                    ((Json)responses[i].Resource).SetAppName(responses[i].AppName);
                    json.AddStepSibling((Json)responses[i].Resource);
                }
            }
            return mainResponse;
        });

        Handle.GET("/launcher/dashboard", () =>
        {
            Launcher launcher = X.GET<Launcher>("/launcher");

            Response resp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/dashboard", out resp);
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
                X.GET(a[1], out resp);
                sb.Append("<launchpad-tile appname=\""+ a[0] +"\">");
                sb.Append(resp.Body);
                sb.Append("</launchpad-tile>");
            }

            return sb.ToString();
        });

        Handle.GET("/launcher/search?query={?}", (string query) => {
            Launcher launcher = X.GET<Launcher>("/launcher");

            Response resp;
            X.GET("/search?query=" + query, out resp);
            if (resp != null) {
                launcher.results = resp;
            }

            return launcher;
        });
        // + dummy responses from launcher itself  
        
        // Launcher's entries for the menu
        // does not get merged :(
        // Handle.GET("/menu", () =>
        // {
        //     var p = new Page()
        //     {
        //         Html = "/LauncherMenu.html"
        //     };
        //     return p;
        // }, HandlerOptions.ApplicationLevel);

        // Disabling registration in gateway.
        HandlerOptions.DefaultHandlerOptions.HandlerLevel = HandlerOptions.HandlerLevels.ApplicationLevel;
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
                launcher.focusedWorkspace = i;
                break;
            }
        }

        if (foundWorkspace == null)
        {
            foundWorkspace = (Launcher.workspacesElementJson)launcher.workspaces.Add();
            foundWorkspace.appName = appName;
            launcher.focusedWorkspace = launcher.workspaces.Count - 1;
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
        X.GET("/search?query=" + HttpUtility.UrlEncode(query.Value), out resp);
        searchEngineResultPageUrl = "/launcher/search?query=" + HttpUtility.UrlEncode(query.Value);
        ((Launcher)this.Parent).results = resp;
    }
}
