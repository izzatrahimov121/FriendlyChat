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
}
