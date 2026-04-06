using KucniSavetBackend.Persistance.Documents;

namespace KucniSavetBackend.Mappers;

public static class RavenIdMapper
{
    public static string HouseholdId(string key) => $"{nameof(HouseholdDocument)}s/{key}";
    public static string UserId(string key) => $"{nameof(UserDocument)}s/{key}";
    public static string ChoreId(string key) => $"{nameof(ChoreDocument)}s/{key}";
}