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
