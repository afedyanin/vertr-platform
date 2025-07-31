namespace Vertr.Platform.Host.Components.Models;

public record SimplePerson
{
    public int PersonId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }

    public SimplePerson(int personId, string firstName, string lastName, int age)
    {
        PersonId = personId;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}
