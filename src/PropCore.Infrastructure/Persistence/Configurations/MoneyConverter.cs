using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PropCore.Domain.ValueObjects;

namespace PropCore.Infrastructure.Persistence.Configurations;

internal sealed class MoneyConverter()
    : ValueConverter<Money, decimal>(
        money => money.Amount,
        value => Money.Create(value))
{
}