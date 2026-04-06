using KucniSavetBackend.Domain;

namespace KucniSavetBackend.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(string key);
    Task<List<User>> GetAllByHouseholdIdAsync(string householdId);
    Task<User?> GetByFacebookIdAsync(string facebookId);
    Task<User?> GeyByInviteCodeAsync(string inviteCode);
    Task<ProfileImage?> GetUserImageById(string key);
    Task<User?> CreateAsync(User user, string? imageBase64 = null);
    Task<User?> UpdateAsync(User user);
    Task DeleteAsync(string key);
    Task DeleteAsync(User user);
}