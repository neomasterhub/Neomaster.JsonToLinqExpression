using System.Linq.Expressions;
using System.Text.Json;
using static Neomaster.JsonToLinq.Consts;

namespace Neomaster.JsonToLinq;

/// <summary>
/// Provides methods to convert JSON-based query definitions into LINQ expressions.
/// </summary>
public static class JsonLinq
{
  public static Expression<Func<T, bool>> ParseToFilterExpression<T>(
    JsonDocument doc,
    ExpressionFieldMapper fieldMapper,
    string logicOperatorPropertyName = "Logic",
    string rulesPropertyName = "Rules",
    string operatorPropertyName = nameof(ExpressionRule.Operator),
    string fieldPropertyName = nameof(ExpressionRule.Field),
    string valuePropertyName = nameof(ExpressionRule.Value),
    ExpressionOperatorMapper operatorMapper = null,
    Func<ExpressionBind, Expression, Expression, Expression> bindBuilder = null)
  {
    operatorMapper ??= ExpressionOperatorMappers.Default;
    bindBuilder ??= ExpressionBindBuilders.Sql;

    return ExpressionHelper.ParseToFilterExpression<T>(
      doc,
      fieldMapper,
      operatorMapper,
      logicOperatorPropertyName,
      rulesPropertyName,
      operatorPropertyName,
      fieldPropertyName,
      valuePropertyName,
      bindBuilder);
  }
}
