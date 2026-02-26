using MediatR;
using OphthalmicRegistry.Application.Common;
using OphthalmicRegistry.Domain.Entities;
using OphthalmicRegistry.Domain.Repositories;

namespace OphthalmicRegistry.Application.ClinicalVisits.Commands.CreateClinicalVisit;

public class CreateClinicalVisitCommandHandler : IRequestHandler<CreateClinicalVisitCommand, Result<Guid>>
{
    private readonly IClinicalVisitRepository _visitRepository;
    private readonly IPatientRepository _patientRepository;

    public CreateClinicalVisitCommandHandler(
        IClinicalVisitRepository visitRepository,
        IPatientRepository patientRepository)
    {
        _visitRepository = visitRepository;
        _patientRepository = patientRepository;
    }

    public async Task<Result<Guid>> Handle(CreateClinicalVisitCommand request, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
            return Result.Failure<Guid>("Patient not found.", "PATIENT_NOT_FOUND");

        var visit = ClinicalVisit.Create(request.PatientId, request.VisitDate);
        visit.WithClinicalData(request.BestCorrectedVisualAcuity, request.IntraocularPressure, request.ClinicalNotes);

        await _visitRepository.AddAsync(visit, cancellationToken);
        return Result.Success(visit.Id);
    }
}
