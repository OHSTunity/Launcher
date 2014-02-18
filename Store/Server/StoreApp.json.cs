using Starcounter;

namespace Store
{
    [StoreApp_json]
    partial class StoreApp : Page
    {
        protected override void OnData()
        {
            Apps = Db.SQL("SELECT a FROM App a");
        }
    }

    [StoreApp_json.Apps]
    partial class StoreAppApp : Json
    {
        void Handle(Input.Buy input)
        {
            this.Appname = "Clicked";
        }
    }
}
