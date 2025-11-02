using System.Linq.Expressions;

namespace Neomaster.JsonToLinq;

public static class Consts
{
  public delegate Expression ExpressionBind(Expression left, Expression right);

  public static class ExpressionOperatorMappers
  {
    public static readonly ExpressionOperatorMapper Default = new ExpressionOperatorMapper()
      .Add("&", Expression.And)
      .Add("&&", Expression.AndAlso)
      .Add("|", Expression.Or)
      .Add("||", Expression.OrElse)
      .Add("=", Expression.Equal)
      .Add("!=", Expression.NotEqual)
      .Add(">", Expression.GreaterThan)
      .Add(">=", Expression.GreaterThanOrEqual)
      .Add("<", Expression.LessThan)
      .Add("<=", Expression.LessThanOrEqual)
      // TODO: contains, does not contain
      ;
  }

  public static class ExpressionBindBuilders
  {
    public static readonly Func<ExpressionBind, Expression, Expression, Expression> Sql = (bind, left, right) =>
      left == null
      ? right
      : bind(left, right);

    public static readonly Func<ExpressionBind, Expression, Expression, Expression> NullAsFalse = (bind, left, right) =>
      left == null
      ? ExpressionHelper.CoalesceNullFalse(right)
      : bind(ExpressionHelper.CoalesceNullFalse(left), ExpressionHelper.CoalesceNullFalse(right));
  }

  public static class ErrorMessages
  {
    public const string JsonPropertyNotFound = "Json property \"{0}\" not found.";
    public const string JsonPropertyNotType = "Json property \"{0}\" must be of type \"{1}\".";
    public const string JsonPropertyEmpty = "Json property \"{0}\" is empty.";
  }

  public static class ErrorDataKeys
  {
    public const string Json = nameof(Json);
    public const string Property = nameof(Property);
    public const string ExpectedType = nameof(ExpectedType);
    public const string CurrentType = nameof(CurrentType);
  }

  public static class JsonLinqOptionsPropertyNames
  {
    public const string LogicOperator = "Logic";
    public const string Rules = "Rules";
    public const string Operator = nameof(ExpressionRule.Operator);
    public const string Field = nameof(ExpressionRule.Field);
    public const string Value = nameof(ExpressionRule.Value);
  }
}
