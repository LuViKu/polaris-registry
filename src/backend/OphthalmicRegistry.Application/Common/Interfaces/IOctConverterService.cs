namespace OphthalmicRegistry.Application.Common.Interfaces;

/// <summary>Converts proprietary ophthalmic image formats to DICOM via the OCT-Converter service.</summary>
public interface IOctConverterService
{
    /// <summary>
    /// Converts a native ophthalmic image file to DICOM format.
    /// </summary>
    /// <param name="inputStream">Stream of the native format file.</param>
    /// <param name="fileExtension">Original file extension (e.g. "e2e", "fds", "img").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Stream containing the converted DICOM data, or null if conversion is not supported.</returns>
    Task<Stream?> ConvertToDicomAsync(Stream inputStream, string fileExtension, CancellationToken cancellationToken = default);
}
