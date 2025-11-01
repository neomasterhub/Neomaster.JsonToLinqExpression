using System.Linq.Expressions;
using System.Text.Json;

namespace Neomaster.JsonToLinq.UnitTests;

public class ExpressionFieldUnitTests
{
  [Fact]
  public void GetValue()
  {
    GetValueTest(typeof(int), je => je?.GetInt32(), 1);
    GetValueTest(typeof(int?), je => je?.GetInt32(), null);
    GetValueTest(typeof(int?), je => je?.GetInt32(), (int?)1);
    GetValueTest(typeof(string), je => je?.GetString(), null);
    GetValueTest(typeof(string), je => je?.GetString(), "1");
    GetValueTest(typeof(ConsoleColor), je => je?.Deserialize<ConsoleColor>(), ConsoleColor.Red);
    GetValueTest(typeof(ConsoleColor?), je => je?.Deserialize<ConsoleColor>(), (ConsoleColor?)null);
    GetValueTest(typeof(ConsoleColor?), je => je?.Deserialize<ConsoleColor>(), (ConsoleColor?)ConsoleColor.Red);
    GetValueTest(typeof(DateTime), je => je?.Deserialize<DateTime>(), DateTime.Now);
    GetValueTest(typeof(DateTime?), je => je?.Deserialize<DateTime>(), (DateTime?)null);
    GetValueTest(typeof(DateTime?), je => je?.Deserialize<DateTime>(), (DateTime?)DateTime.Now);
  }

  private static void GetValueTest(
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
