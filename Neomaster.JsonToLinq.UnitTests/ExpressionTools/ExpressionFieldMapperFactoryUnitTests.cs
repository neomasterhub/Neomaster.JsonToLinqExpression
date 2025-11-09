namespace Neomaster.JsonToLinq.UnitTests;

public class ExpressionFieldMapperFactoryUnitTests
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
    });
  }
}
