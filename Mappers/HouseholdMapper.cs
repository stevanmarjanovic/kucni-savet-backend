using KucniSavetBackend.Domain;
using KucniSavetBackend.DTO.Responses;
using KucniSavetBackend.Persistance.Documents;

namespace KucniSavetBackend.Mappers;

public static class HouseholdMapper
{
    public static HouseholdResponse ToResponse(this Household household) => new()
    {
        Id = household.Id,
        Name = household.Name
    };

    public static Household ToDomain(this HouseholdDocument household) => new()
    {
        Id = household.Id.Split('/').Last(),
        Name = household.Name
    };
}