using System.Linq.Expressions;

namespace Neomaster.JsonToLinqExpression;

public static class Consts
{
  public delegate Expression ExpressionBind(Expression left, Expression right);
}
