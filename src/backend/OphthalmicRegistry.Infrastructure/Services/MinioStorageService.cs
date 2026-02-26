using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using OphthalmicRegistry.Application.Common.Interfaces;

namespace OphthalmicRegistry.Infrastructure.Services;

/// <summary>Stores native imaging files in MinIO (S3-compatible) object storage.</summary>
public class MinioStorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public MinioStorageService(IAmazonS3 s3Client, IConfiguration configuration)
    {
        _s3Client = s3Client;
        _bucketName = configuration["Minio:BucketName"] ?? "ophthalmic-imaging";
    }

    public async Task UploadAsync(Stream stream, string objectKey, string contentType, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            InputStream = stream,
            ContentType = contentType,
            AutoCloseStream = false,
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
    }
}
