using KucniSavetBackend.Domain;
using KucniSavetBackend.Enums;

namespace KucniSavetBackend.Interfaces.Services;

public interface IChoreService
{
    Task<Chore?> GetByIdAsync(string id);
    Task<List<Chore>> GetByHouseholdIdAsync(string householdId);
    Task<Chore?> CreateAsync(string name, Frequency frequency, string householdId);
    Task<Chore?> UpdateAsync(string id, string name, Frequency frequency);
    Task<Chore?> AddAssignee(string choreId, string assigneeId);
    Task<Chore?> RemoveAssignee(string choreId, string assigneeId);
    Task<Chore?> UpdateAssignees(string choreId, List<User> assignees);
    Task<Chore?> MarkAsDone(string choreId);
}