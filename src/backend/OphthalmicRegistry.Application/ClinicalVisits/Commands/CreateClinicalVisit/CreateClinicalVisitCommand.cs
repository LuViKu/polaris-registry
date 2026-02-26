using MediatR;
using OphthalmicRegistry.Application.Common;

namespace OphthalmicRegistry.Application.ClinicalVisits.Commands.CreateClinicalVisit;

/// <summary>Records a new clinical visit for a patient, including all structured ophthalmic measurements.</summary>
/// <param name="PatientId">Patient identifier.</param>
/// <param name="VisitDate">Date the visit occurred.</param>
/// <param name="BestCorrectedVisualAcuity">BCVA measurement (e.g. "6/6" or "20/20").</param>
/// <param name="IntraocularPressure">IOP measurement in mmHg (e.g. "14 mmHg").</param>
/// <param name="ClinicalNotes">Free-text clinical observations.</param>
public record CreateClinicalVisitCommand(
    Guid PatientId,
    DateOnly VisitDate,
    string? BestCorrectedVisualAcuity,
    string? IntraocularPressure,
    string? ClinicalNotes) : IRequest<Result<Guid>>;
