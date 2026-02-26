using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OphthalmicRegistry.Application.ClinicalVisits.Commands.CreateClinicalVisit;
using OphthalmicRegistry.Domain.Repositories;

namespace OphthalmicRegistry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClinicalVisitsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IClinicalVisitRepository _visitRepository;

    public ClinicalVisitsController(IMediator mediator, IClinicalVisitRepository visitRepository)
    {
        _mediator = mediator;
        _visitRepository = visitRepository;
    }

    /// <summary>Records a new clinical visit with structured ophthalmic measurements.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateClinicalVisitRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateClinicalVisitCommand(
            request.PatientId,
            DateOnly.Parse(request.VisitDate),
            request.BestCorrectedVisualAcuity,
            request.IntraocularPressure,
            request.ClinicalNotes);

        var result = await _mediator.Send(command, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.ErrorCode == "PATIENT_NOT_FOUND"
                ? NotFound(new { error = result.Error })
                : BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetByPatient), new { patientId = request.PatientId }, new { id = result.Value });
    }

    /// <summary>Lists clinical visits for a patient, ordered by visit date descending.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByPatient([FromQuery] Guid patientId, CancellationToken cancellationToken)
    {
        var visits = await _visitRepository.GetByPatientAsync(patientId, cancellationToken);
        return Ok(visits.Select(v => new
        {
            v.Id,
            v.PatientId,
            VisitDate = v.VisitDate.ToString("yyyy-MM-dd"),
            v.BestCorrectedVisualAcuity,
            v.IntraocularPressure,
            v.ClinicalNotes,
            v.CreatedAt,
        }));
    }
}

public record CreateClinicalVisitRequest(
    Guid PatientId,
    string VisitDate,
    string? BestCorrectedVisualAcuity,
    string? IntraocularPressure,
    string? ClinicalNotes);
