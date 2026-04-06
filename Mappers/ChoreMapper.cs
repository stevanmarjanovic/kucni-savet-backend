using KucniSavetBackend.Domain;
using KucniSavetBackend.DTO.Responses;
using KucniSavetBackend.Persistance.Documents;

namespace KucniSavetBackend.Mappers;

public static class ChoreMapper
{
    public static ChoreResponse ToResponse(this Chore chore) => new()
    {
        Id = chore.Id ?? null!,
        Name = chore.Name,
        Frequency = chore.Frequency,
        LastDone = chore.LastDone,
        ToDo = chore.ToDo,
        Assignees = chore.Assignees.Select(assignee => assignee.ToResponse()).ToList()
    };

    public static Chore ToDomain(this ChoreDocument chore, List<UserDocument>? users = null, HouseholdDocument? household = null)
    {
        var householdDomain = household?.ToDomain() ?? new Household();
        var usersDomain = users?
            .Where(userDocument => chore.AssigneesIds.Contains(userDocument.Id))
            .Select(user => user.ToDomain(household))
            .ToList();
        return new Chore
        {
            Id = chore.Id.Split('/').Last(),
            Name = chore.Name,
            Frequency = chore.Frequency,
            LastDone = chore.LastDone,
            Assignees = usersDomain ?? [],
            Household = householdDomain
        };
    }
}
