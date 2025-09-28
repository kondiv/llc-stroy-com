using AutoMapper;
using LLCStroyCom.Application.MapperProfiles;
using Microsoft.Extensions.Logging;

namespace LLCStroyCom.Tests.Mapper;

public class MapperTests
{
    [Fact]
    public void WhenConfigured_ShouldNotThrowException()
    {
        // Arrange
        var loggerFactory = new LoggerFactory();
        
        // Act
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<DefectProfile>();
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<CompanyProfile>();
            cfg.AddProfile<ProjectProfile>();
        }, loggerFactory);
        
        // Assert
        configuration.AssertConfigurationIsValid();
    }
}