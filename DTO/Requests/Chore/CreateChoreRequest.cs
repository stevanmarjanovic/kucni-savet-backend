using KucniSavetBackend.Enums;

namespace KucniSavetBackend.DTO.Requests.Chore;

public class CreateChoreRequest
{
    public required string Name { get; set; }
    public required Frequency Frequency { get; set; }
}