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
        Handle.GET("/super-crm/company/{?}", (String companyId) => {
            var page = (CompanyPage)X.GET("/super-crm/partials/company/" + companyId);
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            return m;
        });
    
        Handle.GET("/super-crm/partials/company/{?}", (String companyId) => {
            CompanyPage c =  new CompanyPage() {
                Name = "Id Software",
                Revenue = 0,
                Html = "/company.html"
            };
            c.Contacts.Add( X.GET("/super-crm/partials/contact/Albert/Scientist") );
            c.Contacts.Add( X.GET("/super-crm/partials/contact/John/Programmer") );

            return c;
        });
    
        Handle.GET("/super-crm/partials/contact/{?}/{?}", (String firstName, String title) => {
            ContactPage c = new ContactPage()
            {
                Name = firstName,
                Title = title,
                Html = "/contact.html"
            };
            return c;
        });
    
        Handle.GET("/super-crm", ()=>{
            var m = new Master();
            //Session.Data = m;
            return m;
        });

        Handle.IsMapperHandler = true;
        Handle.GET("/super-crm/partials/contact/{?}/{?}", (String firstName, String title) => {
            // String objectId = (String) Db.SQL("SELECT p.ObjectId FROM Person p WHERE p.Name = ?", firstName).First;
            // return X.GET("/societyobjects/ring2/employee/" + objectId);

            if (firstName == "Albert")
                return (Json) X.GET("/societyobjects/ring1/person/1");
            else if (firstName == "John")
                return (Json) X.GET("/societyobjects/ring1/person/2");
            else throw new Exception("Wrong first name!");
        });

        Handle.GET("/societyobjects/ring1/person/{?}", (String objectId) => {
            // var c = Db.SQL("SELECT e FROM Employee e WHERE e.ObjectId = ?", objectId);
            // return X.GETWITHOUTPREGET("/super-crm/partials/contact/" + p.WhoIs.FullName + "/" + p.Title);

            Handle.CallOnlyNonMapperHandlers = true;

            if (objectId == "1")
                return (Json) X.GET("/super-crm/partials/contact/Albert/Scientist");
            else if (objectId == "2")
                return (Json) X.GET("/super-crm/partials/contact/John/Programmer");
            else throw new Exception("Wrong objectId!");
        });
    }
}




