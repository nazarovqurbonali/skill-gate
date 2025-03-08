namespace Application.Extensions.File;

public class FileService(
    string webRootPath,
    IOptions<FileSettings> fileSettings,
    ILogger<FileService> logger) : IFileService
{
    private readonly FileSettings _fileSettings = fileSettings.Value;

    public async Task<Result<string>> CreateFile(IFormFile file, string folder, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        
        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!_fileSettings.AllowedExtensions.Contains(extension))
        {
            logger.LogWarning($"Invalid file type attempted: {file.FileName}");
            return Result<string>.Failure(ResultPatternError.BadRequest("Invalid file type."));
        }

        if (file.Length > _fileSettings.MaxFileSize)
        {
            logger.LogWarning($"File size exceeds limit: {file.FileName}, Size: {file.Length}");
            return Result<string>.Failure(ResultPatternError.BadRequest(
                $"File size exceeds the maximum allowed size of {_fileSettings.MaxFileSize} bytes."));
        }

        string fileName = $"{Guid.NewGuid()}{extension}";
        string folderPath = Path.Combine(webRootPath, folder);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            logger.LogInformation($"Created directory: {folderPath}");
        }

        string fullPath = Path.Combine(folderPath, fileName);

        try
        {
            await using FileStream stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, token);

            logger.LogInformation($"File uploaded successfully: {fileName}");
            return Result<string>.Success(fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while saving the file.");
            return Result<string>.Failure(
                ResultPatternError.InternalServerError("An error occurred while saving the file.", ex.Message));
        }
    }

    public Result<bool> DeleteFile(string file, string folder, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        string folderPath = Path.Combine(webRootPath, folder);
        string fullPath = Path.Combine(folderPath, file);

        try
        {
            if (!Directory.Exists(folderPath))
            {
                logger.LogWarning($"Directory does not exist: {folderPath}");
                return Result<bool>.Failure(ResultPatternError.BadRequest("Directory does not exist."));
            }

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                logger.LogInformation($"File deleted successfully: {fullPath}");
                return Result<bool>.Success(true);
            }

            logger.LogWarning($"File not found: {fullPath}");
            return Result<bool>.Failure(ResultPatternError.NotFound("File not found."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting the file.");
            return Result<bool>.Failure(
                ResultPatternError.InternalServerError("An error occurred while deleting the file.", ex.Message));
        }
    }
}