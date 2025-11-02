using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinq;

/// <summary>
/// Provides methods to convert JSON-based query definitions into LINQ expressions.
/// </summary>
public static class JsonLinq
{
  private static readonly JsonLinqOptions _defaultOptions = new();

  public static Expression<Func<T, bool>> ParseToFilterExpression<T>(
    JsonDocument doc,
    ExpressionFieldMapper fieldMapper,
    JsonLinqOptions options = null)
  {
    options ??= _defaultOptions;

    return ExpressionHelper.ParseToFilterExpression<T>(
      doc,
      fieldMapper,
      options.OperatorMapper,
      options.LogicOperatorPropertyName,
      options.RulesPropertyName,
      options.OperatorPropertyName,
      options.FieldPropertyName,
      options.ValuePropertyName,
      options.BindBuilder);
  }
}
