using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using Microsoft.Extensions.Logging;

namespace LLCStroyCom.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger<CompanyService> _logger;
    private readonly IMapper _mapper;

    public CompanyService(
        ICompanyRepository companyRepository,
        ILogger<CompanyService> logger,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public Task CreateAsync(CreateCompanyRequest request)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<CompanyDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Trying to get company with Id: {id}");

        var company = await _companyRepository.GetAsync(id, cancellationToken);
        
        _logger.LogInformation("Got company entity");

        var companyDto = _mapper.Map<CompanyDto>(company);
        
        _logger.LogInformation("Mapping succeeded");

        return companyDto;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}