using Starcounter;

namespace Store
{
    [Store_json]
    partial class Store : Json
    {
        protected override void OnData()
        {
            Apps = Db.SQL("SELECT a FROM App a");
        }
    }
}
