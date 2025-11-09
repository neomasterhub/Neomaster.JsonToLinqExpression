namespace Neomaster.JsonToLinq.UnitTests;

public class PropertiesPublicGetSet
{
  public bool Bool { get; set; } = true;
  public bool? BoolNullable1 { get; set; } = true;
  public bool? BoolNullable2 { get; set; } = null;
  public int Int { get; set; } = 1;
  public int? IntNullable1 { get; set; } = 2;
  public int? IntNullable2 { get; set; } = null;
  public string String1 { get; set; } = "1";
  public string String2 { get; set; } = null;
  public ConsoleColor Enum { get; set; } = ConsoleColor.Blue;
  public ConsoleColor? EnumNullable1 { get; set; } = ConsoleColor.Red;
  public ConsoleColor? EnumNullable2 { get; set; } = null;
  public DateTime DateTime { get; set; } = new DateTime(2000, 01, 01);
  public DateTime? DateTimeNullable1 { get; set; } = new DateTime(2000, 01, 02);
  public DateTime? DateTimeNullable2 { get; set; } = null;
}
