using KucniSavetBackend.Domain;
using KucniSavetBackend.DTO.Responses;
using KucniSavetBackend.Persistance.Documents;

namespace KucniSavetBackend.Mappers;

public static class UserMapper
{
    public static UserResponse ToResponse(this User user) {
        return new UserResponse
        {
            Id = user.Id ?? "",
            Name = user?.Name ?? "",
            Household = user?.Household?.ToResponse()
        };
    }

    public static User ToDomain(this UserDocument user, HouseholdDocument? household = null)
    {
        return new User
        {
            Id = user.Id.Split('/').Last(),
            Name = user.Name,
            Household = household?.ToDomain()
        };
    }
}