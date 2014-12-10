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

        Polyjuice.GlobalTypesList = new List<Polyjuice.SoType>();

        Polyjuice.SoType entity = new Polyjuice.SoType() {
            Inherits = null,
            Name = "entity",
            Handlers = new List<Polyjuice.HandlerForSoType>()
        };
        Polyjuice.GlobalTypesList.Add(entity);

        Polyjuice.SoType physicalobject = new Polyjuice.SoType() {
            Inherits = entity,
            Name = "physicalobject",
            Handlers = new List<Polyjuice.HandlerForSoType>()
        };
        Polyjuice.GlobalTypesList.Add(physicalobject);

        Polyjuice.SoType product = new Polyjuice.SoType() {
            Inherits = physicalobject,
            Name = "product",
            Handlers = new List<Polyjuice.HandlerForSoType>()
        };
        Polyjuice.GlobalTypesList.Add(product);

        Polyjuice.SoType vertebrate = new Polyjuice.SoType() {
            Inherits = physicalobject,
            Name = "vertebrate",
            Handlers = new List<Polyjuice.HandlerForSoType>()
        };
        Polyjuice.GlobalTypesList.Add(vertebrate);

        Polyjuice.SoType human = new Polyjuice.SoType() {
            Inherits = vertebrate,
            Name = "human",
            Handlers = new List<Polyjuice.HandlerForSoType>()
        };
        Polyjuice.GlobalTypesList.Add(human);

        Polyjuice.SoType person = new Polyjuice.SoType() {
            Inherits = vertebrate,
            Name = "person",
            Handlers = new List<Polyjuice.HandlerForSoType>()
        };
        Polyjuice.GlobalTypesList.Add(person);

        Polyjuice.SoType organization = new Polyjuice.SoType() {
            Inherits = entity,
            Name = "organization",
            Handlers = new List<Polyjuice.HandlerForSoType>()
        };
        Polyjuice.GlobalTypesList.Add(organization);
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
                launcher = (Launcher)Session.Current.Data;
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
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/launcher/menu", out menuResp, null, 0, HandlerOptions.ApplicationLevel);

            if (menuResp != null) {
                launcher.menu = menuResp;
            }

            // X.GET("/launchpad", out resp); // thumbnails only

            Response userResp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/launcher/user", out userResp, null, 0, HandlerOptions.ApplicationLevel);

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

        Handle.GET("/dashboard", () =>
        {
            Launcher launcher = X.GET<Launcher>("/launcher");

            Response resp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/launcher/dashboard", out resp, null, 0, HandlerOptions.ApplicationLevel);
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

            string uri = "/launcher/search?query=" + HttpUtility.UrlEncode(query);
            Response resp;
            X.Forget(uri);
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
        string uri = "/launcher/search?query=" + HttpUtility.UrlEncode(query.Value);
        X.Forget(uri);
        X.GET(uri, out resp, null, 0, HandlerOptions.ApplicationLevel);
        searchEngineResultPageUrl = uri;
        ((Launcher)this.Parent).results = resp;
    }
}
