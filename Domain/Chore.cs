using KucniSavetBackend.Enums;

namespace KucniSavetBackend.Domain;

public class Chore
{
    public string? Id { get; init; }
    public required Household Household { get; init; }
    public required string Name { get; set; }
    public required Frequency Frequency { get; set; }
    public DateTime? LastDone { get; set; }
    public bool ToDo => LastDone is null || (DateTime.UtcNow - LastDone.Value).TotalDays > (int)Frequency;
    public List<User> Assignees { get; set; } = [];
}