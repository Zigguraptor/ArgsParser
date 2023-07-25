namespace ArgsParser.Attributes;

public class ValueAttribute : ArgAttribute
{
    public int Index { get; set; }
    public int? Max { get; set; }
    public int? Min { get; set; }
    public string HelpText { get; set; } = string.Empty;

    public ValueAttribute(int index)
    {
        Index = index;
    }

    public ValueAttribute(int index, int min) : this(index)
    {
        Min = min;
    }

    public ValueAttribute(int index, int min, int max) : this(index, min)
    {
        Max = max;
    }
}
