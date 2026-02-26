using FluentValidation;

namespace OphthalmicRegistry.Application.Patients.Commands.RegisterPatient;

public class RegisterPatientCommandValidator : AbstractValidator<RegisterPatientCommand>
{
    public RegisterPatientCommandValidator()
    {
        RuleFor(x => x.PseudonymizedId)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.DateOfBirth)
            .NotEmpty()
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.Sex)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.SiteId)
            .NotEmpty();
    }
}
