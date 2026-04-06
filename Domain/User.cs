namespace KucniSavetBackend.Domain;

public class User
{
    public string? Id { get; init; }
    public string? Name { get; set; }
    public string? FacebookId { get; set; }
    public Household? Household { get; init; }
    public string? InviteCode { get; set; }
}
