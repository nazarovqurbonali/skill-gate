namespace Application.Contracts.Repositories;

public interface IUserRoleRepository : IGenericRepository<UserRole>
{
    Task<Result<IEnumerable<UserRole>>> GetAllAsync(UserRoleFilter filter, CancellationToken token = default);
    Task<Result<int>> GetCountAsync(UserRoleFilter filter, CancellationToken token = default);

}