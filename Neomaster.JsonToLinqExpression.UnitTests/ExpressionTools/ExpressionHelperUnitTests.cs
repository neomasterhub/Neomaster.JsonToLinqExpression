using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinqExpression.UnitTests;

public class ExpressionHelperUnitTests
{
  public static void CreateExpressionBindTest<TResult>(
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
