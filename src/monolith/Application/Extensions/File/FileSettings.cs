namespace Application.Extensions.File;

public sealed class FileSettings
{
    public long MaxFileSize { get; set; }
    public List<string> AllowedExtensions { get; set; } =[];
}