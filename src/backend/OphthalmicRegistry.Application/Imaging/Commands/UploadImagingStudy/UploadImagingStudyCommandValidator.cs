using FluentValidation;

namespace OphthalmicRegistry.Application.Imaging.Commands.UploadImagingStudy;

public class UploadImagingStudyCommandValidator : AbstractValidator<UploadImagingStudyCommand>
{
    private static readonly IReadOnlySet<string> AllowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        ".dcm", ".dicom",               // DICOM
        ".e2e", ".fds", ".fda",         // Heidelberg / Topcon OCT
        ".img", ".sdb", ".oct",         // Other native formats
        ".png", ".jpg", ".jpeg", ".tiff" // Standard image fallback
    };

    public UploadImagingStudyCommandValidator()
    {
        RuleFor(x => x.ClinicalVisitId).NotEmpty();

        RuleFor(x => x.Modality)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.FileName)
            .NotEmpty()
            .MaximumLength(500)
            .Must(HaveAllowedExtension)
            .WithMessage($"File type is not supported. Allowed: {string.Join(", ", AllowedExtensions.Order())}.");

        RuleFor(x => x.FileStream)
            .NotNull()
            .Must(s => s.Length > 0)
            .WithMessage("Uploaded file is empty.");

        RuleFor(x => x.AcquiredAt)
            .NotEmpty()
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("Acquisition date cannot be in the future.");
    }

    private static bool HaveAllowedExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);
        return !string.IsNullOrEmpty(ext) && AllowedExtensions.Contains(ext);
    }
}
