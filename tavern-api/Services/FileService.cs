using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Options;
using tavern_api.Commons.Configs;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.Exceptions;

namespace tavern_api.Services;

internal sealed class FileService : IFileService
{
    private readonly long LIMIT_IMAGE_SIZE = 2 * 1024 * 1024;
    private readonly BaseUrlConfig _baseUrlConfig;
    private readonly IWebHostEnvironment _env;

    public FileService(IOptions<BaseUrlConfig> baseUrlConfig, IWebHostEnvironment env)
    {
        _baseUrlConfig = baseUrlConfig.Value;
        _env = env;
    }

    public async Task<string> SaveUserImage(Stream fileStream, string fileExtension, string userId)
    {
        try
        {
            var baseUrlProfile = Path.Combine(_env.WebRootPath, "images", "profiles");

            var fullFilePath = Path.Combine(baseUrlProfile + "\\", $"{userId}{fileExtension}");

            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            using (var stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
            {
                fileStream.Position = 0;
                await fileStream.CopyToAsync(stream);
            }

            var returnUserProfileImageUrl = $"{_baseUrlConfig.Url}images/profiles/{userId}{fileExtension}";
            return returnUserProfileImageUrl;
        }
        catch (Exception ex) 
        {
            throw;
        }
    }

    public Task ValidateImageSizeInBytes(long bytes)
    {
        if (bytes > LIMIT_IMAGE_SIZE)
            throw new FileException("Tamanho da imagem não permitido. (max 2MB)");

        return Task.CompletedTask;
    }
}
