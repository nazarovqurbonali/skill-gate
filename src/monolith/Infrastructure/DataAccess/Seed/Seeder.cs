namespace Infrastructure.DataAccess.Seed;

public static class Seeder
{
    
}

file static class SeedData
{
    public static readonly Guid SystemId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid AdminId = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid UserId = new("33333333-3333-3333-3333-333333333333");

    private static readonly Guid AdminRoleId = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid UserRoleId = new("22222222-2222-2222-2222-222222222222");

    private static readonly Guid UserRoleId1 = new("11111111-1111-1111-1111-111111111111");
    private static readonly Guid UserRoleId2 = new("22222222-2222-2222-2222-222222222222");
    private static readonly Guid UserRoleId3 = new("33333333-3333-3333-3333-333333333333");
    private static readonly Guid UserRoleId4 = new("44444444-4444-4444-4444-444444444444");
    private static readonly Guid UserRoleId5 = new("55555555-5555-5555-5555-555555555555");


    public static readonly List<Role> ListRoles =
    [
        new()
        {
            Id = AdminRoleId,
            Name = Roles.Admin,
            RoleKey = Roles.Admin,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
        new()
        {
            Id = UserRoleId,
            Name = Roles.User,
            RoleKey = Roles.User,
            CreatedBy = SystemId,
            CreatedByIp = "localhost"
        },
    ];

    public static readonly List<User> ListUsers =
    [
        new()
        {
            Id = SystemId,
            Email = "system@gmail.com",
            PhoneNumber = "+99200000000",
            UserName = "System",
            PasswordHash = HashingUtility.ComputeSha256Hash("11111111")
        },
        new()
        {
            Id = AdminId,
            Email = "admin@gmail.com",
            PhoneNumber = "+992001001001",
            UserName = "Admin",
            CreatedBy = SystemId,
            PasswordHash = HashingUtility.ComputeSha256Hash("12345678")
        },
        new()
        {
            Id = UserId,
            Email = "user@gmail.com",
            PhoneNumber = "+992002002002",
            UserName = "User",
            CreatedBy = SystemId,
            PasswordHash = HashingUtility.ComputeSha256Hash("43211234")
        },
    ];

    public static readonly List<UserRole> ListUserRoles =
    [
        new()
        {
            Id = UserRoleId1,
            UserId = AdminId,
            RoleId = AdminRoleId,
            CreatedBy = SystemId
        },
        new()
        {
            Id = UserRoleId2,
            UserId = AdminId,
            RoleId = UserRoleId,
            CreatedBy = SystemId
        },
        new()
        {
            Id = UserRoleId3,
            UserId = UserId,
            RoleId = UserRoleId,
            CreatedBy = SystemId
        },
        new()
        {
            Id = UserRoleId4,
            UserId = SystemId,
            RoleId = AdminRoleId,
            CreatedBy = SystemId
        },
        new()
        {
            Id = UserRoleId5,
            UserId = SystemId,
            RoleId = UserRoleId,
            CreatedBy = SystemId
        },
    ];
}