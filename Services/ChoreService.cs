using KucniSavetBackend.Domain;
using KucniSavetBackend.Enums;
using KucniSavetBackend.Exceptions;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Interfaces.Services;

namespace KucniSavetBackend.Services;

public class ChoreService(IChoreRepository choreRepository, IUserRepository userRepository) : IChoreService
{
    public async Task<Chore?> GetByIdAsync(string id)
    {
        var chore = await choreRepository.GetByIdAsync(id) ?? throw new NotFoundException<Chore>(id);
        return chore;
    }

    public Task<List<Chore>> GetByHouseholdIdAsync(string householdId)
    {
        var chores = choreRepository.GetByHouseholdIdAsync(householdId);

        return chores;
    }
    
    public async Task<Chore?> CreateAsync(string name, Frequency frequency, string householdId)
    {
        if (string.IsNullOrEmpty(name))
        {
            // Do validation
        }

        var household = new Household
        {
            Id = householdId
        };
        var chore = new Chore
        {
            Household = household,
            Name = name,
            Frequency = frequency
        };

        chore = await choreRepository.CreateAsync(chore);

        return chore;
    }
    
    public async Task<Chore?> UpdateAsync(string id, string name, Frequency frequency)
    {
        var chore = await choreRepository.GetByIdAsync(id) ?? throw new NotFoundException<Chore>(id); // TODO Optimizate remote calls

        chore.Name = name;
        chore.Frequency = frequency;

        chore = await choreRepository.UpdateAsync(chore); // TODO Optimizate remote calls THIS IS A SECOND CALL

        return chore;
    }

    public async Task<Chore?> AddAssignee(string choreId, string assigneeId)
    {
        var chore = await choreRepository.GetByIdAsync(choreId) ?? throw new NotFoundException<Chore>(choreId);
        var assignee = await userRepository.GetByIdAsync(assigneeId) ?? throw new NotFoundException<User>(assigneeId);

        if (chore.Assignees.All(a => a.Id != assignee.Id))
            chore.Assignees.Add(assignee);

        chore = await choreRepository.UpdateAsync(chore);

        return chore;
    }

    public async Task<Chore?> RemoveAssignee(string choreId, string assigneeId)
    {
        var chore = await choreRepository.GetByIdAsync(choreId) ?? throw new NotFoundException<Chore>(choreId);
        chore.Assignees.RemoveAll(assignee => assignee.Id == assigneeId);

        chore = await choreRepository.UpdateAsync(chore);

        return chore;
    }

    public async Task<Chore?> UpdateAssignees(string choreId, List<User> assignees)
    {
        var chore = await choreRepository.GetByIdAsync(choreId) ?? throw new NotFoundException<Chore>(choreId);
        chore.Assignees = assignees;
        chore = await choreRepository.UpdateAsync(chore);

        return chore;
    }

    public async Task<Chore?> MarkAsDone(string choreId)
    {
        var chore = await choreRepository.GetByIdAsync(choreId) ?? throw new NotFoundException<Chore>(choreId);
        chore.LastDone = DateTime.UtcNow;

        chore = await choreRepository.UpdateAsync(chore);

        return chore;
    }
}