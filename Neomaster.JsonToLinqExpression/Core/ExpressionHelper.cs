using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinqExpression;

public static class ExpressionHelper
{
  public static Expression<Func<T, bool>> ParseFilterExpression<T>(
    JsonDocument doc,
    ExpressionFieldMapper fieldMapper,
    ExpressionOperatorMapper operatorMapper,
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
      operatorMapper,
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
    ExpressionOperatorMapper operatorMapper,
    string logicOperatorPropertyName,
    string rulesPropertyName,
    string operatorPropertyName,
    string fieldPropertyName,
    string valuePropertyName)
  {
    var bind = CreateExpressionBind(condition, logicOperatorPropertyName, operatorMapper);
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
          operatorMapper,
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
    string logicOperatorPropertyName,
    ExpressionOperatorMapper operatorMapper)
  {
    var logicOperator = condition.GetProperty(logicOperatorPropertyName).GetString();
    var logicBind = operatorMapper.Operators[logicOperator];

    return (left, right) => left == null
      ? right
      : logicBind(left, right);
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
