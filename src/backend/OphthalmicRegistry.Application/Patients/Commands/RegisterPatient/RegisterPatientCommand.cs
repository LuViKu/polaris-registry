using MediatR;
using OphthalmicRegistry.Application.Common;

namespace OphthalmicRegistry.Application.Patients.Commands.RegisterPatient;

/// <summary>Registers a new pseudonymized patient record.</summary>
/// <param name="PseudonymizedId">HMAC-pseudonymized hospital MRN (computed by the caller).</param>
/// <param name="DateOfBirth">Patient date of birth.</param>
/// <param name="Sex">Biological sex (e.g. "Male", "Female", "Other").</param>
/// <param name="SiteId">Identifier of the originating clinical site.</param>
public record RegisterPatientCommand(
    string PseudonymizedId,
    DateOnly DateOfBirth,
    string Sex,
    Guid SiteId) : IRequest<Result<Guid>>;
