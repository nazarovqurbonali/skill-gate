namespace Application.Extensions.File;

public interface IFileService
{
    Task<Result<string>> CreateFile(IFormFile file, string folder,CancellationToken token=default);
    Result<bool> DeleteFile(string file, string folder,CancellationToken token=default);
}