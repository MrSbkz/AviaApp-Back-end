namespace Data.Entities;

public class Country
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IList<City>? Cities { get; set; }
}