using System;
using Starcounter;                                  // Most stuff relating to the database, JSON and communication is in this namespace
using Starcounter.Internal;
using Starcounter.Templates;

[Master_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Master : Page {

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {
        StarcounterEnvironment.AppName = "Store";

        Handle.GET("/person/{?}", (String personId) =>
        {
            var s = new Store.StoreApp();

            var a = s.Apps.Add();
            a.Appname = "En Bambim";
            a.Description = "Supercool En tracking app.";

            s.Html = "<template repeat=\"{{Apps}}\"><article style=\"border: 1px solid #DDD; border-radius:0.4em; padding: 0 1.5em 1.5em\"><h2>{{Appname}}</h2>{{Description}}<button onclick=\"this.model.Buy$ = null\" value=\"{{Buy$}}\">buy</button></article></template>";

            return s;
        });
        
        Handle.POST("/init-demo-data", () => {      // The Handle class is where you register new handlers for incomming requests.
            DemoData.Create();                      // Will create some demo data.
            return 201;                             // Returning an integer is the shortcut for returning a response with a status code.
        });

        Handle.GET("/master", () =>
        {
            Master m = new Master()
            {                                       // This is the root view model for our application. A view model is a JSON object/tree.
                Html = "/master.html",              // This is just a field we added to allow the client to know what Html to load. No magic.
            };
            m.Session = new Session();              // Save the JSON on the server to be accessed using GET/PATCH to allow it to be used as a view model in web components.
            return m;                               // Return the JSON or the HTML depending on the type asked for. See Page.json on how Starcounter knowns what to return.
        });


        Handle.GET("/primary", () =>                // The main screen of the app
        {
            var m = Master.GET("/master");          // Create the view model for the main application frame.
            PrimaryApp p = new PrimaryApp();        // The email application also consists of a view model.
            p.Html = "/primary.html";               // Starcounter is a generic server and does not know of Html, so this is a variable we create in Page.json
            p.AddSomeNiceMenuItems(m);              // Adds some menu items in the main menu (by modifying the master view model)
            m.ApplicationPage = p;                  // Place the email applications view model inside the main application frame as its subpage.
            m.ApplicationName = "Tab 1";            // Used to highlight the current tab in the client
            return p;                               // Returns the home page. As you can see in Page.json, we taught it how to serve both HTML and the JSON view model without any extra work.
        });

        Handle.GET("/primary/create", () => 
        {
            var p = PrimaryApp.GET("/primary");
            p.FocusedPage = new PrimaryPage()
            {
                Html = "/primary-create.html"
            };
            return p;
        });
    }
}




