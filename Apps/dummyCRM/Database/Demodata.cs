using Starcounter;

public class DemoData
{
    public static void Create()
    {
        Db.Transaction(() =>
        {
            var albert = new Person() { FirstName = "Albert", LastName = "Einstein", Email = "joachim.wester@me.com" };
        });
    }
}
