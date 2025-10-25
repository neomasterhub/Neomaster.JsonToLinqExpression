using System.Linq.Expressions;
using System.Text.Json;
using static Neomaster.JsonToLinqExpression.Consts;

namespace Neomaster.JsonToLinqExpression;

public class ExpressionHelper
{
  public static Expression<Func<T, bool>> ParseFilterExpression<T>(
    JsonDocument doc,
    ExpressionFieldMapper fieldMapper,
    string logicOperatorPropertyName,
    string rulesPropertyName,
    string operatorPropertyName,
    string fieldPropertyName,
    string valuePropertyName)
  {
    var par = Expression.Parameter(typeof(T));
    var condition = ParseExpression<T>(
      doc.RootElement,
      par,
      fieldMapper,
      logicOperatorPropertyName,
      rulesPropertyName,
      operatorPropertyName,
      fieldPropertyName,
      valuePropertyName);

    if (condition.CanReduce)
    {
      condition = condition.ReduceAndCheck();
    }

    var exp = Expression.Lambda<Func<T, bool>>(condition, par);

    return exp;
  }

  public static Expression ParseExpression<T>(
    JsonElement condition,
    ParameterExpression par,
    ExpressionFieldMapper fieldMapper,
    string logicOperatorPropertyName,
    string rulesPropertyName,
    string operatorPropertyName,
    string fieldPropertyName,
    string valuePropertyName)
  {
    var bind = CreateExpressionBind(condition, logicOperatorPropertyName);
    var rules = EnumerateExpressionRules(condition, rulesPropertyName);

    Expression left = null;

    foreach (var r in rules)
    {
      Expression right = null;

      if (r.TryGetProperty(logicOperatorPropertyName, out _))
      {
        right = ParseExpression<T>(
          r,
          par,
          fieldMapper,
          logicOperatorPropertyName,
          rulesPropertyName,
          operatorPropertyName,
          fieldPropertyName,
          valuePropertyName);

        left = bind(left, right);

        continue;
      }

      var rule = ExpressionRule.Parse(
        r,
        fieldMapper,
        operatorPropertyName,
        fieldPropertyName,
        valuePropertyName);

      right = rule.CreateFilterExpression(par);
      left = bind(left, right);
    }

    return left;
  }

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
