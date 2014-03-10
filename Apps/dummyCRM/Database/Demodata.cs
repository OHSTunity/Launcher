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
                var email = new Concepts.Ring2.EMailAddress() { EMail = "joachim.wester@me.com" };
                var albert = new Concepts.Ring1.Person() { 
                    FirstName = "Albert", 
                    Surname = "Einstein"
                };
                var relation = new Concepts.Ring2.EMailRelation()
                {
                    Address = email,
                    Addressee = albert
                };
            }
        });
    }
}
