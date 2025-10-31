using System.Linq.Expressions;
using System.Text.Json;
using Xunit.Abstractions;
using static Neomaster.JsonToLinqExpression.Consts;

namespace Neomaster.JsonToLinqExpression.UnitTests;

public class ExpressionRuleUnitTests(ITestOutputHelper output)
{
  [Fact]
  public void Parse_ShouldParse()
  {
    var rule = new
    {
      op = "1",
      field = "2",
      value = "3",
    };
    var jsonElement = JsonSerializer.SerializeToElement(rule);
    var expectedField = rule.field + "*";
    var expectedValue = jsonElement.GetProperty(nameof(rule.value));
    var mapper = new ExpressionFieldMapper()
      .Add(
        rule.field,
        new ExpressionField
        {
          Name = expectedField,
          GetValue = je => Expression.Constant(je.Value.GetString()),
        });

    var ruleParsed = ExpressionRule.Parse(
      jsonElement,
      mapper,
      nameof(rule.op),
      nameof(rule.field),
      nameof(rule.value));
    output.WriteLine($"op: {ruleParsed.Operator}");
    output.WriteLine($"field: {ruleParsed.Field}");
    output.WriteLine($"value: {ruleParsed.ValueConstantExpression.Value}");

    Assert.Equal(rule.op, ruleParsed.Operator);
    Assert.Equal(rule.value, ruleParsed.ValueConstantExpression.Value);
    Assert.Equal(expectedField, ruleParsed.Field);
    Assert.Equal(expectedValue, ruleParsed.Value);
  }

  [Fact]
  public void Parse_ShouldThrowKeyNotFoundException_InvalidFieldPropertyName()
  {
    const string invalidKey = "0";
    var rule = new
    {
      op = "1",
      field = "2",
      value = "3",
    };
    var ruleJson = JsonSerializer.Serialize(rule);
    var ruleJsonElement = JsonSerializer.SerializeToElement(rule);
    var expectedExMessage = string.Format(ErrorMessages.JsonPropertyNotFound, invalidKey);
    var mapper = new ExpressionFieldMapper()
      .Add(
        rule.field,
        new ExpressionField
        {
          Name = rule.field,
          GetValue = je => Expression.Constant(je.Value.GetString()),
        });

    var ex = Assert.Throws<KeyNotFoundException>(() => ExpressionRule.Parse(
      ruleJsonElement,
      mapper,
      nameof(rule.op),
      invalidKey,
      nameof(rule.value)));

    Assert.Equal(expectedExMessage, ex.Message);
    Assert.Equal(invalidKey, ex.Data[ErrorDataKeys.JsonPropertyNotFound.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.JsonPropertyNotFound.Json]);
  }

  [Fact]
  public void Parse_ShouldThrowInvalidOperationException_InvalidFieldPropertyType()
  {
    var rule = new
    {
      op = "1",
      field = 2,
      value = "3",
    };
    var fieldPropertyName = nameof(rule.field);
    var ruleJson = JsonSerializer.Serialize(rule);
    var ruleJsonElement = JsonSerializer.SerializeToElement(rule);
    var expectedExMessage = string.Format(ErrorMessages.JsonPropertyNotType, fieldPropertyName, JsonValueKind.String);

    var ex = Assert.Throws<InvalidOperationException>(() => ExpressionRule.Parse(
      ruleJsonElement,
      new(),
      nameof(rule.op),
      fieldPropertyName,
      nameof(rule.value)));

    Assert.Equal(expectedExMessage, ex.Message);
    Assert.Equal(fieldPropertyName, ex.Data[ErrorDataKeys.JsonPropertyNotType.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.JsonPropertyNotType.Json]);
    Assert.Equal(JsonValueKind.String, ex.Data[ErrorDataKeys.JsonPropertyNotType.ExpectedType]);
    Assert.Equal(JsonValueKind.Number, ex.Data[ErrorDataKeys.JsonPropertyNotType.CurrentType]);
  }
}
