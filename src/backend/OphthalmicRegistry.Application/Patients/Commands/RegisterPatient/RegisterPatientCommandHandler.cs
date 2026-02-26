using MediatR;
using OphthalmicRegistry.Application.Common;
using OphthalmicRegistry.Domain.Entities;
using OphthalmicRegistry.Domain.Repositories;

namespace OphthalmicRegistry.Application.Patients.Commands.RegisterPatient;

public class RegisterPatientCommandHandler : IRequestHandler<RegisterPatientCommand, Result<Guid>>
{
    private readonly IPatientRepository _patientRepository;
    private readonly ISiteRepository _siteRepository;

    public RegisterPatientCommandHandler(IPatientRepository patientRepository, ISiteRepository siteRepository)
    {
        _patientRepository = patientRepository;
        _siteRepository = siteRepository;
    }

    public async Task<Result<Guid>> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetByIdAsync(request.SiteId, cancellationToken);
        if (site is null)
            return Result.Failure<Guid>("Site not found.", "SITE_NOT_FOUND");

        var existing = await _patientRepository.GetByPseudonymizedIdAsync(request.PseudonymizedId, cancellationToken);
        if (existing is not null)
            return Result.Failure<Guid>("A patient with this pseudonymized ID already exists.", "DUPLICATE_PATIENT");

        var patient = Patient.Create(request.PseudonymizedId, request.DateOfBirth, request.Sex, request.SiteId);
        await _patientRepository.AddAsync(patient, cancellationToken);
        return Result.Success(patient.Id);
    }
}
