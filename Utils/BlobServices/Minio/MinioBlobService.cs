using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace AicaDocsApi.Utils.BlobServices.Minio;

public class MinioBlobService : IBlobService
{
    private readonly string _bucketName;
    private readonly IMinioClient _minioClient;

    public MinioBlobService(IOptions<MinioOptions> options)
    {
        _bucketName = options.Value.Bucket;
        _minioClient = new MinioClient()
            .WithEndpoint(options.Value.Endpoint)
            .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
            .WithSSL(true)
            .Build();
    }

    public async Task UploadObject(IFormFile file, string codeObj, CancellationToken ct)
    {
        var isPdf = file.ContentType == "application/pdf";
        var folder = isPdf ? "pdf" : "word";
        var ext = isPdf ? "pdf" : "docx";

        await using var fileStream = file.OpenReadStream();
        var poaPdf = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject($"/{folder}/{codeObj}.{ext}")
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(file.ContentType);
        await _minioClient.PutObjectAsync(poaPdf, ct);
    }

    public async Task<string> PresignedGetUrl(string objPath, CancellationToken ct)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objPath)
            .WithExpiry(60 * 5);
        return await _minioClient.PresignedGetObjectAsync(args);
    }

    public async Task<bool> ValidateExistanceObject(string objPath, CancellationToken ct)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objPath);

            await _minioClient.StatObjectAsync(statObjectArgs, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
