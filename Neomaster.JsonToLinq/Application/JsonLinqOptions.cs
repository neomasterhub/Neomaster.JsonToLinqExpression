using System.Linq.Expressions;
using static Neomaster.JsonToLinq.Consts;

namespace Neomaster.JsonToLinq;

/// <summary>
/// Provides configuration options for JSON-to-LINQ expression parsing.
/// </summary>
public record JsonLinqOptions
{
  public string LogicOperatorPropertyName { get; set; } = JsonLinqOptionsPropertyNames.LogicOperator;
  public string RulesPropertyName { get; set; } = JsonLinqOptionsPropertyNames.Rules;
  public string OperatorPropertyName { get; set; } = JsonLinqOptionsPropertyNames.Operator;
  public string FieldPropertyName { get; set; } = JsonLinqOptionsPropertyNames.Field;
  public string ValuePropertyName { get; set; } = JsonLinqOptionsPropertyNames.Value;
  public ExpressionOperatorMapper OperatorMapper { get; set; } = ExpressionOperatorMappers.Default;
  public Func<ExpressionBind, Expression, Expression, Expression> BindBuilder { get; set; } = ExpressionBindBuilders.Sql;
}
