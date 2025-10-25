using System.Linq.Expressions;

namespace Neomaster.JsonToLinqExpression;

public class ExpressionOperatorMapper
{
  public readonly Dictionary<string, Consts.ExpressionBind> Operators = [];

  public ExpressionOperatorMapper(string or, string and)
  {
    Operators.Add(or, Expression.Or);
    Operators.Add(and, Expression.And);
  }

  public ExpressionOperatorMapper Add(string op, Consts.ExpressionBind bind)
  {
    Operators.Add(op, bind);

    return this;
  }
}
