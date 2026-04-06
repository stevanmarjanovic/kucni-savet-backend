namespace KucniSavetBackend.Domain;

public class ProfileImage
{
    public required Stream Stream { get; set; }
    public string ContentType { get; set; } = "application/octet-stream";
}