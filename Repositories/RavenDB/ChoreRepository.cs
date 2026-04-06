using KucniSavetBackend.Domain;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Mappers;
using KucniSavetBackend.Persistance.Documents;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace KucniSavetBackend.Repositories.RavenDB;

public class ChoreRepository(IAsyncDocumentSession session) : IChoreRepository
{
    public async Task<Chore?> CreateAsync(Chore? chore)
    {
        if (chore is null)
            return null;

        var householdId = RavenIdMapper.HouseholdId(chore.Household.Id);

        var doc = new ChoreDocument
        {
            HouseholdId = householdId,
            Frequency = chore.Frequency,
            Name = chore.Name,
            LastDone = chore.LastDone,
            AssigneesIds = chore.Assignees.Select(assignee => assignee.Id!).ToList()
        };

        await session.StoreAsync(doc);
        await session.SaveChangesAsync();
        
        return doc.ToDomain();
    }

    public async Task<Chore?> GetByIdAsync(string key)
    {
        var id = RavenIdMapper.ChoreId(key);

        var doc = await session
            .Include<ChoreDocument>(chore => chore.AssigneesIds)
            .Include(chore => chore.HouseholdId)
            .LoadAsync<ChoreDocument>(id);
        var assignees = await session.LoadAsync<UserDocument>(doc.AssigneesIds);
        var household = await session.LoadAsync<HouseholdDocument>(doc.HouseholdId);

        return doc.ToDomain(assignees.Values.ToList(), household);
    }

    public async Task<List<Chore>> GetByHouseholdIdAsync(string householdId)
    {
        householdId = RavenIdMapper.HouseholdId(householdId);
        
        var docs = await session.Query<ChoreDocument>()
            .Include(doc => doc.AssigneesIds)
            .Include(doc => householdId)
            .Where(chore => chore.HouseholdId == householdId)
            .ToListAsync();
        
        var usersIds = docs.SelectMany(doc => doc.AssigneesIds).ToList();
        var userDocuments = await session.LoadAsync<UserDocument>(usersIds);
        var householdDocument = await session.LoadAsync<HouseholdDocument>(householdId);
        
        return docs.Select(doc => doc.ToDomain(userDocuments.Values.ToList(), householdDocument)).ToList();
    }

    public async Task<Chore?> UpdateAsync(Chore chore)
    {
        if (chore.Id is null)
            return null;
        
        var choreId = RavenIdMapper.ChoreId(chore.Id);
        var doc = await session
            .Include<ChoreDocument>(choreDocument => choreDocument.HouseholdId)
            .Include<ChoreDocument>(choreDocument => choreDocument.AssigneesIds)
            .LoadAsync<ChoreDocument>(choreId);
        
        doc.Name = chore.Name;
        doc.Frequency = chore.Frequency;
        doc.LastDone = chore.LastDone;
        doc.AssigneesIds = chore.Assignees.Select(assignee => RavenIdMapper.UserId(assignee.Id!)).ToList();
        
        var assignees = await session.LoadAsync<UserDocument>(doc.AssigneesIds);
        var household = await session.LoadAsync<HouseholdDocument>(doc.HouseholdId);

        await session.SaveChangesAsync();

        return doc.ToDomain(assignees.Values.ToList(), household);
    }

    public async Task DeleteAsync(string key)
    {
        var id = RavenIdMapper.ChoreId(key);
        session.Delete(id);
        await session.SaveChangesAsync();
    }

    public async Task DeleteAsync(Chore chore)
    {
        if (chore.Id is null)
            return;
        
        await DeleteAsync(chore.Id);
    }
}