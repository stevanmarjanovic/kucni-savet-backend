using KucniSavetBackend.Domain;

namespace KucniSavetBackend.Interfaces.Repositories;

public interface IChoreRepository
{
    Task<Chore?> GetByIdAsync(string key);
    Task<List<Chore>> GetByHouseholdIdAsync(string householdId);
    Task<Chore?> CreateAsync(Chore chore);
    Task<Chore?> UpdateAsync(Chore chore);
    Task DeleteAsync(string key);
    Task DeleteAsync(Chore chore);
}