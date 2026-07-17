namespace QAHub.Api.Domain.Identity;

public sealed class UserRole
{
    private UserRole() { }
    public UserRole(Guid userId, Guid roleId)
    {
        if (userId == Guid.Empty || roleId == Guid.Empty) throw new ArgumentException("User and role are required.");
        UserId = userId; RoleId = roleId; AssignedAtUtc = DateTimeOffset.UtcNow;
    }
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public DateTimeOffset AssignedAtUtc { get; private set; }
    public UserAccount User { get; private set; } = null!;
    public AppRole Role { get; private set; } = null!;
}
