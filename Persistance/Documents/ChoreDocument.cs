using KucniSavetBackend.Enums;

namespace KucniSavetBackend.Persistance.Documents;

public class ChoreDocument
{
    public string Id { get; } = null!;
    public required string HouseholdId { get; init; }
    public required string Name { get; set; }
    public required Frequency Frequency { get; set; }
    public DateTime? LastDone { get; set; }
    public List<string> AssigneesIds { get; set; } = [];
}