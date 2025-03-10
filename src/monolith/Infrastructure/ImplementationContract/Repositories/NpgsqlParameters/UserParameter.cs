namespace Infrastructure.ImplementationContract.Repositories.NpgsqlParameters;

public static class UserParameter
{
    public static void AddUserParameters(this NpgsqlCommand command, User entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@FirstName", entity.FirstName.ToDbValue());
        command.Parameters.AddWithValue("@LastName", entity.LastName.ToDbValue());
        command.Parameters.AddWithValue("@Email", entity.Email);
        command.Parameters.AddWithValue("@PhoneNumber", entity.PhoneNumber);
        command.Parameters.AddWithValue("@UserName", entity.UserName);
        command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
        command.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
        command.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy.ToDbValue());
        command.Parameters.AddWithValue("@CreatedByIp", entity.CreatedByIp.ToDbValue());
    }

    public static void AddUpdateUserParameters(this NpgsqlCommand command, User entity)
    {
        command.Parameters.AddWithValue("@Id", entity.Id);
        command.Parameters.AddWithValue("@FirstName", entity.FirstName.ToDbValue());
        command.Parameters.AddWithValue("@LastName", entity.LastName.ToDbValue());
        command.Parameters.AddWithValue("@Email", entity.Email);
        command.Parameters.AddWithValue("@PhoneNumber", entity.PhoneNumber);
        command.Parameters.AddWithValue("@UserName", entity.UserName);
        command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
        command.Parameters.AddWithValue("@Status", entity.Status);
        command.Parameters.AddWithValue("@UpdatedAt", entity.UpdatedAt.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedBy", entity.UpdatedBy.ToDbValue());
        command.Parameters.AddWithValue("@UpdatedByIp", entity.UpdatedByIp.ToDbValue());
    }
}