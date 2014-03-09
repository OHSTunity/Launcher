using Starcounter;                                  // Most stuff relating to the database, JSON and communication is in this namespace
using Starcounter.Templates;
using Starcounter.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Reflection;

[Launcher_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Launcher : Page {

    static String[] RootHtml = File.ReadAllText(Starcounter.Application.Current.WorkingDirectory + "\\LauncherTemplate.html").Split(new String[] { "@AppsHtml@" }, StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {

        Handle.GET("/", () =>
        {
            Response resp;
            X.GET("/launcher", out resp);

            return resp;
        });

        Handle.MergeResponses((Request req, List<Response> responses, List<String> appNames) =>
        {
            StringBuilder sb = new StringBuilder();

            switch (Session.InitialRequest.PreferredMimeType)
            {
                case MimeType.Text_Html:
                {
                    // Going through each response and appending it.
                    for (Int32 i = 0; i < responses.Count; i++)
                    {
                        sb.Append("<template bind=\"{{" + appNames[i] + "}}\">\n");
                        sb.Append(responses[i].GetContentString(MimeType.Text_Html));
                        sb.Append("</template>\n");
                    }

                    break;
                }

                case MimeType.Application_Json:
                {
                    Json root = new Json();
                    Int32 n = responses.Count;
                    for (Int32 i = 0; i < n; i++) {
                        root[appNames[i]] = (Json)responses[i].Hypermedia;
                    }
                    if (Session.Current != null)
                        root.Session = Session.Current;
                    else 
                        root.Session = new Session();

                    return root;
                }

                default:
                    throw new ArgumentException("Request is of unsuitable MIME type to merge responses!");
            }

            Response mergedResp = new Response()
            {
                Body = sb.ToString()
            };

            return mergedResp;
        });

        Handle.GET("/launcher", (Request req) =>
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
    }
}





