using System.Text.Json;
using Xunit.Abstractions;

namespace Neomaster.JsonToLinq.UnitTests;

public class ExpressionFieldMapperFactoryUnitTests(ITestOutputHelper output)
{
  private static readonly Type _propertiesPublicGetSet = typeof(PropertiesPublicGetSet);

  [Fact]
  public void CreateForPublicProperties_Mapping()
  {
    var props = _propertiesPublicGetSet
      .GetProperties()
      .ToArray();

    var mapper = ExpressionFieldMapperFactory
      .CreateForPublicProperties<PropertiesPublicGetSet>();

    Assert.Equal(props.Length, mapper.Fields.Count);
    Assert.All(mapper.Fields, (f, i) =>
    {
      var expectedName = props[i].Name;
      Assert.Equal(expectedName, f.Key);
      Assert.Equal(expectedName, f.Value.Name);
      output.WriteLine(f.Key);
    });
  }

  [Fact]
  public void CreateForPublicProperties_GettingValue()
  {
    var obj = new PropertiesPublicGetSet();
    var propValues = _propertiesPublicGetSet
      .GetProperties()
      .Select(p => p.GetValue(obj))
      .ToArray();
    var jsonElements = propValues
      .Select(v => JsonSerializer.SerializeToElement(v))
      .ToArray();

    var mapperValues = ExpressionFieldMapperFactory
      .CreateForPublicProperties<PropertiesPublicGetSet>().Fields.Values
      .Select((v, i) => v.GetValue(jsonElements[i]).Value)
      .ToArray();

    Assert.Equal(propValues.Length, mapperValues.Length);
    Assert.All(mapperValues, (actual, i) =>
    {
      var expected = propValues[i];
      Assert.Equal(expected, actual);
      output.WriteLine(actual?.ToString() ?? "Null");
    });
  }
}
