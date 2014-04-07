using Starcounter;

[CompanyPage_json]
partial class CompanyPage : Page
{
    void Handle(Input.Save input)
    {
        Transaction.Commit();
    }
}
