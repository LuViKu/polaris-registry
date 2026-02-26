using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OphthalmicRegistry.Application.Common.Interfaces;

namespace OphthalmicRegistry.Infrastructure.Services;

/// <summary>Uploads DICOM files to an Orthanc PACS server via its REST API.</summary>
public class OrthancService : IOrthancService
{
    private readonly HttpClient _httpClient;

    public OrthancService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<string> UploadDicomAsync(Stream dicomStream, CancellationToken cancellationToken = default)
    {
        using var content = new StreamContent(dicomStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/dicom");

        var response = await _httpClient.PostAsync("instances", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        // Orthanc returns {"ID":"<instance-id>","Path":"/instances/<id>","Status":"Success","ParentStudy":"<study-id>"}
        if (doc.RootElement.TryGetProperty("ParentStudy", out var studyProp))
            return studyProp.GetString() ?? string.Empty;

        // Fall back to the instance ID
        return doc.RootElement.GetProperty("ID").GetString() ?? string.Empty;
    }
}
