namespace UsingMinimalApiDiscovery.Data;

public class Customer
{
  public int Id { get; set; }
  public string? Name { get; set; }
  public string? Phone { get; set; }
  public DateOnly? Birthdate { get; set; }
}