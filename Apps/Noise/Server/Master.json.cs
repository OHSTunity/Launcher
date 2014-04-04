using System;
using Starcounter;                           // Most stuff relating to the database, JSON and communication is in this namespace

[Master_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Master : Page {

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main() {
        Handle.GET("/menu", GetNoiseResponse);
        Handle.GET("/super-crm/companies/add", GetNoiseResponse);
        Handle.GET("/super-crm/partials/companies/add", GetNoiseResponse);
        Handle.GET<String>("/super-crm/companies/{?}", GetNoiseResponse);
        Handle.GET<String>("/super-crm/partials/companies/{?}", GetNoiseResponse);
        Handle.GET("/super-crm/contacts/add", GetNoiseResponse);
        Handle.GET("/super-crm/partials/contacts/add", GetNoiseResponse);
    }

    static Response GetNoiseResponse()
    {
        var page = new Page()
        {
            Html = "/noise.html"
        };

        return page;
    }

    static Response GetNoiseResponse(String url)
    {
        var page = new Page()
        {
            Html = "/noise.html"
        };

        return page;
    }
}




