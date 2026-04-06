namespace KucniSavetBackend.DTO.Requests.Chore;

public class UpdateChoreAssigneesRequest
{
    public required List<Domain.User> Assignees { get; set; }
}