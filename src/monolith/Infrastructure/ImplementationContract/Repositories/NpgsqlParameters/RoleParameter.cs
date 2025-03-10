namespace Infrastructure.ImplementationContract.Repositories.NpgsqlParameters;

public static class RoleParameter
{
    public static void AddRoleParameters(this NpgsqlCommand command, Role entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@RoleKey", entity.RoleKey);
        command.Parameters.AddWithValue("@Description", entity.Description.ToDbValue());
        command.Parameters.AddWithValue("@Status", entity.Status);
        command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
        command.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy.ToDbValue());
        command.Parameters.AddWithValue("@CreatedByIp", entity.CreatedByIp.ToDbValue());
    }

    public static void AddUpdateRoleParameters(this NpgsqlCommand command, Role entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@Name", entity.Name);
        command.Parameters.AddWithValue("@RoleKey", entity.RoleKey);
        command.Parameters.AddWithValue("@Description", entity.Description.ToDbValue());
        command.Parameters.AddWithValue("@Status", entity.Status);
        command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedByIp", entity.UpdatedByIp.ToDbValue());
    }

    public static void AddDeleteRoleParameters(this NpgsqlCommand command, Guid roleId)
    {
        command.Parameters.AddWithValue("@Id", roleId);
    }
}