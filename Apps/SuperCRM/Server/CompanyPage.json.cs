using Starcounter;

[CompanyPage_json]
partial class CompanyPage : Page
{
    void Handle(Input.Save input)
    {
        Transaction.Commit();
        RedirectUrl = Uri;
    }

    void Handle(Input.AddContact input)
    {
        Transaction.Commit();
        RedirectUrl = "/launcher/workspace/super-crm/contacts/add";
    }

    protected override string UriFragment
    {
        get
        {
            return "/launcher/workspace/super-crm/companies/" + Data.GetObjectID();
        }
    }
}
