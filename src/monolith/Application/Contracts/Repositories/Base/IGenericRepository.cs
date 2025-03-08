namespace Application.Contracts.Repositories.Base;

public interface IGenericRepository<T> :
    IAdd<T>,
    IUpdate<T>,
    IRemove<T>,
    IFind<T> where T : BaseEntity;