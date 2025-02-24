using System.Reflection;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Vertr.Adapters.Tinvest.Converters;

namespace Vertr.Adapters.Tinvest.Tests;

[TestFixture(Category = "Unit")]
public class MappingTests
{
    [Test]
    public void CanUseTinvestMappingProfile()
    {
        // Arrange
        var config = new MapperConfiguration(configuration =>
        {
            configuration.EnableEnumMappingValidation();
            configuration.AddMaps(typeof(TinvestMappingProfile).GetTypeInfo().Assembly);
        });

        // Assert
        config.AssertConfigurationIsValid();
    }
}
