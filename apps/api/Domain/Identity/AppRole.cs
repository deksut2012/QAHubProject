namespace QAHub.Api.Domain.Identity;

public sealed class AppRole
{
    private AppRole() { }
    public AppRole(string code, string name, bool isSystem = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code); ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = Guid.NewGuid(); Code = code.Trim(); Name = name.Trim(); IsSystem = isSystem;
    }
    public Guid Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsSystem { get; private set; }
    public ICollection<UserRole> Users { get; private set; } = [];
}
