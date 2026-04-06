namespace KucniSavetBackend.Persistance.Documents;

public class UserDocument
{
    public string Id { get; } = null!;
    public string Image { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string HouseholdId { get; init; } = null!;
    public string FacebookId { get ; set; } = null!;
    public string InviteCode { get; set; } = null!;
}