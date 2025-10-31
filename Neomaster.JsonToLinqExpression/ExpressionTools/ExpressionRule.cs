using System.Linq.Expressions;
using System.Text.Json;
using static Neomaster.JsonToLinqExpression.Consts;

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
    if (!jsonElement.TryGetProperty(fieldPropertyName, out var fieldProperty))
    {
      var exMessage = string.Format(ErrorMessages.JsonPropertyNotFound, fieldPropertyName);
      var ex = new KeyNotFoundException(exMessage);
      ex.Data[ErrorDataKeys.Json] = jsonElement.GetRawText();
      ex.Data[ErrorDataKeys.Property] = fieldPropertyName;

      throw ex;
    }

    if (fieldProperty.ValueKind != JsonValueKind.String)
    {
      var exMessage = string.Format(ErrorMessages.JsonPropertyNotType, fieldPropertyName, JsonValueKind.String);
      var ex = new InvalidOperationException(exMessage);
      ex.Data[ErrorDataKeys.Json] = jsonElement.GetRawText();
      ex.Data[ErrorDataKeys.Property] = fieldPropertyName;
      ex.Data[ErrorDataKeys.ExpectedType] = JsonValueKind.String;
      ex.Data[ErrorDataKeys.CurrentType] = fieldProperty.ValueKind;

      throw ex;
    }

    var srcFieldName = fieldProperty.GetString();

    if (string.IsNullOrWhiteSpace(srcFieldName))
    {
      var exMessage = string.Format(ErrorMessages.JsonPropertyEmpty, fieldPropertyName);
      var ex = new ArgumentException(exMessage);
      ex.Data[ErrorDataKeys.Json] = jsonElement.GetRawText();
      ex.Data[ErrorDataKeys.Property] = fieldPropertyName;

      throw ex;
    }

    if (!jsonElement.TryGetProperty(valuePropertyName, out var valueProperty))
    {
      var exMessage = string.Format(ErrorMessages.JsonPropertyNotFound, valuePropertyName);
      var ex = new KeyNotFoundException(exMessage);
      ex.Data[ErrorDataKeys.Json] = jsonElement.GetRawText();
      ex.Data[ErrorDataKeys.Property] = valuePropertyName;

      throw ex;
    }

    var field = mapper.Fields[srcFieldName];
    var rule = new ExpressionRule
    {
      Operator = jsonElement.GetProperty(operatorPropertyName).GetString(),
      Field = field.Name,
      Value = valueProperty,
      ValueConstantExpression = field.GetValue(valueProperty),
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
