using KucniSavetBackend.Domain;
using KucniSavetBackend.Interfaces.Repositories;
using KucniSavetBackend.Mappers;
using KucniSavetBackend.Persistance.Documents;
using Microsoft.AspNetCore.Http.Metadata;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace KucniSavetBackend.Repositories.RavenDB;

public class UserRepository(IAsyncDocumentSession session) : IUserRepository
{
    private const string ProfileImageFileName = "profile.png";
    public async Task<User?> GetByIdAsync(string key)
    {
        var id = RavenIdMapper.UserId(key);

        var doc = await session.LoadAsync<UserDocument>(id);

        if (doc is null)
            return null;
        
        var household = await session.LoadAsync<HouseholdDocument>(doc.HouseholdId);

        return doc.ToDomain(household);
    }

    public async Task<List<User>> GetAllByHouseholdIdAsync(string householdId)
    {
        householdId = RavenIdMapper.HouseholdId(householdId);
        
        var docs = await session.Query<UserDocument>()
            .Include(householdId)
            .Where(user => user.HouseholdId == householdId)
            .ToListAsync();
        
        var household = await session.LoadAsync<HouseholdDocument>(householdId);

        return docs.Select(doc => doc.ToDomain(household)).ToList();
    }

    public async Task<User?> GetByFacebookIdAsync(string facebookId)
    {
        var doc = await session.Query<UserDocument>()
            .Include(user => user.HouseholdId)
            .Where(user => user.FacebookId == facebookId)
            .FirstOrDefaultAsync();

        if (doc is null) return null;

        var household = await session.LoadAsync<HouseholdDocument>(doc.HouseholdId);

        return doc.ToDomain(household);
    }

    public async Task<User?> GeyByInviteCodeAsync(string inviteCode)
    {
        var doc = await session.Query<UserDocument>()
            .Include(user => user.HouseholdId)
            .Where(user => user.InviteCode == inviteCode)
            .FirstOrDefaultAsync();

        if (doc is null)
            return null;

        var household = await session.LoadAsync<HouseholdDocument>(doc.HouseholdId);
        
        return doc.ToDomain(household);
    }

    public async Task<ProfileImage?> GetUserImageById(string key)
    {
        var userId = RavenIdMapper.UserId(key);
        var attachment = await session.Advanced.Attachments.GetAsync(userId, ProfileImageFileName);

        if (attachment is null)
            return null;

        return new ProfileImage
        {
            Stream = attachment.Stream,
            ContentType = attachment.Details.ContentType
        };
    }

    public async Task<User?> CreateAsync(User user, string? imageBase64 = null)
    {
        var doc = new UserDocument
        {
            Name = user.Name ?? "",
            FacebookId = user.FacebookId ?? "",
            HouseholdId = RavenIdMapper.HouseholdId(user.Household?.Id ?? ""),
            InviteCode = user.InviteCode ?? ""
        };
        
        await session.StoreAsync(doc);

        if (imageBase64 is not null)
        {
            var bytes = Convert.FromBase64String(imageBase64);
            var stream = new MemoryStream(bytes);
            session.Advanced.Attachments.Store(doc, ProfileImageFileName, stream, "image/jpeg");
        }
        var  household = await session.LoadAsync<HouseholdDocument>(doc.HouseholdId);

        
        await session.SaveChangesAsync();
        
        return doc.ToDomain(household);
    }

    public async Task<User?> UpdateAsync(User user)
    {
        var doc = await session.Include<UserDocument>(userDocument => userDocument.HouseholdId).LoadAsync<UserDocument>(user.Id);

        doc.Name = user.Name ?? doc.Name;
        doc.FacebookId = user.FacebookId ?? doc.FacebookId;
        doc.InviteCode = user.InviteCode ?? doc.InviteCode;

        await session.SaveChangesAsync();
        
        var household = await session.LoadAsync<HouseholdDocument>(doc.HouseholdId);

        return doc.ToDomain(household);
    }

    public async Task DeleteAsync(string key)
    {
        var id = RavenIdMapper.UserId(key);

        session.Delete(id);

        await session.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        if (user.Id is null)
            return;
        
        await DeleteAsync(user.Id);
    }
}