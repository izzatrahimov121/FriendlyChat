using Chat.MVC.Exceptions;
using Chat.MVC.HelperServices.Interfaces;
using Chat.MVC.Utilites.Extensions;

namespace Chat.MVC.HelperServices.Implementations;

public class FileService : IFileService
{
    public async Task<string> CopyFileAsync(IFormFile file, string wwwroot, params string[] folders)
    {
        string fileName = string.Empty;

        if (file is not null)
        {
            if (!file.CheckFileFormat("image/"))
            {
                throw new IncorrectFileFormatException("Please select 'Image' type files only");
            }
            if (!file.CheckFileSize(10000))
            {
                throw new IncorrectFileFormatException("Image size is larger than 10 Mb");
            }
            fileName = Guid.NewGuid().ToString() + file.FileName;

            string resultPath = wwwroot;

            foreach (var folder in folders)
            {
                resultPath = Path.Combine(resultPath, folder);
            }
            resultPath = Path.Combine(resultPath, fileName);

            using (FileStream stream = new FileStream(resultPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
        throw new Exception();
    }

    public async Task<string> CopyFileAsyncForMedia(IFormFile file, string wwwroot, params string[] folders)
    {
        var fileName = String.Empty;
        if (file is not null)
        {
            if (!file.CheckFileFormat("image/") || !file.CheckFileFormat("video/"))
            {
                throw new IncorrectFileFormatException("Please select 'Image' or 'Video' type files only");
            }
            if (!file.CheckFileSize(100000))
            {
                throw new IncorrectFileFormatException("File size is larger than 100 Mb");
            }
            fileName = Guid.NewGuid().ToString() + file.FileName;
            string resultPath = wwwroot;
            foreach (var folder in folders)
            {
                resultPath = Path.Combine(resultPath, folder);
            }
            resultPath = Path.Combine(resultPath, fileName);

            using (FileStream stream = new FileStream(resultPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
        throw new Exception("Unexpected error has occurred");
    }
}
