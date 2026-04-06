using KucniSavetBackend.Domain;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Mappers;
using KucniSavetBackend.Persistance.Documents;
using Raven.Client.Documents.Session;

namespace KucniSavetBackend.Repositories.RavenDB;

public class HouseholdRepository(IAsyncDocumentSession session) : IHouseholdRepository
{
    public async Task<Household?> GetByIdAsync(string key)
    {
        var id = RavenIdMapper.HouseholdId(key);
        
        var doc = await session.LoadAsync<HouseholdDocument>(id);

        return doc?.ToDomain();
    }

    public async Task<Household?> CreateAsync(Household household)
    {
        var doc = new HouseholdDocument
        {
            Name = household.Name
        };

        await session.StoreAsync(doc);
        await session.SaveChangesAsync();

        return doc.ToDomain();
    }

    public async Task<Household?> UpdateAsync(Household household)
    {
        var id = RavenIdMapper.HouseholdId(household.Id);
        var doc = await session.LoadAsync<HouseholdDocument>(id);

        doc.Name = household.Name;

        await session.SaveChangesAsync();

        return doc.ToDomain();
    }

    public async Task DeleteAsync(string key)
    {
        var id = RavenIdMapper.HouseholdId(key);

        session.Delete(id);

        await session.SaveChangesAsync();
    }

    public async Task DeleteAsync(Household household)
    {
        await DeleteAsync(household.Id);
    }
}
