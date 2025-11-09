namespace Neomaster.JsonToLinq;

/// <summary>
/// Naming strategy for property name conversion for JSON.
/// </summary>
public enum JsonNamingStrategy
{
  /// <summary>
  /// Use property names as-is.
  /// </summary>
  AsIs = 1,

  /// <summary>
  /// Convert property names to camelCase.
  /// </summary>
  Camel = 2,

  /// <summary>
  /// Convert property names to kebab-case.
  /// </summary>
  Kebab = 3,

  /// <summary>
  /// Convert property names to KEBAB-CASE.
  /// </summary>
  KebabUpper = 4,

  /// <summary>
  /// Convert property names to snake_case.
  /// </summary>
  Snake = 5,

  /// <summary>
  /// Convert property names to SNAKE_CASE.
  /// </summary>
  SnakeUpper = 6,
}
