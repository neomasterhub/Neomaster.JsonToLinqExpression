using System.Linq.Expressions;
using System.Text.Json;
using static Neomaster.JsonToLinqExpression.Consts;

namespace Neomaster.JsonToLinqExpression.UnitTests;

public class ExpressionHelperUnitTests
{
  [Theory]
  [InlineData(null, null, null)]
  [InlineData(null, true, null)]
  [InlineData(null, false, false)]
  [InlineData(true, null, null)]
  [InlineData(true, true, true)]
  [InlineData(true, false, false)]
  [InlineData(false, null, false)]
  [InlineData(false, true, false)]
  [InlineData(false, false, false)]
  public void CreateExpressionBind_Sql_AndAlso(bool? left, bool? right, bool? result)
  {
    CreateExpressionBindTest(
      ExpressionBindBuilders.Sql,
      "and",
      Expression.AndAlso,
      expr => Expression.Lambda<Func<bool?>>(expr),
      Expression.Constant(left, typeof(bool?)),
      Expression.Constant(right, typeof(bool?)),
      result);
  }

  [Theory]
  [InlineData(null, null, null)]
  [InlineData(null, true, true)]
  [InlineData(null, false, null)]
  [InlineData(true, null, true)]
  [InlineData(true, true, true)]
  [InlineData(true, false, true)]
  [InlineData(false, null, null)]
  [InlineData(false, true, true)]
  [InlineData(false, false, false)]
  public void CreateExpressionBind_Sql_OrElse(bool? left, bool? right, bool? result)
  {
    CreateExpressionBindTest(
      ExpressionBindBuilders.Sql,
      "or",
      Expression.OrElse,
      expr => Expression.Lambda<Func<bool?>>(expr),
      Expression.Constant(left, typeof(bool?)),
      Expression.Constant(right, typeof(bool?)),
      result);
  }

  [Theory]
  [InlineData(null, null, false)]
  [InlineData(null, true, false)]
  [InlineData(null, false, false)]
  [InlineData(true, null, false)]
  [InlineData(true, true, true)]
  [InlineData(true, false, false)]
  [InlineData(false, null, false)]
  [InlineData(false, true, false)]
  [InlineData(false, false, false)]
  public void CreateExpressionBind_NullAsFalse_AndAlso(bool? left, bool? right, bool? result)
  {
    CreateExpressionBindTest(
      ExpressionBindBuilders.NullAsFalse,
      "and",
      Expression.AndAlso,
      expr => Expression.Lambda<Func<bool>>(expr),
      Expression.Constant(left, typeof(bool?)),
      Expression.Constant(right, typeof(bool?)),
      result);
  }

  [Theory]
  [InlineData(null, null, false)]
  [InlineData(null, true, true)]
  [InlineData(null, false, false)]
  [InlineData(true, null, true)]
  [InlineData(true, true, true)]
  [InlineData(true, false, true)]
  [InlineData(false, null, false)]
  [InlineData(false, true, true)]
  [InlineData(false, false, false)]
  public void CreateExpressionBind_NullAsFalse_OrElse(bool? left, bool? right, bool? result)
  {
    CreateExpressionBindTest(
      ExpressionBindBuilders.NullAsFalse,
      "or",
      Expression.OrElse,
      expr => Expression.Lambda<Func<bool>>(expr),
      Expression.Constant(left, typeof(bool?)),
      Expression.Constant(right, typeof(bool?)),
      result);
  }

  [Theory]
  [InlineData(null, false)]
  [InlineData(false, false)]
  [InlineData(true, true)]
  public void CoalesceNullFalse(bool? input, bool output)
  {
    var inputExpression = Expression.Constant(input, typeof(bool?));

    var outputExpression = Expression.Constant(output, typeof(bool));
    var outputFunc = Expression.Lambda<Func<bool>>(outputExpression).Compile();

    Assert.Equal(output, outputFunc());
  }

  [Fact]
  public void EnumerateExpressionRules_ShouldEnumerateRootRules()
  {
    var rules = Enumerable
      .Range(1, 3)
      .Select(n => new
      {
        X = $"1.{n}",
        Rules = new[] { new { X = $"1.{n}.1" } },
      })
      .ToArray();
    var tree = new
    {
      X = "1",
      Rules = rules,
    };
    var treeJsonElement = JsonSerializer.SerializeToElement(tree);

    var enumeratedRules = ExpressionHelper.EnumerateExpressionRules(treeJsonElement, nameof(tree.Rules)).ToArray();

    Assert.Equal(rules.Length, enumeratedRules.Length);
    Assert.Equal(JsonSerializer.Serialize(rules), JsonSerializer.Serialize(enumeratedRules));
  }

  private static void CreateExpressionBindTest<TResult>(
    Func<ExpressionBind, Expression, Expression, Expression> buildBind,
    string logicOperator,
    ExpressionBind logicOperatorExpression,
    Func<Expression, LambdaExpression> buildLambda,
    Expression leftExpression,
    Expression rightExpression,
    TResult expectedLambdaResult)
  {
    var condition = new { logic = logicOperator };
    var logicOperatorPropertyName = nameof(condition.logic);
    var operatorMapper = new ExpressionOperatorMapper().Add(logicOperator, logicOperatorExpression);
    var conditionJsonElement = JsonSerializer.SerializeToElement(condition);
    var bind = ExpressionHelper.CreateExpressionBind(
      conditionJsonElement,
      logicOperatorPropertyName,
      operatorMapper,
      buildBind);

    var binded = bind(leftExpression, rightExpression);
    var lambda = buildLambda(binded).Compile();

    Assert.Equal(expectedLambdaResult, lambda.DynamicInvoke());
  }
}
