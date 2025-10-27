using System.Linq.Expressions;

namespace Neomaster.JsonToLinqExpression;

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
  }
}
