namespace Neomaster.JsonToLinqExpression;

public class ExpressionOperatorMapper
{
  public readonly Dictionary<string, Consts.ExpressionBind> Operators = [];

  public ExpressionOperatorMapper Add(string op, Consts.ExpressionBind bind)
  {
    Operators.Add(op, bind);

    return this;
  }
}
