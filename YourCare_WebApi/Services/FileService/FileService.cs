using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace YourCare_WebApi.Services.FileService
{
    public interface IFileService
    {
        Task<List<string>> SaveFilesAsync(List<IFormFile> formFiles, string folder);
        Task<string?> SaveFileAsync(IFormFile file, string folder);
        Task DeleteFileAsync(List<string> imagePaths);
    }

    public class FileService : IFileService
    {
        public async Task<string?> SaveFileAsync(IFormFile file, string folder)
        {
            if (file.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), folder, fileName);
                await SaveToFileAsync(file, path);
                return Path.Combine("/" + folder, fileName);
            }
            return null;
        }

        public async Task<List<string>> SaveFilesAsync(List<IFormFile> formFiles, string folder)
        {
            var tasks = formFiles
                .Where(x => x.Length > 0)
                .Select(x => SaveFileAsync(x, folder));

            var listPath = await Task.WhenAll(tasks);
            return listPath.Where(x => x != null).Select(x => x!).ToList();

        }
        public Task DeleteFileAsync(List<string> imagePaths)
        {
            foreach (var path in imagePaths)
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            return Task.CompletedTask;
        }

        private async Task SaveToFileAsync(IFormFile file, string path)
        {
            const int limitSize = 102400; // 100 KB
            if (file.ContentType.StartsWith("image/") && file.Length > limitSize) //reduce image quality
            {
                using var image = await Image.LoadAsync(file.OpenReadStream());
                var quality = 30;
                var encoder = new JpegEncoder { Quality = quality };
                await image.SaveAsync(path, encoder);
            }
            else
            {
                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);
            }
            
        }
    }
}
