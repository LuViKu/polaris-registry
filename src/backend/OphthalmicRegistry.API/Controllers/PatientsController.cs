using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OphthalmicRegistry.Application.Patients.Commands.RegisterPatient;
using OphthalmicRegistry.Domain.Repositories;

namespace OphthalmicRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPatientRepository _patientRepository;

    public PatientsController(IMediator mediator, IPatientRepository patientRepository)
    {
        _mediator = mediator;
        _patientRepository = patientRepository;
    }

    /// <summary>Registers a new pseudonymized patient record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterPatientRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterPatientCommand(
            request.PseudonymizedId,
            DateOnly.Parse(request.DateOfBirth),
            request.Sex,
            request.SiteId);

        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ErrorCode == "DUPLICATE_PATIENT"
                ? Conflict(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetBySite), new { siteId = request.SiteId }, new { id = result.Value });
    }

    /// <summary>Lists patients for a given site.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySite([FromQuery] Guid siteId, CancellationToken cancellationToken)
    {
        var patients = await _patientRepository.GetBySiteAsync(siteId, cancellationToken);
        return Ok(patients.Select(p => new
        {
            p.Id,
            p.PseudonymizedId,
            DateOfBirth = p.DateOfBirth.ToString("yyyy-MM-dd"),
            p.Sex,
            p.SiteId,
            p.CreatedAt,
        }));
    }
}

public record RegisterPatientRequest(
    string PseudonymizedId,
    string DateOfBirth,
    string Sex,
    Guid SiteId);
