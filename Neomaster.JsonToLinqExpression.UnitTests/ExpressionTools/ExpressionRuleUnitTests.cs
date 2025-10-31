using System.Linq.Expressions;
using System.Text.Json;
using Xunit.Abstractions;

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
}
