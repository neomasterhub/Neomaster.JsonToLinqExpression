using System.Linq.Expressions;
using System.Text.Json;
using static Neomaster.JsonToLinqExpression.Consts;

namespace Neomaster.JsonToLinqExpression;

public class ExpressionHelper
{
  public static Func<Expression, Expression, Expression> CreateExpressionBind(
    JsonElement condition,
    string logicOperatorPropertyName)
  {
    var logicOperator = condition.GetProperty(logicOperatorPropertyName).GetString();

    var binaryBind = logicOperator == "and" // TODO: Use ExpressionOperatorMapper
      ? (ExpressionBind)Expression.And
      : Expression.Or;

    return (left, right) => left == null
      ? right
      : binaryBind(left, right);
  }
}
