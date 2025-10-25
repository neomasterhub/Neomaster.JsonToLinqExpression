using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinqExpression;

public class ExpressionRule
{
  public string Operator { get; set; }
  public string Field { get; set; }
  public JsonElement? Value { get; set; }
  public ConstantExpression ValueConstantExpression { get; set; }
}
