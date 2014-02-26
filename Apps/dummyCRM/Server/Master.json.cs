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
        StarcounterEnvironment.AppName = "dummyCRM";
        
        Handle.POST("/init-demo-data", () => {      // The Handle class is where you register new handlers for incomming requests.
            DemoData.Create();                      // Will create some demo data.
            return 201;                             // Returning an integer is the shortcut for returning a response with a status code.
        });

        Handle.GET("/person/{?}", (String personId) =>
        {
            var person = Db.SQL<Person>("SELECT p FROM Person p WHERE FirstName=?", "Albert").First;

            var page = new PrimaryPage()
            {
                Html = "/person.html",
                Data = person
            };
            page.Transaction = new Transaction();
            return page;
        });
    }
}




