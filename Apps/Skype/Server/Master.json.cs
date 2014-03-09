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
        Handle.GET("/person/{?}", (String personId) =>
        {
            var widget = new CallWidget()
            {
                // Html = "/person.html" // <-- why this works? there is no person.html in this App
                Html = "/callWidget.html"
                //, Data = person
            };
            //widget.Transaction = new Transaction();
            return widget;
        });
      
    }
}




