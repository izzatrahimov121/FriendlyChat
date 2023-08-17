namespace Chat.MVC.HelperServices.Interfaces;

public interface IFileService
{
    Task<string> CopyFileAsync(IFormFile file, string wwwroot, params string[] folders);
}
