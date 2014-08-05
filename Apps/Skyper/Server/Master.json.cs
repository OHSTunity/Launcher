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
        HandlerOptions h1 = new HandlerOptions() { HandlerLevel = 1 };
        HandlerOptions h0 = new HandlerOptions() { HandlerLevel = 0 };

        Handle.GET("/skyper/partials/skyper-user/{?}", (String objectId) =>
        {
            return (Json)X.GET("/societyobjects/ring1/person/" + objectId);
        }, h0);

        Handle.GET("/societyobjects/ring1/person/{?}", (String objectId) =>
        {
            return (Json)X.GET("/skyper/partials/skyper-user/" + objectId, 0, h1);
        }, h0);

        // Setting default handler level to 1.
        HandlerOptions.DefaultHandlerLevel = 1;
        Handlers.AddExtraHandlerLevel();

        Handle.GET("/skyper/friends-list", () =>
        {
            SkyperFriendsList page = (SkyperFriendsList)X.GET("/skyper/partials/friends-list");
            Master m = (Master)X.GET("/skyper");
            m.MyOnlyFriend = page;
            return m;
        });

        Handle.GET("/skyper/partials/friends-list", () =>
        {
            var page = new SkyperFriendsList()
            {
                Html = "/skyper-friends-list.html"
            };

            var contacts = SQL<Skyper.Contact>("SELECT c FROM Skyper.Contact c");
            var enumerator = contacts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var p = new SkyperUserPage()
                {
                    Html = "/skyper-user.html"
                };
                p.Contact.Data = enumerator.Current;
                p.Transaction = new Transaction();
                var s = new SkyperFriendsList.FriendsElementJson();
                s.Skyper = p;
                s.Html = "/skyper-user.html";
                page.Friends.Add(s);
            }
            return page;
        });

        Handle.GET("/skyper/skype-user/{?}", (String objectId) =>
        {
            SkyperUserPage page = (SkyperUserPage)X.GET("/skyper/partials/skyper-user/" + objectId);
            Master m = (Master) X.GET("/skyper");
            m.MyOnlyFriend = page;
            return m;
        });

        Handle.GET("/skyper/partials/skyper-user/{?}", (String objectId) =>
        {
            var page = new SkyperUserPage()
            {
                Html = "/skyper-user.html",
                Editing = false
            };
            page.Transaction = new Transaction();
            Skyper.Contact contact = SQL<Skyper.Contact>("SELECT c FROM Skyper.Contact c WHERE c.ExternalId = ?", objectId).First;
            if (contact == null)
            {
                page.Transaction.Add(() =>
                {
                    contact = new Skyper.Contact()
                    {
                        ExternalId = objectId,
                        SkypeId = ""
                    };
                    if (contact.SkypeId == "")
                    {
                        page.Editing = true;
                    }
                    page.Contact.Data = contact;
                });
            }
            else
            {
                page.Contact.Data = contact;
                if (contact.SkypeId == "")
                {
                    page.Editing = true;
                }
            }
            return page;
        });

        Handle.GET("/skyper", () =>
        {
            Master m = new Master() {
                Html = "/skyper.html"
            };
            return m;
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




