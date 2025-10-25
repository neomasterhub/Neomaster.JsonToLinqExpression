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

  public static IEnumerable<JsonElement> EnumerateExpressionRules(
    JsonElement condition,
    string rulesPropertyName)
  {
    var rules = condition.GetProperty(rulesPropertyName).EnumerateArray();

    foreach (var r in rules)
    {
      yield return r;
    }
  }
}
