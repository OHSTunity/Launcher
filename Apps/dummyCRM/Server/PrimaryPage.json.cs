using Starcounter;

[PrimaryPage_json]
partial class PrimaryPage : Page
{
    void Handle(Input.Delete input)
    {
        Data.Delete();
        Transaction.Commit();
    }
}