namespace Fossa.API.Persistence.Mongo.Entities;

public class AddressMongo
{
  public string? Line1 { get; set; }

  public string? Line2 { get; set; }

  public string? City { get; set; }

  public string? Subdivision { get; set; }

  public string? PostalCode { get; set; }

  public string? CountryCode { get; set; }
}
