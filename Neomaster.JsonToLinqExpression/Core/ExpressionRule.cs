using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinqExpression;

public class ExpressionRule
{
  public string Operator { get; set; }
  public string Field { get; set; }
  public JsonElement? Value { get; set; }
  public ConstantExpression ValueConstantExpression { get; set; }

  public static ExpressionRule Parse(
    JsonElement jsonElement,
    ExpressionFieldMapper mapper,
    string fieldPropertyName = nameof(Field),
    string valuePropertyName = nameof(Value))
  {
    var srcFieldName = jsonElement.GetProperty(fieldPropertyName).GetString();
    var srcValue = jsonElement.GetProperty(valuePropertyName);
    var field = mapper.Fields[srcFieldName];
    var rule = new ExpressionRule
    {
      Operator = field.Operator,
      Field = field.Name,
      Value = srcValue,
      ValueConstantExpression = field.GetValue(srcValue),
    };

    return rule;
  }
}
