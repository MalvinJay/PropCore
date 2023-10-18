using PropCore.Domain.Common;
using PropCore.Domain.Enums;
using PropCore.Domain.ValueObjects;

namespace PropCore.Domain.Entities;

public sealed class Address : Entity
{
    private Address()
    {
    }

    public string Line1 { get; private set; } = null!;
    public string? Line2 { get; private set; }
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }

    public static Address Create(
        string line1,
        string? line2,
        string city,
        string state,
        string postalCode,
        string country,
        double? latitude = null,
        double? longitude = null)
    {
        return new Address
        {
            Line1 = line1,
            Line2 = line2,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country,
            Latitude = latitude,
            Longitude = longitude
        };
    }
}