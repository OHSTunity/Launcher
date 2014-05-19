using Starcounter;

[ContactPage_json]
partial class ContactPage : Page
{
    void Handle(Input.Save input)
    {
        Transaction.Commit();
        RedirectUrl = Uri;
    }

    void Handle(Input.SelectedCompanyIndex input)
    {
        var index = (int)input.Value;
        var company = Companies[index];
        ((SuperCRM.Contact)Data).Company = (SuperCRM.Company)company.Data;
    }

    protected override string UriFragment
    {
        get
        {
            return "/launcher/workspace/supercrm/contacts/" + Data.GetObjectID();
        }
    }
}
