using AutoMapper;
using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Entities;
using LLCStroyCom.Domain.Repositories;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace LLCStroyCom.Application.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CompanyService> _logger;
    private readonly IMapper _mapper;

    public CompanyService(
        ICompanyRepository companyRepository,
        IUserRepository userRepository,
        ILogger<CompanyService> logger,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
        _logger = logger;
        _mapper = mapper;
    }
    
    public async Task<CompanyDto> CreateAsync(CompanyCreateRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating Company");
        
        ArgumentNullException.ThrowIfNull(request);

        var company = _mapper.Map<Company>(request);
        
        var result = await _companyRepository.CreateAsync(company, cancellationToken);
        
        var companyDto = _mapper.Map<CompanyDto>(result);

        _logger.LogInformation("Created Company");
        return companyDto;
    }

    public async Task<Guid> AddEmployeeAsync(Guid companyId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding Employee");
        var employee = await _userRepository.GetAsync(employeeId, cancellationToken);
        
        var company = await _companyRepository.GetExtendedAsync(companyId, cancellationToken);
        
        _logger.LogInformation("Got required entities");
        
        employee.SetCompany(companyId);
        company.AddEmployee(employee);

        await _companyRepository.UpdateAsync(company, cancellationToken);

        _logger.LogInformation("Added Employee");
        return employee.Id;
    }

    public async Task<CompanyDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Trying to get company with Id: {id}");
        
        var company = await _companyRepository.GetAsync(id, cancellationToken);

        var companyDto = _mapper.Map<CompanyDto>(company);

        return companyDto;
    }

    public async Task UpdateAsync(Guid id, JsonPatchDocument<CompanyPatchDto> patchDocument, CancellationToken cancellationToken = default)
    {
        var company = await _companyRepository.GetAsync(id, cancellationToken);
        _logger.LogInformation("Found company");
        
        var companyDto = _mapper.Map<CompanyPatchDto>(company);
        
        patchDocument.ApplyTo(companyDto);
        
        _mapper.Map(companyDto, company);
        
        await _companyRepository.UpdateAsync(company, cancellationToken);
        _logger.LogInformation("Updated company");
    }

    public async Task RemoveEmployeeAsync(Guid companyId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await _userRepository.GetAsync(employeeId, cancellationToken);
        var company = await _companyRepository.GetExtendedAsync(companyId, cancellationToken);
        _logger.LogInformation("Got required entities");
        
        employee.RemoveCompany();
        company.RemoveEmployee(employee.Id);
        
        await _companyRepository.UpdateAsync(company, cancellationToken);
        _logger.LogInformation("Removed employee");
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Trying to delete company with Id: {id}");
        
        await _companyRepository.DeleteAsync(id, cancellationToken);
        
        _logger.LogInformation($"Company with Id: {id} was successfully deleted");
    }
}