using Starcounter;                                  // Most stuff relating to the database, JSON and communication is in this namespace
using Starcounter.Templates;
using System;
using System.Collections.Generic;

[Launcher_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Launcher : Page {

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {


        Handle.GET("/", () =>
        {
            return Launcher.GET("/launcher");
        });

        Handle.GET("/launcher", () =>
        {
            Launcher m = new Launcher()
            {                                       // This is the root view model for our application. A view model is a JSON object/tree.
                Html = "/launcher.html",              // This is just a field we added to allow the client to know what Html to load. No magic.
            };
            m.Session = new Session();              // Save the JSON on the server to be accessed using GET/PATCH to allow it to be used as a view model in web components.


            var j = (LauncherWorkspace)X.GET("/whoareyou_combined"); // We are cheating as multiple handlers are not allowed and there is no merger code to merge them together anyways.


            m.workspace = j;

            return m;                               // Return the JSON or the HTML depending on the type asked for. See Page.json on how Starcounter knowns what to return.
        });



        Handle.GET("/whoareyou1", () =>
        {
            var eTemplate = new TObject();
            eTemplate.Add<TString>("firstName");
            eTemplate.Add<TString>("html");

            dynamic e = new Json();
            e.Template = eTemplate;
            e.firstName = "Enecto!!";
            e.html = "<article><input value=\"{{firstName}}!!\"></article>";

            return e;
        });

        Handle.GET("/whoareyou2", () =>
        {
            var sTemplate = new TObject();
            sTemplate.Add<TString>("title");
            sTemplate.Add<TString>("html");

            dynamic s = new Json();
            s.Template = sTemplate;
            s.title = "Skype!!!!";
            s.html = "<div><button>{{title}}</button><div>";

            return s;
        });


        Handle.GET("/whoareyou_combined", () =>
        {
            var e = (Json)X.GET("/whoareyou1");
            var s = (Json)X.GET("/whoareyou2");

            var page = new LauncherWorkspace();
            page.appThumbnails.Add(e);
            page.appThumbnails.Add(s);

            return page;
        });
       
    }

}





