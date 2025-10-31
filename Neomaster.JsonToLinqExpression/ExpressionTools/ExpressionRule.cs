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
    var op = GetPropertyRequiredStringValue(jsonElement, operatorPropertyName, JsonValueKind.String);
    var srcFieldName = GetPropertyRequiredStringValue(jsonElement, fieldPropertyName, JsonValueKind.String);
    var valueProperty = GetProperty(jsonElement, valuePropertyName);

    var field = mapper.Fields[srcFieldName];
    var rule = new ExpressionRule
    {
      Operator = op,
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

  private static JsonElement GetProperty(
    JsonElement obj,
    string propertyName,
    JsonValueKind? expectedPropertyType = null)
  {
    if (!obj.TryGetProperty(propertyName, out var property))
    {
      var exMessage = string.Format(ErrorMessages.JsonPropertyNotFound, propertyName);
      var ex = new KeyNotFoundException(exMessage);
      ex.Data[ErrorDataKeys.Json] = obj.GetRawText();
      ex.Data[ErrorDataKeys.Property] = propertyName;

      throw ex;
    }

    if (expectedPropertyType != null
      && property.ValueKind != expectedPropertyType)
    {
      var exMessage = string.Format(ErrorMessages.JsonPropertyNotType, propertyName, expectedPropertyType);
      var ex = new InvalidOperationException(exMessage);
      ex.Data[ErrorDataKeys.Json] = obj.GetRawText();
      ex.Data[ErrorDataKeys.Property] = propertyName;
      ex.Data[ErrorDataKeys.ExpectedType] = expectedPropertyType;
      ex.Data[ErrorDataKeys.CurrentType] = property.ValueKind;

      throw ex;
    }

    return property;
  }

  public static string GetPropertyRequiredStringValue(
    JsonElement obj,
    string propertyName,
    JsonValueKind? expectedPropertyType = null)
  {
    var value = GetProperty(obj, propertyName, expectedPropertyType).GetString();

    if (!string.IsNullOrWhiteSpace(value))
    {
      return value;
    }

    var exMessage = string.Format(ErrorMessages.JsonPropertyEmpty, propertyName);
    var ex = new ArgumentException(exMessage);
    ex.Data[ErrorDataKeys.Json] = obj.GetRawText();
    ex.Data[ErrorDataKeys.Property] = propertyName;

    throw ex;
  }
}
