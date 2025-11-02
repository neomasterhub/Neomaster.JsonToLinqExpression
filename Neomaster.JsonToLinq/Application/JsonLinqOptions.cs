using System.Linq.Expressions;
using static Neomaster.JsonToLinq.Consts;

namespace Neomaster.JsonToLinq;

/// <summary>
/// Provides configuration options for JSON-to-LINQ expression parsing.
/// </summary>
public record JsonLinqOptions
{
  /// <summary>
  /// Gets or sets the JSON property name that specifies the logical operator used to combine rules.
  /// The default value is <see cref="JsonLinqOptionsPropertyNames.LogicOperator"/>.
  /// </summary>
  public string LogicOperatorPropertyName { get; set; } = JsonLinqOptionsPropertyNames.LogicOperator;

  /// <summary>
  /// Gets or sets the JSON property name that contains the collection of rules.
  /// The default value is <see cref="JsonLinqOptionsPropertyNames.Rules"/>.
  /// </summary>
  public string RulesPropertyName { get; set; } = JsonLinqOptionsPropertyNames.Rules;

  /// <summary>
  /// Gets or sets the JSON property name that specifies the operator for a single rule.
  /// The default value is <see cref="JsonLinqOptionsPropertyNames.Operator"/>.
  /// </summary>
  public string OperatorPropertyName { get; set; } = JsonLinqOptionsPropertyNames.Operator;

  /// <summary>
  /// Gets or sets the JSON property name that specifies the field name in a rule.
  /// The default value is <see cref="JsonLinqOptionsPropertyNames.Field"/>.
  /// </summary>
  public string FieldPropertyName { get; set; } = JsonLinqOptionsPropertyNames.Field;

  /// <summary>
  /// Gets or sets the JSON property name that specifies the comparison value in a rule.
  /// The default value is <see cref="JsonLinqOptionsPropertyNames.Value"/>.
  /// </summary>
  public string ValuePropertyName { get; set; } = JsonLinqOptionsPropertyNames.Value;

  /// <summary>
  /// Gets or sets the operator mapper used to convert string-based operators from JSON into LINQ expressions.
  /// The default value is <see cref="ExpressionOperatorMappers.Default"/>.
  /// </summary>
  public ExpressionOperatorMapper OperatorMapper { get; set; } = ExpressionOperatorMappers.Default;

  /// <summary>
  /// Gets or sets the delegate that specifies how two expressions are combined with a logical operator.
  /// The default value is <see cref="ExpressionBindBuilders.Sql"/>.
  /// </summary>
  public Func<ExpressionBind, Expression, Expression, Expression> BindBuilder { get; set; } = ExpressionBindBuilders.Sql;
}
