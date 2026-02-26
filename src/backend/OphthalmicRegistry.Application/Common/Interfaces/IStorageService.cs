namespace OphthalmicRegistry.Application.Common.Interfaces;

/// <summary>Stores and retrieves native (non-DICOM) imaging files in object storage.</summary>
public interface IStorageService
{
    /// <summary>
    /// Uploads a file to object storage and returns its object key.
    /// </summary>
    /// <param name="stream">File content stream.</param>
    /// <param name="objectKey">Destination key/path within the bucket.</param>
    /// <param name="contentType">MIME content type of the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UploadAsync(Stream stream, string objectKey, string contentType, CancellationToken cancellationToken = default);
}
