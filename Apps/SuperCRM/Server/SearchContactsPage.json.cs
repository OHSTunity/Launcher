using Starcounter;

[SearchContactsPage_json]
partial class SearchContactsPage : Page
{
}

[SearchContactsPage_json.Contacts]
partial class SearchContactsPageContacts : Page, IBound<SuperCRM.Contact>
{
    protected override string UriFragment
    {
        get
        {
            return "/launcher/workspace/super-crm/contacts/" + Data.GetObjectID();
        }
    }
}