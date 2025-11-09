namespace Neomaster.JsonToLinq.UnitTests;

public class PropertiesPublicGetSet
{
  public bool Bool { get; set; }
  public bool BoolNullable { get; set; }
  public int Int { get; set; }
  public int? IntNullable { get; set; }
  public string String { get; set; }
  public ConsoleColor Enum { get; set; }
  public ConsoleColor? EnumNullable { get; set; }
  public DateTime DateTime { get; set; }
  public DateTime? DateTimeNullable { get; set; }
}
