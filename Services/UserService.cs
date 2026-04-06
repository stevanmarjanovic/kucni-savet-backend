using KucniSavetBackend.Domain;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Interfaces.Services;

namespace KucniSavetBackend.Services;

public class UserService(IUserRepository userRepository, IHouseholdRepository householdRepository, HttpClient client) : IUserService
{
    private const string DiceBearUri = "https://api.dicebear.com/9.x/croodles/jpg";

    public async Task<User?> CreateAsync(User? user)
    {
        // Do validation at the beginning
        if (user is null)
            return null;
        
        user = await userRepository.CreateAsync(user);

        return user;
    }

    public async Task<User?> CreateWithInviteCodeAsync(string name, string householdId)
    {
        var household = await householdRepository.GetByIdAsync(householdId);

        if (household is null)
            return null;

        var imageBytes = await client.GetByteArrayAsync($"{DiceBearUri}?size=512&seed={name}");
        var imageString = Convert.ToBase64String(imageBytes);

        var user = new User
        {
            Name = name,
            Household = household,
            InviteCode = Guid.NewGuid().ToString()
        };

        user = await userRepository.CreateAsync(user, imageString);

        return user;
    }

    public Task<List<User>> GetAllByHouseholdIdAsync(string householdId)
    {
        var users = userRepository.GetAllByHouseholdIdAsync(householdId);
        return users;
    }

    public async Task<ProfileImage?> GetProfileImageAsync(string userId)
    {
        return await userRepository.GetUserImageById(userId);
    }

    public async Task<User?> GetByFacebookIdAsync(string facebookId)
    {
        var user = await userRepository.GetByFacebookIdAsync(facebookId);
        return user;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        var user = await userRepository.GetByIdAsync(id);
        return user;
    }
}