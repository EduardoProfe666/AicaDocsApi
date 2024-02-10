namespace AicaDocsApi.Utils;

public class BucketNameProvider(string bucketName)
{
    public string BucketName { get; } = bucketName;
}