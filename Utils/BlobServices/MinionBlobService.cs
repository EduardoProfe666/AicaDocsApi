using AicaDocsApi.Models;
using Minio;
using Minio.DataModel.Args;

namespace AicaDocsApi.Utils.BlobServices;

public class MinionBlobService(string bucketName, IMinioClient minioClient) : IBlobService
{
    public async Task UploadObject(IFormFile file, string codeObj, CancellationToken ct)
    {
        var isPdf = file.ContentType == "application/pdf";
        var folder = isPdf ? "pdf" : "word";
        var ext = isPdf ? "pdf" : "docx";

        await using var fileStream = file.OpenReadStream();
        var poaPdf = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject($"/{folder}/{codeObj}.{ext}")
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(file.ContentType);
        await minioClient.PutObjectAsync(poaPdf, ct);
    }

    public async Task<string> PresignedGetUrl(string objPath, CancellationToken ct)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objPath)
            .WithExpiry(60 * 5);
        return await minioClient.PresignedGetObjectAsync(args);
    }

    public async Task<bool> ValidateExistanceObject(string objPath, CancellationToken ct)
    {
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objPath);

            await minioClient.StatObjectAsync(statObjectArgs, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }
}