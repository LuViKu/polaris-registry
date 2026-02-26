using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OphthalmicRegistry.Application.Common.Interfaces;

namespace OphthalmicRegistry.Infrastructure.Services;

/// <summary>
/// Calls the OCT-Converter microservice to convert proprietary ophthalmic image formats to DICOM.
/// Returns null when conversion is unavailable or the service is unreachable.
/// </summary>
public class OctConverterService : IOctConverterService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OctConverterService> _logger;

    public OctConverterService(HttpClient httpClient, ILogger<OctConverterService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Stream?> ConvertToDicomAsync(Stream inputStream, string fileExtension, CancellationToken cancellationToken = default)
    {
        try
        {
            using var formContent = new MultipartFormDataContent();
            using var fileContent = new StreamContent(inputStream);
            formContent.Add(fileContent, "file", $"upload.{fileExtension.TrimStart('.')}");
            formContent.Add(new StringContent(fileExtension.TrimStart('.')), "format");

            var response = await _httpClient.PostAsync("convert", formContent, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OCT-Converter returned {StatusCode} for format {Format}", response.StatusCode, fileExtension);
                return null;
            }

            // Read into a MemoryStream so the caller owns the lifetime
            var ms = new MemoryStream();
            await response.Content.CopyToAsync(ms, cancellationToken);
            ms.Position = 0;
            return ms;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OCT-Converter service unavailable; skipping DICOM conversion for format {Format}", fileExtension);
            return null;
        }
    }
}
