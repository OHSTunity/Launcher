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

    static String[] RootHtml = File.ReadAllText(Starcounter.Application.Current.WorkingDirectory + "\\LauncherTemplate.html").Split(new String[] { "@AppsHtml@" }, StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {

        // Dashboard
        Handle.GET("/", () => {
            // Check if there is any App that would like to occupy "/" location.
            Response otherAppIndex;
            X.GET("/index", out otherAppIndex);
            if (otherAppIndex.Resource != null) //TODO: Provide nicer check (tomalec)
            {
                return otherAppIndex;
            }
            else // if not proceed with Launcher's one
            {
                return (Launcher)X.GET("/launcher");
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

            Response resp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/dashboard", out resp);

            // X.GET("/launchpad", out resp); // thumbnails only
            launcher.results = resp;

            Response icons;
            X.GET("/app-icon", out icons); // thumbnails only
            launcher.launchpad.icons = icons;

            Response menuResp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/menu", out menuResp);

            // X.GET("/launchpad", out resp); // thumbnails only
            launcher.menu = menuResp;

            Response userResp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/user", out userResp);

            launcher.user = userResp;

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
            Launcher launcher = (Launcher)X.GET("/");
            Response resp;
            X.GET("/app-icon", out resp);
            launcher.dashboard = resp;
            return launcher;

        });
        //do-nothing handler reproduces problem with link handling in Polyjuice Launcher
        //expected: clicking on a link should result in a Patch to the client that contains only the changed part
        //actual: for some reason, the Patch replaces the whole root path (/)
        Handle.GET("/do-nothing", () =>
        {
            Launcher launcher = (Launcher)Launcher.GET("/");
            launcher.focusedWorkspace = 99;
            return launcher;
        });

        Handle.GET("/launcher/workspace/{?}/{?}", (String appName, String uri) => {
            return WorkspaceResponse(appName, uri);
        });

        Handle.GET("/launcher/workspace/{?}", (String appName) => {
            return WorkspaceResponse(appName, null);
        });

        // Merges HTML partials according to provided URLs.
        Handle.GET("/polyjuice-merger?{?}", (String s) => {
            StringBuilder sb = new StringBuilder();

            String[] allPartialInfos = s.Split(new char[] { '&' });

            foreach (String appNamePlusPartialUrl in allPartialInfos) {
                String[] a = appNamePlusPartialUrl.Split(new char[] { '=' });
                Response resp;
                X.GET(a[1], out resp);
                sb.Append(resp.Body);
            }

            return sb.ToString();
        });

        Handle.GET("/launcher/person/123", (Request req) => {
            Response resp;
            X.GET("/person/123", out resp);

            if (Session.InitialRequest.PreferredMimeType == MimeType.Text_Html)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(RootHtml[0]);
                sb.Append(resp.GetContentString(MimeType.Text_Html));
                sb.Append(RootHtml[1]);
                resp = new Response() { Body = sb.ToString() };
            }

            if (Session.Current == null)
                Session.Current = new Session();

            return resp;                               // Return the JSON or the HTML depending on the type asked for. See Page.json on how Starcounter knowns what to return.
        });

        Handle.GET("/launcher/search?query={?}", (string query) => {
            Response resp;
            X.GET("/search?query=" + query, out resp);

            if (Session.InitialRequest.PreferredMimeType == MimeType.Text_Html)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(RootHtml[0]);
                sb.Append(resp.GetContentString(MimeType.Text_Html));
                sb.Append(RootHtml[1]);
                resp = new Response() { Body = sb.ToString() };
            }

            if (Session.Current == null)
                Session.Current = new Session();

            return resp;                               // Return the JSON or the HTML depending on the type asked for. See Page.json on how Starcounter knowns what to return.
  
        });
        // + dummy responses from launcher itself  

        // Setting default handler level to 1.
        HandlerOptions.DefaultHandlerLevel = 1;
        Handlers.AddExtraHandlerLevel();


        // Launcher's entries for the menu
        // does not get merged :(
        // HandlerOptions h1 = new HandlerOptions() { HandlerLevel = 1 };
        // Handle.GET("/menu", () =>
        // {
        //     var p = new Page()
        //     {
        //         Html = "/LauncherMenu.html"
        //     };
        //     return p;
        // }, h1);

    }

    static Response WorkspaceResponse(String appName, String uri)
    {
        Launcher launcher = (Launcher)Launcher.GET("/");
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
            foundWorkspace.master = (Json)X.GET("/" + appName );
        }
        else
        {
            foundWorkspace.master = (Json)X.GET("/" + appName + "/" + uri);
        }


        return launcher;
    }
}


[Launcher_json.searchBar]
partial class SearchBar : Json {
    void Handle(Input.query query) {
        Response resp;
        X.GET("/search?query=" + HttpUtility.UrlEncode(query.Value), out resp);
        ((Launcher)this.Parent).results = resp;
    }
}



