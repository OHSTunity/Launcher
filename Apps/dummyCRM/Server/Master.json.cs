using System;
using Starcounter;                                  // Most stuff relating to the database, JSON and communication is in this namespace
using Starcounter.Internal;
using Starcounter.Templates;
using System.Web;

[Master_json]                                       // This attribute tells Starcounter that the class corresponds to an object in the JSON-by-example file.
partial class Master : Page {

    /// <summary>
    /// Every application in Starcounter works like a console application. They have an .EXE ending. They have a Main() function and
    /// they can do console output. However, they are run inside the scope of a database rather than connecting to it.
    /// </summary>
    static void Main()
    {
        Handle.GET("/super-crm/companies/add", () =>
        {
            var page = (CompanyPage)X.GET("/super-crm/partials/companies/add");
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            return m;
        });

        Handle.GET("/super-crm/partials/companies/add", () =>
        {
            CompanyPage c = new CompanyPage()
            {
                Name = "",
                Revenue = 0,
                Uri = "/launcher/workspace/super-crm/companies/add",
                Html = "/company.html"
            };

            return c;
        });

        Handle.GET("/super-crm/companies/{?}", (String companyId) => 
        {
            var page = (CompanyPage)X.GET("/super-crm/partials/companies/" + companyId);
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            return m;
        });

        Handle.GET("/super-crm/partials/companies/{?}", (String companyId) =>
        {
            CompanyPage c = new CompanyPage()
            {
                Name = "Id Software",
                Revenue = 0,
                Uri = "/launcher/workspace/super-crm/companies/1",
                Html = "/company.html"
            };
            c.Contacts.Add((ContactPage)X.GET("/super-crm/partials/contact/Albert/Scientist"));
            c.Contacts.Add((ContactPage)X.GET("/super-crm/partials/contact/John/Programmer"));

            return c;
        });

        Handle.GET("/super-crm/contacts/add", () =>
        {
            var page = (ContactPage)X.GET("/super-crm/partials/contacts/add");
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            return m;
        });
    
        Handle.GET("/super-crm/partials/contacts/add", () => {
            ContactPage c = new ContactPage()
            {
                Name = "",
                Title = "",
                Email = "",
                Html = "/contact.html",
                Uri = "/super-crm/partials/contacts/add"
            };
            return c;
        });

        Handle.GET("/super-crm/contact/{?}/{?}", (String firstName, String title) =>
        {
            var page = (ContactPage)X.GET("/super-crm/partials/contact/" + firstName + "/" + title);
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            return m;
        });

        Handle.GET("/super-crm/partials/contact/{?}/{?}", (String firstName, String title) =>
        {
            ContactPage c = new ContactPage()
            {
                Name = firstName,
                Title = title,
                Email = "name@company.com",
                Html = "/contact.html",
                Uri = "/super-crm/partials/contact/" + firstName + "/" + title
            };
            return c;
        });
    
        Handle.GET("/super-crm", ()=>{
            var m = new Master() {
                Html = "/dummyCRM.html"
            };

            //m.Session = new Session();

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

        /*Handle.GET("/dashboard", () =>
        {
            Response resp;
            X.GET("/super-crm/partials/contact/John/Programmer", out resp);
            return resp;
        });*/

        Handle.GET("/dashboard", () =>
        {
            Response resp;
            X.GET("/super-crm/partials/search-contacts/John/Programmer", out resp);
            return resp;
        });

        Handle.GET("/dashboard", () =>
        {
            Response resp;
            X.GET("/super-crm/partials/search-companies/" + "123", out resp);
            return resp;
        });

        Handle.GET("/menu", () =>
        {
            var p = new Page()
            {
                Html = "/menu.html"
            };
            return p;
        });

        Handle.GET("/search?query={?}", (String query) =>
        {
            Response resp;
            //X.GET("/super-crm/partials/search-contacts/" + HttpUtility.UrlEncode(query), out resp);
            X.GET("/super-crm/partials/search-companies/" + HttpUtility.UrlEncode(query), out resp);
            return resp;
        });

        Handle.GET("/super-crm/partials/search-companies/{?}", (String companyId) =>
        {
            SearchCompaniesPage p = new SearchCompaniesPage()
            {
                Html = "/search-companies.html"
            };
            p.Companies.Add((CompanyPage)X.GET("/super-crm/partials/companies/" + companyId));
            return p;
        });

        Handle.GET("/super-crm/partials/search-contacts/{?}", (String companyId) =>
        {
            SearchContactsPage p = new SearchContactsPage()
            {
                Html = "/search-contacts.html"
            };
            p.Contacts.Add((ContactPage)X.GET("/super-crm/partials/contact/" + companyId + "/Programmer"));
            return p;
        });

    }
}




