using Starcounter;

[Database]
public class Company
{
    public string Name;
}

[Database]
public class Person
{
    public string FirstName;
    public string LastName;
}

[Database]
public class Employee
{
    public Company Company;
    public Person Person;
}
