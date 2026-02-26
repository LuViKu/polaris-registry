using FluentValidation;

namespace OphthalmicRegistry.Application.ClinicalVisits.Commands.CreateClinicalVisit;

public class CreateClinicalVisitCommandValidator : AbstractValidator<CreateClinicalVisitCommand>
{
    public CreateClinicalVisitCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty();

        RuleFor(x => x.VisitDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Visit date cannot be in the future.");

        RuleFor(x => x.BestCorrectedVisualAcuity)
            .MaximumLength(50)
            .When(x => x.BestCorrectedVisualAcuity is not null);

        RuleFor(x => x.IntraocularPressure)
            .MaximumLength(50)
            .When(x => x.IntraocularPressure is not null);
    }
}
