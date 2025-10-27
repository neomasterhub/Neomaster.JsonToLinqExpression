using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinqExpression.UnitTests;

public class ExpressionHelperUnitTests
{
  [Theory]
  [InlineData(null, null, null)]
  [InlineData(null, true, null)]
  [InlineData(null, false, false)] // SQL
  [InlineData(true, null, null)]
  [InlineData(false, null, false)] // SQL
  [InlineData(true, true, true)]
  [InlineData(false, false, false)]
  [InlineData(false, true, false)]
  [InlineData(true, false, false)]
  public void CreateExpressionBind_AndAlso(bool? left, bool? right, bool? result)
  {
    CreateExpressionBindTest(
      "and",
      Expression.AndAlso,
      expr => Expression.Lambda<Func<bool?>>(expr),
      Expression.Constant(left, typeof(bool?)),
      Expression.Constant(right, typeof(bool?)),
      result);
  }

  private static void CreateExpressionBindTest<TResult>(
    string logicOperator,
    Consts.ExpressionBind logicOperatorExpression,
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
      operatorMapper);

    var binded = bind(leftExpression, rightExpression);
    var lambda = buildLambda(binded).Compile();

    Assert.Equal(expectedLambdaResult, lambda.DynamicInvoke());
  }
}
