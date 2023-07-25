namespace ArgsParser.Attributes;

public class ValueAttribute : ArgAttribute
{
    public int Index { get; set; }
    public int? Max { get; set; }
    public int? Min { get; set; }
    public string HelpText { get; set; } = string.Empty;
}
