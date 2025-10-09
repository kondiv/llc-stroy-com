using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.ResultPattern;
using LLCStroyCom.Domain.ResultPattern.Errors;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications.Defects;
using Microsoft.AspNetCore.JsonPatch;

namespace LLCStroyCom.Application.Services;

public sealed class DefectService : IDefectService
{
    private readonly IDefectRepository _defectRepository;
    private readonly IMapper _mapper;

    public DefectService(IDefectRepository defectRepository, IMapper mapper)
    {
        _defectRepository = defectRepository;
        _mapper = mapper;
    }

    public async Task<Result<DefectDto>> GetAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default)
    {
        var getDefectResult =  await _defectRepository.GetAsync(projectId, defectId, cancellationToken);

        if (getDefectResult.IsFailure)
        {
            return Result<DefectDto>.Failure(getDefectResult.Error);
        }
        
        var dto = _mapper.Map<DefectDto>(getDefectResult.Value);

        return Result<DefectDto>.Success(dto);
    }

    public async Task<PaginationResult<DefectDto>> ListAsync(Guid projectId, DefectSpecification specification, int maxPageSize, int page,
        CancellationToken cancellationToken = default)
    {
        var defectPaginatedResult = await _defectRepository.ListAsync(projectId, specification, maxPageSize, page, cancellationToken);
        
        var defectDtoList = defectPaginatedResult.Items.Select(d => _mapper.Map<DefectDto>(d)).ToList();
        
        return new PaginationResult<DefectDto>(defectDtoList, defectPaginatedResult.Page, defectPaginatedResult.MaxPageSize,
            defectPaginatedResult.PageCount, defectPaginatedResult.TotalCount);
    }

    public async Task<Result<DefectDto>> CreateAsync(Guid projectId, DefectCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var defect = new Defect()
        {
            Name = request.Name,
            Description = request.Description,
            ProjectId = projectId,
        };

        var defectCreateResult = await _defectRepository.CreateAsync(defect, cancellationToken);

        if (defectCreateResult.IsFailure)
        {
            return Result<DefectDto>.Failure(defectCreateResult.Error);
        }
        
        var dto = _mapper.Map<DefectDto>(defectCreateResult.Value);
        
        return Result<DefectDto>.Success(dto);
    }

    public async Task<Result> UpdateAsync(Guid projectId, Guid defectId, JsonPatchDocument<DefectPatchDto> patchDocument,
        CancellationToken cancellationToken = default)
    {
        var getDefectResult = await _defectRepository.GetAsync(projectId, defectId, cancellationToken);

        if (getDefectResult.IsFailure)
        {
            return Result.Failure(getDefectResult.Error);
        }
        
        var defectPatchDto = _mapper.Map<DefectPatchDto>(getDefectResult.Value);
        
        patchDocument.ApplyTo(defectPatchDto);
        
        var defect = _mapper.Map(defectPatchDto, getDefectResult.Value);
        
        return await _defectRepository.UpdateAsync(defect, cancellationToken);
    }

    public async Task<Result> DeleteAsync(Guid projectId, Guid defectId, CancellationToken cancellationToken = default)
    {
        return await _defectRepository.DeleteAsync(projectId, defectId, cancellationToken);
    }
}