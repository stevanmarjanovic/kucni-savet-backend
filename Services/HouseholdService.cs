using KucniSavetBackend.Domain;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Interfaces.Services;

namespace KucniSavetBackend.Services;

public class HouseholdService(IHouseholdRepository householdRepository) : IHouseholdService
{
    public async Task<Household?> CreateAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            // Do validation in this layer
        }

        var household = new Household
        {
            Name = name
        };

        household = await householdRepository.CreateAsync(household);
        
        return household;
    }

    public async Task<Household?> GetByIdAsync(string id)
    {
        var household = await householdRepository.GetByIdAsync(id);
        return household;
    }

    public async Task<Household?> UpdateAsync(string id, string name)
    {
        var household = await householdRepository.GetByIdAsync(id);

        if (household is null) return null;

        household.Name = name;

        household = await householdRepository.UpdateAsync(household);

        return household;
    }
}