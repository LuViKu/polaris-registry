using MediatR;
using OphthalmicRegistry.Application.Common;

namespace OphthalmicRegistry.Application.Imaging.Commands.UploadImagingStudy;

/// <summary>Uploads an imaging study to the registry, routing DICOMs to Orthanc and native formats to MinIO.</summary>
/// <param name="ClinicalVisitId">The visit this study belongs to.</param>
/// <param name="Modality">Imaging modality (e.g. "OCT", "FP", "FAF", "ICG").</param>
/// <param name="FileStream">Stream of the uploaded file.</param>
/// <param name="FileName">Original file name (used to detect format by extension).</param>
/// <param name="ContentType">MIME type of the uploaded file.</param>
/// <param name="AcquiredAt">Acquisition date/time of the study.</param>
public record UploadImagingStudyCommand(
    Guid ClinicalVisitId,
    string Modality,
    Stream FileStream,
    string FileName,
    string ContentType,
    DateTimeOffset AcquiredAt) : IRequest<Result<Guid>>;
