using LLCStroyCom.Domain.Dto;
using LLCStroyCom.Domain.Models;
using LLCStroyCom.Domain.Requests;
using LLCStroyCom.Domain.Specifications.Companies;
using LLCStroyCom.Domain.Specifications.Users;
using Microsoft.AspNetCore.JsonPatch;

namespace LLCStroyCom.Domain.Services;

public interface ICompanyService
{
    Task<CompanyDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginationResult<CompanyDto>> ListAsync(CompanySpecification specification, int maxPageSize,
        int page, CancellationToken cancellationToken = default);
    Task<CompanyDto> CreateAsync(CompanyCreateRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(Guid Id, JsonPatchDocument<CompanyPatchDto> patchDocument, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, CancellationToken cancellationToken = default);
    Task<PaginationResult<EmployeeDto>> ListCompanyEmployeesAsync(Guid companyId, ApplicationUserSpecification specification,
        int maxPageSize, int page, CancellationToken cancellationToken = default);
    Task<Guid> AddEmployeeAsync(Guid companyId, Guid employeeId, CancellationToken cancellationToken = default);
    Task RemoveEmployeeAsync(Guid companyId, Guid employeeId, CancellationToken cancellationToken = default);
}