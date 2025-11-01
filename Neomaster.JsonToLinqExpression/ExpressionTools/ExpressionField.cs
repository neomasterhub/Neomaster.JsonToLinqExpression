using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinq;

public class ExpressionField
{
  public string Name { get; set; }
  public Func<JsonElement?, ConstantExpression> GetValue { get; set; }
}
