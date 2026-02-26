namespace OphthalmicRegistry.Application.Common.Interfaces;

/// <summary>Manages uploads and retrieval of DICOM studies in Orthanc.</summary>
public interface IOrthancService
{
    /// <summary>
    /// Uploads a DICOM file to the Orthanc server.
    /// </summary>
    /// <param name="dicomStream">Stream containing the DICOM file data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The Orthanc study ID assigned to the uploaded study.</returns>
    Task<string> UploadDicomAsync(Stream dicomStream, CancellationToken cancellationToken = default);
}
