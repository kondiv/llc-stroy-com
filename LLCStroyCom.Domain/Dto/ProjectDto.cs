using LLCStroyCom.Domain.Enums;

namespace LLCStroyCom.Domain.Dto;

public record ProjectDto(string Name, string City, Guid CompanyId, Status Status, DateTimeOffset CreatedAt);