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
            CompanyPage page = new CompanyPage()
            {
                Uri = "/launcher/workspace/super-crm/companies/add",
                Html = "/company.html"
            };
            page.Transaction = new Transaction();
            page.Transaction.Add(() =>
            {
                page.Data = new SuperCRM.Company();
            });

            return page;
        });

        Handle.GET("/super-crm/companies/{?}", (String companyId) => 
        {
            var page = CompanyPage.GET("/super-crm/partials/companies/" + companyId);
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            return m;
        });

        Handle.GET("/super-crm/partials/companies/{?}", (String objectId) =>
        {
            CompanyPage c = new CompanyPage()
            {
                Html = "/company.html"
            };
            var company = SQL<SuperCRM.Company>("SELECT c FROM SuperCRM.Company c WHERE ObjectId = ?", objectId).First;
            c.Data = company;
            //c.Uri = "/launcher/workspace/super-crm/companies/" + objectId;
            c.Transaction = new Transaction();

            var contacts = SQL<SuperCRM.Contact>("SELECT c FROM SuperCRM.Contact c WHERE Company = ?", company);
            var enumerator = contacts.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var p = (ContactPage)X.GET("/super-crm/partials/contacts/" + enumerator.Current.GetObjectID());
                c.Contacts.Add(p);
            }

            return c;
        });

        Handle.GET("/super-crm/contacts/add", () =>
        {
            var page = (ContactPage)X.GET("/super-crm/partials/contacts/add");
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            /*if (m.AddContactToCompany.Data != null)
            {
                ((SuperCRM.Contact)page.Data).Company = (SuperCRM.Company)m.AddContactToCompany.Data;
                m.AddContactToCompany.Data = null;
            }*/
            return m;
        });

        Handle.GET("/super-crm/partials/contacts/add", () =>
        {
            ContactPage page = new ContactPage()
            {
                Html = "/contact.html",
                Uri = "/super-crm/partials/contacts/add"
            };
            var companies = SQL<SuperCRM.Company>("SELECT c FROM SuperCRM.Company c");
            page.Transaction = new Transaction();
            page.SelectedCompanyIndex = -1;
            page.Transaction.Add(() =>
            {
                var contact = new SuperCRM.Contact()
                {
                    Person = new SuperCRM.Person()
                };
                if (companies.First != null)
                {
                    contact.Company = companies.First;
                    page.SelectedCompanyIndex = 0;
                }
                page.Data = contact;
            });
            page.Companies.Data = companies;

            /*page.Person.Transaction = new Transaction();
            page.Person.Transaction.Add(() =>
            {
                var person = new SuperCRM.Person();
                page.Person.Data = person;
            });

            page.Transaction = new Transaction();
            page.Transaction.Add(() =>
            {
                var contact = new SuperCRM.Contact();
                contact.Title = "Specialist";
                contact.Person = page.Person.Data;
                page.Data = contact;
            });*/

            return page;
        });

        Handle.GET("/super-crm/contacts/{?}", (String objectId) =>
        {
            var page = (ContactPage)X.GET("/super-crm/partials/contacts/" + objectId);
            Master m = (Master)X.GET("/super-crm");
            m.FavoriteCustomer = page;
            return m;
        });

        Handle.GET("/super-crm/partials/contacts/{?}", (String objectId) =>
        {
            ContactPage page = new ContactPage()
            {
                Html = "/contact.html"
            };
            var contact = SQL<SuperCRM.Contact>("SELECT c FROM SuperCRM.Contact c WHERE ObjectId = ?", objectId).First;
            if (contact == null)
            {
                //return empty response
                return new Page()
                {
                    Html = ""
                };
            }
            page.Data = contact;
            //page.Uri = "/launcher/workspace/super-crm/contacts/" + objectId;
            page.Transaction = new Transaction();
            page.SelectedCompanyIndex = -1;
            var companies = SQL<SuperCRM.Company>("SELECT c FROM SuperCRM.Company c");
            page.Companies.Data = companies;
            var enumertator = companies.GetEnumerator();
            var i = 0;
            while (enumertator.MoveNext())
            {
                if (enumertator.Current.Equals(contact.Company))
                {
                    page.SelectedCompanyIndex = i;
                    break;
                }
                i++;
            }
            return page;
        });
    
        Handle.GET("/super-crm", ()=>{
            var m = new Master() {
                Html = "/SuperCRM.html"
            };

            //m.Session = new Session();

            return m;
        });

        Handle.IsMapperHandler = true;
        Handle.GET("/super-crm/partials/contacts/{?}", (String objectId) => {
            // String objectId = (String) Db.SQL("SELECT p.ObjectId FROM Person p WHERE p.Name = ?", firstName).First;
            // return X.GET("/societyobjects/ring2/employee/" + objectId);

            return (Json)X.GET("/societyobjects/ring1/person/" + objectId);
        });

        Handle.GET("/societyobjects/ring1/person/{?}", (String objectId) => {
            // var c = Db.SQL("SELECT e FROM Employee e WHERE e.ObjectId = ?", objectId);
            // return X.GETWITHOUTPREGET("/super-crm/partials/contact/" + p.WhoIs.FullName + "/" + p.Title);

            Handle.CallOnlyNonMapperHandlers = true;
            return (Json)X.GET("/super-crm/partials/contacts/" + objectId);
        });

        Handle.GET("/dashboard", () =>
        {
            Response resp;
            X.GET("/super-crm/partials/search/", out resp);
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
            X.GET("/super-crm/partials/search/" + HttpUtility.UrlEncode(query), out resp);
            return resp;
        });

        Handle.GET("/super-crm/partials/search/{?}", (String query) =>
        {
            SearchPage page = new SearchPage()
            {
                Html = "/search.html"
            };
            var wildcardQuery = "%" + query + "%";
            var companies = SQL<SuperCRM.Company>("SELECT c FROM SuperCRM.Company c WHERE Name LIKE ? FETCH ?", wildcardQuery, 5);
            var contacts = SQL<SuperCRM.Contact>("SELECT c FROM SuperCRM.Contact c WHERE Person.FirstName LIKE ? OR Person.LastName LIKE ? OR Title LIKE ? OR Company.Name LIKE ? FETCH ?", wildcardQuery, wildcardQuery, wildcardQuery, wildcardQuery, 5);
            page.Companies.Data = companies;
            page.Contacts.Data = contacts;
            return page;
        });

        Handle.GET("/super-crm/delete-all-data", () =>
        {
            Db.Transaction(() =>
            {
                SlowSQL("DELETE FROM SuperCRM.Company");
                SlowSQL("DELETE FROM SuperCRM.Contact");
                SlowSQL("DELETE FROM SuperCRM.Person");
            });
            Master m = (Master)X.GET("/super-crm");
            m.Message = "SugarCRM's company, contact and person data was removed";
            return m;
        });

    }
}




