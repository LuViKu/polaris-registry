using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OphthalmicRegistry.Application.Imaging.Commands.UploadImagingStudy;
using OphthalmicRegistry.Domain.Repositories;

namespace OphthalmicRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IImagingStudyRepository _imagingStudyRepository;

    public ImagingController(IMediator mediator, IImagingStudyRepository imagingStudyRepository)
    {
        _mediator = mediator;
        _imagingStudyRepository = imagingStudyRepository;
    }

    /// <summary>
    /// Uploads an imaging study for a clinical visit.
    /// Accepts DICOM files (.dcm) or native ophthalmic formats (.e2e, .fds, .fda, .img, .sdb, .oct).
    /// DICOM files are stored in Orthanc; native files are stored in MinIO and converted when possible.
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequestSizeLimit(500 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] UploadImagingStudyRequest request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
            return BadRequest(new { error = "No file provided." });

        await using var stream = request.File.OpenReadStream();

        var acquiredAt = string.IsNullOrWhiteSpace(request.AcquiredAt)
            ? DateTimeOffset.UtcNow
            : DateTimeOffset.Parse(request.AcquiredAt);

        var command = new UploadImagingStudyCommand(
            request.ClinicalVisitId,
            request.Modality,
            stream,
            request.File.FileName,
            request.File.ContentType,
            acquiredAt);

        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ErrorCode == "VISIT_NOT_FOUND"
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetByVisit), new { clinicalVisitId = request.ClinicalVisitId }, new { id = result.Value });
    }

    /// <summary>Lists imaging studies for a clinical visit.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByVisit([FromQuery] Guid clinicalVisitId, CancellationToken cancellationToken)
    {
        var studies = await _imagingStudyRepository.GetByVisitAsync(clinicalVisitId, cancellationToken);
        return Ok(studies.Select(s => new
        {
            s.Id,
            s.ClinicalVisitId,
            s.Modality,
            s.FileFormat,
            s.OriginalFileName,
            s.OrthancStudyId,
            s.MinioObjectKey,
            AcquiredAt = s.AcquiredAt,
            s.CreatedAt,
        }));
    }
}

public class UploadImagingStudyRequest
{
    public Guid ClinicalVisitId { get; set; }
    public string Modality { get; set; } = string.Empty;
    public IFormFile? File { get; set; }
    /// <summary>ISO 8601 acquisition date/time. Defaults to upload time if omitted.</summary>
    public string? AcquiredAt { get; set; }
}
