using System.Linq.Expressions;

namespace Neomaster.JsonToLinqExpression;

public static class Consts
{
  public delegate Expression ExpressionBind(Expression left, Expression right);

  public static class ExpressionOperatorMappers
  {
    public static readonly ExpressionOperatorMapper Default = new ExpressionOperatorMapper()
      .Add("or", Expression.Or)
      .Add("and", Expression.And)
      .Add("eq", Expression.Equal)
      .Add("neq", Expression.NotEqual)
      .Add("gt", Expression.GreaterThan)
      .Add("gte", Expression.GreaterThanOrEqual)
      .Add("lt", Expression.LessThan)
      .Add("lte", Expression.LessThanOrEqual)
      // TODO: contains, does not contain
      ;
  }
}
