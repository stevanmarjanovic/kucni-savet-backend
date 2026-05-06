using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KucniSavetBackend.Domain;
using KucniSavetBackend.DTO.Responses;
using KucniSavetBackend.Exceptions;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;

namespace KucniSavetBackend.Services;

public class AuthService(IUserRepository userRepository, IHouseholdRepository householdRepository, IConfiguration configuration, HttpClient client) : IAuthService
{
    private const string FacebookUri = "https://graph.facebook.com/v25.0";
    private const string DiceBearUri = "https://api.dicebear.com/9.x/croodles/png";

    public string GenerateJwt(User user)
    {
        if (user is null)
            throw new OperationalException("Invalid user");

        var secret =  configuration["Jwt:Secret"] ?? throw new OperationalException("Invalid secret");
        
        var key = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(secret)
        );
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("userId", user.Id),
            new Claim("householdId", user.Household.Id)
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User?> LoginOrRegisterWithFacebookAsync(string accessToken)
    {
        if (accessToken == "")
            return null;

        FacebookMeResponse? facebookUser;
        try
        {
            facebookUser = await client.GetFromJsonAsync<FacebookMeResponse>($"{FacebookUri}/me?access_token={Uri.EscapeDataString(accessToken)}");

            if (facebookUser is null)
                return null;
        } catch
        {
            throw new Exception("Invalid Facebook Token");
        }

        var user = await userRepository.GetByFacebookIdAsync(facebookUser.Id);
        
        if (user is not null) return user;
        
        // Create user
        var household = new Household
        {
            Name = $"{facebookUser.Name}'s Household"
        };
        household = await householdRepository.CreateAsync(household);

        var imageBytes = await client.GetByteArrayAsync($"{DiceBearUri}?size=512&seed={facebookUser.Name}");
        var imageString = Convert.ToBase64String(imageBytes);

        user = new User
        {
            FacebookId = facebookUser.Id,
            Name = facebookUser.Name,
            Household = household
        };

        user = await userRepository.CreateAsync(user, imageString);

        return user;
    }

    public async Task<User?> RegisterWithFacebookAndInviteCodeAsync(string accessToken, string inviteCode)
    {
        var user = await userRepository.GeyByInviteCodeAsync(inviteCode);

        if (user is null)
            return null;

        FacebookMeResponse? facebookUser;
        try
        {
            facebookUser = await client.GetFromJsonAsync<FacebookMeResponse>($"{FacebookUri}/me?access_token={Uri.EscapeDataString(accessToken)}");

            if (facebookUser is null)
                return null;
        } catch
        {
            throw new Exception("Invalid Facebook Token");
        }

        user.Name = facebookUser.Name;
        user.FacebookId = facebookUser.Id;
        user.InviteCode = null;

        user = await userRepository.UpdateAsync(user);

        return user;
    }
}