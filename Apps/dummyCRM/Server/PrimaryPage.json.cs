using Starcounter;

[PrimaryPage_json]
partial class PrimaryPage : Page
{
    void Handle(Input.Delete input)
    {
        this.FirstName = "Clicked";
    }
}