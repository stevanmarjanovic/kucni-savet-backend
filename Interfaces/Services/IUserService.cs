using KucniSavetBackend.Domain;

namespace KucniSavetBackend.Interfaces.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(string id);
    Task<List<User>> GetAllByHouseholdIdAsync(string householdId);
    Task<ProfileImage?> GetProfileImageAsync(string userId);
    Task<User?> GetByFacebookIdAsync(string facebookId);
    Task<User?> CreateAsync(User user);
    Task<User?> CreateWithInviteCodeAsync(string name, string householdId);
}