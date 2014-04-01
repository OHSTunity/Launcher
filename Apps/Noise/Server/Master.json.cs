using System;
using Starcounter;                           // Most stuff relating to the database, JSON and communication is in this namespace

[Master_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Master : Page
{

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main()
    {

        // Handle.GET("/super-crm/partials/companies/{?}", (String url) =>  // <-- this works
        // Handle.GET("/super-crm/partials/companies/{?}", (String url) =>  // <-- this doesn't
        // Handle.GET("/super-crm/partials/{?}/{?}", (String name, String url) =>  // <-- nope
        // Handle.GET("/super-crm/{?}", (String name, String url) =>  // <-- nope
        Handle.GET("/{?}", (String url) => // <-- nope
        {
            var page = new Page()
            {
                Html = "/noise.html"
            };

            return page;
        });
    }
}




