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
    string operatorPropertyName = nameof(Operator),
    string fieldPropertyName = nameof(Field),
    string valuePropertyName = nameof(Value))
  {
    var srcFieldName = jsonElement.GetProperty(fieldPropertyName).GetString();
    var srcValue = jsonElement.GetProperty(valuePropertyName);
    var field = mapper.Fields[fieldPropertyName];
    var rule = new ExpressionRule
    {
      Operator = jsonElement.GetProperty(operatorPropertyName).GetString(),
      Field = field.Name,
      Value = srcValue,
      ValueConstantExpression = field.GetValue(srcValue),
    };

    return rule;
  }

  public Expression CreateFilterExpression(
    ParameterExpression par,
    ExpressionOperatorMapper mapper = null)
  {
    mapper ??= Consts.ExpressionOperatorMappers.Default;

    var prop = Expression.Property(par, Field);
    var body = mapper.Operators[Operator](prop, ValueConstantExpression);

    return body;
  }
}
