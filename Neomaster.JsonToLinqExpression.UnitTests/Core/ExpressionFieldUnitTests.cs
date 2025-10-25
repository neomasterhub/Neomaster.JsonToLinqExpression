using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinqExpression.UnitTests;

public class ExpressionFieldUnitTests
{
  [Fact]
  public void GetValue()
  {
    GetValue_T(typeof(int), je => je?.GetInt32(), 1);
    GetValue_T(typeof(int?), je => je?.GetInt32(), null);
    GetValue_T(typeof(int?), je => je?.GetInt32(), (int?)1);
    GetValue_T(typeof(string), je => je?.GetString(), null);
    GetValue_T(typeof(string), je => je?.GetString(), "1");
    GetValue_T(typeof(ConsoleColor), je => je?.Deserialize<ConsoleColor>(), ConsoleColor.Red);
    GetValue_T(typeof(ConsoleColor?), je => je?.Deserialize<ConsoleColor>(), (ConsoleColor?)null);
    GetValue_T(typeof(ConsoleColor?), je => je?.Deserialize<ConsoleColor>(), (ConsoleColor?)ConsoleColor.Red);
    GetValue_T(typeof(DateTime), je => je?.Deserialize<DateTime>(), DateTime.Now);
    GetValue_T(typeof(DateTime?), je => je?.Deserialize<DateTime>(), (DateTime?)null);
    GetValue_T(typeof(DateTime?), je => je?.Deserialize<DateTime>(), (DateTime?)DateTime.Now);
  }

  private static void GetValue_T(
    Type expectedType,
    Func<JsonElement?, object> convertJsonElement,
    object expectedValue)
  {
    var jsonElement = expectedValue == null
      ? (JsonElement?)null
      : JsonSerializer.SerializeToElement(expectedValue);
    var sut = new ExpressionField
    {
      GetValue = je => Expression.Constant(convertJsonElement(je), expectedType),
    };

    var actual = sut.GetValue(jsonElement);

    Assert.Equal(expectedType, actual.Type);
    Assert.Equal(expectedValue, actual.Value);
  }
}
