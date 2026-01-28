using Microsoft.Extensions.Options;
using System.IO;
using tavern_api.Commons.Configs;
using tavern_api.Commons.Contracts.Services;
using tavern_api.Commons.Exceptions;
using tavern_api.Commons.Responses;

namespace tavern_api.Services;

internal sealed class FileService : IFileService
{
    private readonly long LIMIT_IMAGE_SIZE = 2 * 1024 * 1024;
    private readonly BaseUrlConfig _baseUrlConfig;
    private readonly IWebHostEnvironment _env;
    private readonly BaseTempUrlPath _uploadDiskPath;

    private readonly List<string> ALLOWED_FILE_EXTENSIONS = new() 
    { 
        ".pdf",
    };

    public FileService(IOptions<BaseUrlConfig> baseUrlConfig, IWebHostEnvironment env, IOptions<BaseTempUrlPath> uploadDiskPath)
    {
        _baseUrlConfig = baseUrlConfig.Value;
        _env = env;
        _uploadDiskPath = uploadDiskPath.Value;
    }

    public async Task<Result<byte[]>> GetFileBytesAsync(string itemId)
    {
        try
        {
            var fullFileName = itemId + ".pdf";
            var fullFilePath = Path.Combine(_uploadDiskPath.Value, fullFileName);

            if (!File.Exists(fullFilePath))
                return new Result<byte[]>().Failure("Arquivo não encontrado no servidor.", null, 404);

            var fileBytes = File.ReadAllBytes(fullFilePath);

            return new Result<byte[]>().Success("Arquivo encontrado", fileBytes, 200);
        }
        catch (Exception ex)
        {
            throw new FileException("Ocorreu um problema com seu arquivo. Tente novamente mais tarde. Se o problema persistir contate o suporte");
        }
    }

    public async Task<Result<string>> CreateFileFromWebUpload(Stream stream, string fileName, string fileId)
    {
        try
        {

            var fileExtension = Path.GetExtension(fileName);
            var fileOnlyName = Path.GetFileNameWithoutExtension(fileName);

            if (!ALLOWED_FILE_EXTENSIONS.Contains(fileExtension.ToLower()))
                return new Result<string>().Failure("Tipo de arquivo não permitido", null, 400);

            if (stream.Length < LIMIT_IMAGE_SIZE)
                return new Result<string>().Failure("Arquivo maior do que esperado", null, 400);

            if (!Directory.Exists(_uploadDiskPath.Value))
            {
                Directory.CreateDirectory(_uploadDiskPath.Value);
            }

            var fullFileName = fileId + fileExtension;

            var newFilePath = Path.Combine(_uploadDiskPath.Value, fullFileName);

            using (var st = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.Position = 0;
                await stream.CopyToAsync(st);
            }

            return new Result<string>().Success("Arquivo salvo com sucesso", fileId, 201);

        } catch (Exception ex)
        {
            throw new FileException("Ocorreu um problema com seu arquivo. Tente novamente mais tarde. Se o problema persistir contate o suporte");
        }
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

    public async Task<Result<string>> DeleteItemFromDisk(string itemId)
    {
        try
        {
            var fullFileName = itemId + ".pdf";
            var fullFilePath = Path.Combine(_uploadDiskPath.Value + fullFileName);

            if (!File.Exists(fullFilePath))
                return new Result<string>().Success("Arquivo já deletado do servidor", null, 201);

            File.Delete(fullFilePath);

            return new Result<string>().Success("Arquivo deletado com sucesso", null, 201);

        }
        catch (Exception ex)
        {
            throw new FileException("Ocorreu um problema com seu arquivo. Tente novamente mais tarde. Se o problema persistir contate o suporte");
        }
    }
}
