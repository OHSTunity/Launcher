using Starcounter;

[SkyperUserPage_json]
partial class SkyperUserPage : Page
{
    void Handle(Input.Save input)
    {
        Transaction.Commit();
        if (Contact.SkypeId != "")
        {
            Editing = false; //does not work until https://github.com/Starcounter/Starcounter/issues/1871 is fixed
        }
    }
}
