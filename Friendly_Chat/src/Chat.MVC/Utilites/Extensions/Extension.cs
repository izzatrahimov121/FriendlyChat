namespace Chat.MVC.Utilites.Extensions;

public static class Extension
{
    public static bool CheckFileFormat(this IFormFile file, string format)
    {
        return file.ContentType.Contains(format);
    }

    public static bool CheckFileSize(this IFormFile file, int fileSize)
    {
        return file.Length / 1024 < fileSize;
    }
}
