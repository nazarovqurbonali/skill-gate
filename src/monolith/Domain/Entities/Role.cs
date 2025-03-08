namespace Domain.Entities;

public sealed class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string RoleKey { get; set; } = string.Empty;
    public string? Description { get; set; }
}