using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinqExpression;

public class ExpressionField
{
  public string Name { get; set; }
  public string Operator { get; set; }
  public Func<JsonElement?, ConstantExpression> GetValue { get; set; }
}
