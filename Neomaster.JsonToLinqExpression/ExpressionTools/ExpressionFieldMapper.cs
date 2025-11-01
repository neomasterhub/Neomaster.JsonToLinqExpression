namespace Neomaster.JsonToLinq;

public class ExpressionFieldMapper
{
  public readonly Dictionary<string, ExpressionField> Fields = [];

  public ExpressionFieldMapper Add(string srcFieldName, ExpressionField dstField)
  {
    Fields.Add(srcFieldName, dstField);

    return this;
  }
}
