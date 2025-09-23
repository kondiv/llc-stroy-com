using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Services;

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
    
    public async Task<DefectDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var defect = await _defectRepository.GetAsync(id, cancellationToken);
        
        return _mapper.Map<DefectDto>(defect);
    }
}