using Starcounter;

public class DemoData
{
    public static void Create()
    {
        Db.Transaction(() =>
        {
            Db.SlowSQL("DELETE FROM Concepts.Ring1.Person");
            var person = Db.SQL<Concepts.Ring1.Person>("SELECT p FROM Concepts.Ring1.Person p WHERE FirstName=?", "Albert").First;
            if (person == null)
            {
                var albert = new Concepts.Ring1.Person() { FirstName = "Albert", Surname = "Einstein" };
            }
        });
    }
}
