using Starcounter;

[ContactPage_json]
partial class ContactPage : Page
{
    void Handle(Input.Save input)
    {
        Transaction.Commit();
    }

    void Handle(Input.SelectedCompanyIndex input)
    {
        var index = (int)input.Value;
        var company = Companies[index];
        ((SuperCRM.Contact)Data).Company = (SuperCRM.Company)company.Data;
    }
}
