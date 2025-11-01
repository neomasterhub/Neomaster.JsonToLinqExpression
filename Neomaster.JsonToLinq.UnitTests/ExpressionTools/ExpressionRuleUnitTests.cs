using System.Linq.Expressions;
using System.Text.Json;
using Xunit.Abstractions;
using static Neomaster.JsonToLinq.Consts;

namespace Neomaster.JsonToLinq.UnitTests;

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
  public void Parse_ShouldThrowKeyNotFoundException_InvalidOperatorPropertyName()
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
      invalidKey,
      nameof(rule.field),
      nameof(rule.value)));

    Assert.Equal(expectedExMessage, ex.Message);
    Assert.Equal(invalidKey, ex.Data[ErrorDataKeys.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.Json]);
  }

  [Fact]
  public void Parse_ShouldThrowInvalidOperationException_InvalidOperatorPropertyType()
  {
    var rule = new
    {
      op = 1,
      field = "2",
      value = "3",
    };
    var operatorPropertyName = nameof(rule.op);
    var ruleJson = JsonSerializer.Serialize(rule);
    var ruleJsonElement = JsonSerializer.SerializeToElement(rule);
    var expectedExMessage = string.Format(ErrorMessages.JsonPropertyNotType, operatorPropertyName, JsonValueKind.String);

    var ex = Assert.Throws<InvalidOperationException>(() => ExpressionRule.Parse(
      ruleJsonElement,
      new(),
      operatorPropertyName,
      nameof(rule.field),
      nameof(rule.value)));

    Assert.Equal(expectedExMessage, ex.Message);
    Assert.Equal(operatorPropertyName, ex.Data[ErrorDataKeys.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.Json]);
    Assert.Equal(JsonValueKind.String, ex.Data[ErrorDataKeys.ExpectedType]);
    Assert.Equal(JsonValueKind.Number, ex.Data[ErrorDataKeys.CurrentType]);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Parse_ShouldThrowArgumentException_InvalidFieldOperatorValue(string op)
  {
    // null - invalid json property type
    var rule = new
    {
      op,
      field = "2",
      value = "3",
    };
    var operatorPropertyName = nameof(rule.op);
    var ruleJson = JsonSerializer.Serialize(rule);
    var ruleJsonElement = JsonSerializer.SerializeToElement(rule);
    var expectedExMessage = string.Format(ErrorMessages.JsonPropertyEmpty, operatorPropertyName);
    var mapper = new ExpressionFieldMapper()
      .Add(
        rule.field,
        new ExpressionField
        {
          Name = rule.field,
          GetValue = je => Expression.Constant(je.Value.GetString()),
        });

    var ex = Assert.Throws<ArgumentException>(() => ExpressionRule.Parse(
      ruleJsonElement,
      mapper,
      operatorPropertyName,
      nameof(rule.field),
      nameof(rule.value)));

    Assert.Equal(expectedExMessage, ex.Message);
    Assert.Equal(operatorPropertyName, ex.Data[ErrorDataKeys.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.Json]);
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
    Assert.Equal(invalidKey, ex.Data[ErrorDataKeys.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.Json]);
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
    Assert.Equal(fieldPropertyName, ex.Data[ErrorDataKeys.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.Json]);
    Assert.Equal(JsonValueKind.String, ex.Data[ErrorDataKeys.ExpectedType]);
    Assert.Equal(JsonValueKind.Number, ex.Data[ErrorDataKeys.CurrentType]);
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Parse_ShouldThrowArgumentException_InvalidFieldPropertyValue(string field)
  {
    // null - invalid json property type
    var rule = new
    {
      op = "1",
      field,
      value = "3",
    };
    var fieldPropertyName = nameof(rule.field);
    var ruleJson = JsonSerializer.Serialize(rule);
    var ruleJsonElement = JsonSerializer.SerializeToElement(rule);
    var expectedExMessage = string.Format(ErrorMessages.JsonPropertyEmpty, fieldPropertyName);
    var mapper = new ExpressionFieldMapper()
      .Add(
        rule.field,
        new ExpressionField
        {
          Name = rule.field,
          GetValue = je => Expression.Constant(je.Value.GetString()),
        });

    var ex = Assert.Throws<ArgumentException>(() => ExpressionRule.Parse(
      ruleJsonElement,
      mapper,
      nameof(rule.op),
      fieldPropertyName,
      nameof(rule.value)));

    Assert.Equal(expectedExMessage, ex.Message);
    Assert.Equal(fieldPropertyName, ex.Data[ErrorDataKeys.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.Json]);
  }

  [Fact]
  public void Parse_ShouldThrowKeyNotFoundException_InvalidValuePropertyName()
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
      nameof(rule.field),
      invalidKey));

    Assert.Equal(expectedExMessage, ex.Message);
    Assert.Equal(invalidKey, ex.Data[ErrorDataKeys.Property]);
    Assert.Equal(ruleJson, ex.Data[ErrorDataKeys.Json]);
  }

  [Fact]
  public void CreateFilterExpression()
  {
    const string expectedFilterString = "(user.Id == 0)";
    var rule = new
    {
      op = "=",
      field = "user",
      value = 0,
    };
    var fieldPropertyName = rule.field;
    var jsonElement = JsonSerializer.SerializeToElement(rule);
    var opMapper = new ExpressionOperatorMapper()
      .Add(rule.op, Expression.Equal);
    var fieldMapper = new ExpressionFieldMapper()
      .Add(
        fieldPropertyName,
        new ExpressionField
        {
          Name = nameof(User.Id),
          GetValue = je => Expression.Constant(rule.value),
        });
    var par = Expression.Parameter(typeof(User), fieldPropertyName);
    var ruleParsed = ExpressionRule.Parse(
      jsonElement,
      fieldMapper,
      nameof(rule.op),
      nameof(rule.field),
      nameof(rule.value));

    var filter = ruleParsed.CreateFilterExpression(par, opMapper);
    var lambda = Expression.Lambda<Func<User, bool>>(filter, par).Compile();

    Assert.Equal(expectedFilterString, filter.ToString());
    Assert.True(lambda(new User { Id = 0 }));
    Assert.False(lambda(new User { Id = 1 }));
    Assert.False(lambda(new User { Id = -1 }));
  }
}
