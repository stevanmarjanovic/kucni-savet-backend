namespace KucniSavetBackend.DTO.Responses;

public class UserResponse
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Image => $"/user/{Id}/image";
    public HouseholdResponse? Household { get; set; } = new();
}