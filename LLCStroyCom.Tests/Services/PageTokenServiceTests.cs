using System.Text;
using LLCStroyCom.Application.Services;
using LLCStroyCom.Domain.Enums;
using LLCStroyCom.Domain.Exceptions;
using LLCStroyCom.Domain.Models.PageTokens;
using LLCStroyCom.Domain.Services;
using LLCStroyCom.Domain.Specifications;
using LLCStroyCom.Domain.Specifications.Projects;

namespace LLCStroyCom.Tests.Services;

public class PageTokenServiceTests
{
    private readonly IPageTokenService _service;
    
    public PageTokenServiceTests()
    {
        _service = new PageTokenService();
    }

    private class UserPageToken : PageToken
    {
        public override string OrderBy { get; set; } = "email";
    }

    [Fact]
    public void Encode_ValidToken_ReturnsBase64String()
    {
        // Arrange
        var token = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "Test Project",
            OrderBy = "Name",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = _service.Encode(token);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Matches(@"^[a-zA-Z0-9\+/]*={0,2}$", result); // Base64 pattern
    }

    [Fact]
    public void Decode_ValidEncodedToken_ReturnsOriginalToken()
    {
        // Arrange
        var originalToken = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "Test Project",
            OrderBy = "Name",
            CreatedAt = DateTimeOffset.UtcNow,
            ProjectCreatedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };

        var encodedToken = _service.Encode(originalToken);

        // Act
        var decodedToken = _service.Decode<ProjectPageToken>(encodedToken);

        // Assert
        Assert.NotNull(decodedToken);
        Assert.Equal(originalToken.ProjectId, decodedToken.ProjectId);
        Assert.Equal(originalToken.ProjectName, decodedToken.ProjectName);
        Assert.Equal(originalToken.OrderBy, decodedToken.OrderBy);
        Assert.Equal(originalToken.CreatedAt, decodedToken.CreatedAt);
        Assert.Equal(originalToken.ProjectCreatedAt, decodedToken.ProjectCreatedAt);
    }

    [Fact]
    public void EncodeDecode_RoundTrip_ReturnsEquivalentObject()
    {
        // Arrange
        var originalToken = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "Round Trip Test",
            OrderBy = "Name",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var encoded = _service.Encode(originalToken);
        var decoded = _service.Decode<ProjectPageToken>(encoded);

        // Assert
        Assert.Equal(originalToken.ProjectId, decoded.ProjectId);
        Assert.Equal(originalToken.ProjectName, decoded.ProjectName);
        Assert.Equal(originalToken.OrderBy, decoded.OrderBy);
        Assert.Equal(originalToken.CreatedAt, decoded.CreatedAt);
    }
    
    [Fact]
    public void Encode_NullToken_ThrowsArgumentNullException()
    {
        // Arrange
        ProjectPageToken nullToken = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.Encode(nullToken));
    }

    [Fact]
    public void Decode_NullEncodedToken_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _service.Decode<ProjectPageToken>(null));
    }

    [Fact]
    public void Decode_EmptyString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.Decode<ProjectPageToken>(""));
    }

    [Fact]
    public void Decode_WhitespaceString_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.Decode<ProjectPageToken>("   "));
    }

    [Fact]
    public void Decode_InvalidBase64_ThrowsFormatException()
    {
        // Arrange
        var invalidBase64 = "This is not base64!";

        // Act & Assert
        Assert.Throws<PageTokenDecodingException>(() => _service.Decode<ProjectPageToken>(invalidBase64));
    }

    [Fact]
    public void Decode_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = "This is not JSON!";
        var invalidJsonBytes = Encoding.UTF8.GetBytes(invalidJson);
        var invalidBase64 = Convert.ToBase64String(invalidJsonBytes);

        // Act & Assert
        Assert.Throws<PageTokenDecodingException>(() => _service.Decode<ProjectPageToken>(invalidBase64));
    }

    [Fact]
    public void Decode_ValidBase64ButWrongType_ThrowsJsonException()
    {
        // Arrange
        var token = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "Test",
            OrderBy = "Name",
            CreatedAt = DateTimeOffset.UtcNow
        };

        var encodedToken = _service.Encode(token);

        // Act & Assert - пытаемся декодировать как другой тип
        Assert.Throws<PageTokenDecodingException>(() => _service.Decode<UserPageToken>(encodedToken));
    }
    
    [Fact]
    public void EncodeDecode_TokenWithNullProperties_PreservesNullValues()
    {
        // Arrange
        var token = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = null, // null property
            OrderBy = "Name",
            CreatedAt = DateTimeOffset.UtcNow,
            ProjectCreatedAt = null // null property
        };

        // Act
        var encoded = _service.Encode(token);
        var decoded = _service.Decode<ProjectPageToken>(encoded);

        // Assert
        Assert.Null(decoded.ProjectName);
        Assert.Null(decoded.ProjectCreatedAt);
    }

    [Fact]
    public void EncodeDecode_TokenWithEmptyString_PreservesEmptyString()
    {
        // Arrange
        var token = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "", // empty string
            OrderBy = "Name",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var encoded = _service.Encode(token);
        var decoded = _service.Decode<ProjectPageToken>(encoded);

        // Assert
        Assert.Equal("", decoded.ProjectName);
    }

    [Fact]
    public void EncodeDecode_TokenWithMinMaxDateTime_PreservesValues()
    {
        // Arrange
        var token = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "DateTime Test",
            OrderBy = "Name",
            CreatedAt = DateTimeOffset.MinValue,
            ProjectCreatedAt = DateTimeOffset.MaxValue
        };

        // Act
        var encoded = _service.Encode(token);
        var decoded = _service.Decode<ProjectPageToken>(encoded);

        // Assert
        Assert.Equal(DateTimeOffset.MinValue, decoded.CreatedAt);
        Assert.Equal(DateTimeOffset.MaxValue, decoded.ProjectCreatedAt);
    }

    [Fact]
    public void EncodeDecode_MultipleTokens_WorkIndependently()
    {
        // Arrange
        var orderBy1 = "Name";
        var orderBy2 = "Name";
        var desc1 = true;
        var desc2 = false;
        
        var token1 = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "First",
            OrderBy = orderBy1,
            Descending = desc1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var token2 = new ProjectPageToken
        {
            ProjectId = Guid.NewGuid(),
            ProjectName = "Second",
            OrderBy = orderBy2,
            Descending = desc2,
            CreatedAt = DateTimeOffset.UtcNow.AddHours(1)
        };

        // Act
        var encoded1 = _service.Encode(token1);
        var encoded2 = _service.Encode(token2);
        
        var decoded1 = _service.Decode<ProjectPageToken>(encoded1);
        var decoded2 = _service.Decode<ProjectPageToken>(encoded2);

        // Assert
        Assert.Equal("First", decoded1.ProjectName);
        Assert.Equal("Second", decoded2.ProjectName);
        Assert.Equal(orderBy1, decoded1.OrderBy);
        Assert.Equal(orderBy2, decoded2.OrderBy);
    }
}