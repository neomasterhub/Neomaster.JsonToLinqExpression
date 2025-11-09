using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Neomaster.JsonToLinq;

public class ExpressionFieldMapperFactory
{
  public static ExpressionFieldMapper CreateForPublicProperties<T>()
  {
    var mapper = new ExpressionFieldMapper();
    var type = typeof(T);
    var props = type
      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .Where(p => p.CanRead && p.CanWrite);

    foreach (var prop in props)
    {
      mapper.Add(prop.Name, new ExpressionField
      {
        Name = prop.Name,
        GetValue = je => Expression.Constant(
          je?.Deserialize(prop.PropertyType),
          prop.PropertyType),
      });
    }

    return mapper;
  }
}
