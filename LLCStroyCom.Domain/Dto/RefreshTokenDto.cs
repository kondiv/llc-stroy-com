using LLCStroyCom.Domain.Entities;

namespace LLCStroyCom.Domain.Dto;

public record RefreshTokenDto(RefreshToken RefreshTokenEntity, string PlainRefreshToken);