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

[Launcher_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Launcher : Page {

    static String[] RootHtml = File.ReadAllText(Starcounter.Application.Current.WorkingDirectory + "\\LauncherTemplate.html").Split(new String[] { "@AppsHtml@" }, StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {

        // Dashboard
        Handle.GET("/", () =>
        {

            var launcher = new Launcher()
            {
                Html = "/LauncherTemplate.html"
            };
            Response resp;
            // It would be nice to call "/" on other apps, except this one, to prevent infinite loop
            // X.GET("/" + query.Value, out resp);
            // Functional bricks
            X.GET("/dashboard", out resp);


            // X.GET("/launchpad", out resp); // thumbnails only
            launcher.results = resp;

            launcher.Session = new Session();
            return launcher;
        });




        // Not actually a mergerer anymore but linker of sibling Json parts.
        Handle.MergeResponses((Request req, List<Response> responses) =>
        {
            for (Int32 i = 1; i < responses.Count; i++) {
                ((Json) responses[i].Resource).AppName = responses[i].AppName;
                ((Json) responses[0].Resource).JsonSiblings.Add((Json) responses[i].Resource);
            }

            ((Json) responses[0].Resource).AppName = responses[0].AppName;

            return responses[0];
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

        Handle.GET("/launcher/person/123", (Request req) =>
        {
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

        Handle.GET("/launcher/search?query={?}", (string query) =>
        {
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


        Handle.GET("/menu", () =>
        {
            Response resp;
            X.GET("/launcher", out resp);

            return resp;
        });
        // + dummy responses from launcher itself
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



