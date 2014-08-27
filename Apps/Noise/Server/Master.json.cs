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
        Handle.GET("/dashboard", GetNoiseResponse);
        Handle.GET("/menu", GetNoiseResponse);
        Handle.GET<String>("/search?query={?}", GetNoiseResponse);
        //after https://github.com/Polyjuice/Launcher/issues/13 was closed, Noise App is no longer needed to force namespaces also on single application outputs
        //however, still waiting for https://github.com/Polyjuice/Launcher/issues/14 / https://github.com/Starcounter/Starcounter/issues/1568 to be able 
        //to respond to any request with a wildcard
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



