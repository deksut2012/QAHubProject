namespace QAHub.Api.Domain.Identity;

public sealed class UserAccount
{
    private UserAccount() { }
    public UserAccount(string externalId, string displayName, string? email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(externalId);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        Id = Guid.NewGuid(); ExternalId = externalId.Trim(); DisplayName = displayName.Trim();
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(); IsActive = true;
    }
    public Guid Id { get; private set; }
    public string ExternalId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<UserRole> Roles { get; private set; } = [];
}
