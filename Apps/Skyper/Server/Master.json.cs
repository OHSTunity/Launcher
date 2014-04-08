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
    static void Main()
    {
        Handle.GET("/skyper/skype-user/{?}", (String objectId) =>
        {
            SkyperUserPage page = (SkyperUserPage)X.GET("/skyper/partials/skyper-user/" + objectId);
            Master m = (Master) X.GET("/skyper");
            m.MyOnlyFriend = page;
            return m;
        });

        Handle.GET("/skyper/partials/skyper-user/{?}", (String objectId) =>
        {
            //if has Skype account
                return new SkyperUserPage()
                {
                    Name = objectId + " on Skype",
                    SkypeId = "skype_" + objectId,
                    Html = "/skyper-user.html"
                }; 
            //}
        });

        Handle.GET("/skyper", () =>
        {
            Master m = new Master() {
                Html = "/skyper.html"
            };
            
            //m.Session = new Session();

            return m;
        });

        Handle.IsMapperHandler = true;
        Handle.GET("/skyper/partials/skyper-user/{?}", (String objectId) => {
           return (Json) X.GET("/societyobjects/ring1/person/" + objectId);
        });

        Handle.GET("/societyobjects/ring1/person/{?}", (String objectId) => {
           Handle.CallOnlyNonMapperHandlers = true;
           return (Json) X.GET("/skyper/partials/skyper-user/" + objectId);
        });

        Handle.GET("/dashboard", () =>
        {
            var page = new SkyperUserPage()
            {
                //Html = "/dashboard.html" // will break as Map does also use `/dashboard.html`
                Html = "/skyper-dashboard.html"
            };

            page.Transaction = new Transaction();
            return page;
        });
    }
}




