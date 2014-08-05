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

        // Setting default handler level to 1.
        HandlerOptions.DefaultHandlerLevel = 1;
        Handlers.AddExtraHandlerLevel();

        Handle.GET("/person/{?}", (String personId) =>
        {
            var s = new Store.StoreApp();

            var a = s.Apps.Add();
            a.Appname = "En Bambim";
            a.Description = "Supercool En tracking app";

            var person = Db.SQL<Concepts.Ring1.Person>("SELECT p FROM Concepts.Ring1.Person p WHERE FirstName=?", "Albert").First;
            a.Author.Data = person;

            s.Html = "<template repeat=\"{{Apps}}\"><article style=\"border: 1px solid #DDD; border-radius:0.4em; padding: 0 1.5em 1.5em\"><h2>{{Appname}}</h2>{{Description}} by {{Author.FullName}}<button bind=\"{{Buy$}}\" onclick=\"setModelValue(this)\" value=\"null\">buy</button></article></template>";
            return s;
        });

        Handle.POST("/init-demo-data", () => {      // The Handle class is where you register new handlers for incomming requests.
            DemoData.Create();                      // Will create some demo data.
            return 201;                             // Returning an integer is the shortcut for returning a response with a status code.
        });
         
    }

    // Browsers will ask for "text/html" and we will give it to them
    // by loading the contents of the URI in our Html property
    public override string AsMimeType(MimeType type) {
        if (type == MimeType.Text_Html) {
            return this.Html;
            //return X.GET<string>(Html);
        }
        return base.AsMimeType(type);
    }
}




