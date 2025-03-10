namespace Infrastructure.ImplementationContract.Repositories.NpgsqlParameters;

public static class UserRoleParameter
{
    public static void AddUserRoleParameters(this NpgsqlCommand command, UserRole entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@UserId", entity.UserId);
        command.Parameters.AddWithValue("@RoleId", entity.RoleId);
        command.Parameters.AddWithValue("@Status", entity.Status);
        command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
        command.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy.ToDbValue());
        command.Parameters.AddWithValue("@CreatedByIp", entity.CreatedByIp.ToDbValue());
    }

    public static void AddUpdateUserRoleParameters(this NpgsqlCommand command, UserRole entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@UserId", entity.UserId);
        command.Parameters.AddWithValue("@RoleId", entity.RoleId);
        command.Parameters.AddWithValue("@Status", entity.Status);
        command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedByIp", entity.UpdatedByIp.ToDbValue());
    }

    public static void AddDeleteUserRoleParameters(this NpgsqlCommand command, Guid userRoleId)
    {
        command.Parameters.AddWithValue("@Id", userRoleId);
    }
}