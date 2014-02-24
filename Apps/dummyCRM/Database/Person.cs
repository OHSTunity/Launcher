using Starcounter;
using System.Threading;

[Database]
public class Person
{
    static int GlobalId;

    public int Id;
    public string FirstName;
    public string LastName;

    public Person()
    {
        Id = Interlocked.Increment(ref GlobalId);
    }

    public string DisplayName
    {
        get
        {
            string[] all = new string[] { FirstName, LastName };
            string complete = string.Join(" ", all);
            return System.Text.RegularExpressions.Regex.Replace(complete, @"\s+", " ");
        }
        set
        {
            //if we allow to edit contact name details in a single `DisplayName` field,
            //here goes logic how to parse this field into actual properties
        }
    }
}
