namespace digital_library.Models;

public class User
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }

    /// <summary>Null for accounts created via an external provider (e.g. Google).</summary>
    public string? PasswordHash { get; set; }

    /// <summary>Google "sub" identifier, set when the account is linked to Google.</summary>
    public string? GoogleId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
