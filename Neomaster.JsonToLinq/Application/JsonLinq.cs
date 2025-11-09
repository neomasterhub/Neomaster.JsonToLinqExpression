using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinq;

/// <summary>
/// Provides methods to convert JSON-based query definitions into LINQ expressions.
/// </summary>
public static class JsonLinq
{
  private static readonly JsonLinqOptions _defaultOptions = new();

  /// <summary>
  /// Parses a JSON document into a LINQ expression filter for <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">Type of objects to filter.</typeparam>
  /// <param name="doc">JSON filter definition.</param>
  /// <param name="fieldMapper">Maps JSON field names to <see cref="ExpressionField"/> definitions.</param>
  /// <param name="options">Optional parser settings. Uses defaults if null.</param>
  /// <returns>An <see cref="Expression{Func}"/> representing the filter for <typeparamref name="T"/>.</returns>
  public static Expression<Func<T, bool>> ParseToFilterExpression<T>(
    JsonDocument doc,
    ExpressionFieldMapper fieldMapper = null,
    JsonLinqOptions options = null)
  {
    options ??= _defaultOptions;

    fieldMapper ??= ExpressionFieldMapperFactory
      .CreateForPublicProperties<T>(options.ConvertPropertyNameForJson);

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
