using MediatR;
using OphthalmicRegistry.Application.Common;
using OphthalmicRegistry.Application.Common.Interfaces;
using OphthalmicRegistry.Domain.Entities;
using OphthalmicRegistry.Domain.Repositories;

namespace OphthalmicRegistry.Application.Imaging.Commands.UploadImagingStudy;

public class UploadImagingStudyCommandHandler : IRequestHandler<UploadImagingStudyCommand, Result<Guid>>
{
    private static readonly IReadOnlySet<string> DicomExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".dcm", ".dicom" };

    private static readonly IReadOnlySet<string> NativeConvertibleExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".e2e", ".fds", ".fda", ".img", ".sdb", ".oct" };

    private readonly IClinicalVisitRepository _visitRepository;
    private readonly IImagingStudyRepository _imagingStudyRepository;
    private readonly IOrthancService _orthancService;
    private readonly IStorageService _storageService;
    private readonly IOctConverterService _octConverterService;

    public UploadImagingStudyCommandHandler(
        IClinicalVisitRepository visitRepository,
        IImagingStudyRepository imagingStudyRepository,
        IOrthancService orthancService,
        IStorageService storageService,
        IOctConverterService octConverterService)
    {
        _visitRepository = visitRepository;
        _imagingStudyRepository = imagingStudyRepository;
        _orthancService = orthancService;
        _storageService = storageService;
        _octConverterService = octConverterService;
    }

    public async Task<Result<Guid>> Handle(UploadImagingStudyCommand request, CancellationToken cancellationToken)
    {
        var visit = await _visitRepository.GetByIdAsync(request.ClinicalVisitId, cancellationToken);
        if (visit is null)
            return Result.Failure<Guid>("Clinical visit not found.", "VISIT_NOT_FOUND");

        var extension = Path.GetExtension(request.FileName).TrimStart('.');
        var fileFormat = extension.ToLowerInvariant();
        var study = ImagingStudy.Create(request.ClinicalVisitId, request.Modality, fileFormat, request.AcquiredAt, request.FileName);

        var ext = Path.GetExtension(request.FileName);

        if (DicomExtensions.Contains(ext))
        {
            // DICOM: upload directly to Orthanc
            var orthancStudyId = await _orthancService.UploadDicomAsync(request.FileStream, cancellationToken);
            study.SetOrthancStudyId(orthancStudyId);
        }
        else if (NativeConvertibleExtensions.Contains(ext))
        {
            // Native format: store original in MinIO and attempt DICOM conversion
            var objectKey = $"imaging/{study.Id}/{request.FileName}";
            await _storageService.UploadAsync(request.FileStream, objectKey, request.ContentType, cancellationToken);
            study.SetMinioObjectKey(objectKey);

            // Attempt conversion to DICOM via OCT-Converter service
            request.FileStream.Position = 0;
            var dicomStream = await _octConverterService.ConvertToDicomAsync(request.FileStream, ext.TrimStart('.'), cancellationToken);
            if (dicomStream is not null)
            {
                await using (dicomStream)
                {
                    var orthancStudyId = await _orthancService.UploadDicomAsync(dicomStream, cancellationToken);
                    study.SetOrthancStudyId(orthancStudyId);
                }
            }
        }
        else
        {
            // Unknown format: store as-is in MinIO
            var objectKey = $"imaging/{study.Id}/{request.FileName}";
            await _storageService.UploadAsync(request.FileStream, objectKey, request.ContentType, cancellationToken);
            study.SetMinioObjectKey(objectKey);
        }

        await _imagingStudyRepository.AddAsync(study, cancellationToken);
        return Result.Success(study.Id);
    }
}
