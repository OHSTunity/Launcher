using System;
using Starcounter;                           // Most stuff relating to the database, JSON and communication is in this namespace

[Master_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Master : Page {

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main()
    {
        // Setting default handler level to 1.
        HandlerOptions.DefaultHandlerLevel = 1;
        Handlers.AddExtraHandlerLevel();

        Handle.GET("/dashboard", GetNoiseResponse);
        Handle.GET("/menu", GetNoiseResponse);
        Handle.GET<String>("/search?query={?}", GetNoiseResponse);
        Handle.GET("/supercrm/companies/add", GetNoiseResponse);
        Handle.GET("/supercrm/partials/companies/add", GetNoiseResponse);
        Handle.GET<String>("/supercrm/companies/{?}", GetNoiseResponse);
        Handle.GET<String>("/supercrm/partials/companies/{?}", GetNoiseResponse);
        Handle.GET("/supercrm/contacts/add", GetNoiseResponse);
        Handle.GET("/supercrm/partials/contacts/add", GetNoiseResponse);
        Handle.GET<String>("/supercrm/contacts/{?}", GetNoiseResponse);
        Handle.GET<String>("/supercrm/partials/contacts/{?}", GetNoiseResponse);
        Handle.GET("/supercrm/delete-all-data", GetNoiseResponse);
        Handle.GET("/skyper/friends-list", GetNoiseResponse);
        Handle.GET("/skyper/partials/friends-list", GetNoiseResponse);
        Handle.GET("/polyjuiceboilerplate", GetNoiseResponse);
        Handle.GET("/polyjuiceboilerplate/ingredients", GetNoiseResponse);
        Handle.GET("/polyjuiceboilerplate/ingredients/add", GetNoiseResponse);

        Handle.GET("/board/threads/add", GetNoiseResponse);
        Handle.GET("/board/threads", GetNoiseResponse);
        Handle.GET("/board", GetNoiseResponse);
    }

    static Response GetNoiseResponse()
    {
        var page = new NoisePage()
        {
            Html = "/noise.html"
        };

        return page;
    }

    static Response GetNoiseResponse(String url)
    {
        var page = new NoisePage()
        {
            Html = "/noise.html"
        };

        return page;
    }
}

/**
 * NoisePage extends Page so in stack traces we can see when the object's origin is the Noise app
 */
public class NoisePage : Page {

}



