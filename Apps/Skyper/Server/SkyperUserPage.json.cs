using Starcounter;

[SkyperUserPage_json]
partial class SkyperUserPage : Page
{
    void Handle(Input.Save input)
    {
        Transaction.Commit();
        if (Contact.SkypeId != "")
        {
            Editing = false;
        }
    }
}
