using Starcounter;

[SearchPage_json]
partial class SearchPage : Page
{
}

[SearchPage_json.Contacts]
partial class SearchPageContacts : Page, IBound<SuperCRM.Contact>
{
    protected override string UriFragment
    {
        get
        {
            return "/launcher/workspace/supercrm/contacts/" + Data.GetObjectID();
        }
    }
}

[SearchPage_json.Companies]
partial class SearchPageCompanies : Page, IBound<SuperCRM.Company>
{
    protected override string UriFragment
    {
        get
        {
            return "/launcher/workspace/supercrm/companies/" + Data.GetObjectID();
        }
    }
}
