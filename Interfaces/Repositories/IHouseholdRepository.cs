using KucniSavetBackend.Domain;

namespace KucniSavetBackend.Interfaces.Repositories;

public interface IHouseholdRepository
{
    Task<Household?> GetByIdAsync(string key);
    Task<Household?> CreateAsync(Household household);
    Task<Household?> UpdateAsync(Household household);
    Task DeleteAsync(string key);
    Task DeleteAsync(Household household);
}