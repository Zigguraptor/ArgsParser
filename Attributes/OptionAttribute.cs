namespace ArgsParser.Attributes;

public class OptionAttribute : ArgAttribute
{
    public char? Alias { get; set; }
    public string Name { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public string HelpText { get; set; } = string.Empty;

    public OptionAttribute(string name)
    {
        Name = name;
    }

    public OptionAttribute(string name, int min) : this(name)
    {
        Min = min;
    }

    public OptionAttribute(string name, int min, int max) : this(name, min)
    {
        Max = max;
    }

    public OptionAttribute(char alias, string name) : this(name)
    {
        Alias = alias;
    }

    public OptionAttribute(char alias, string name, int min) : this(alias, name)
    {
        Min = min;
    }

    public OptionAttribute(char alias, string name, int min, int max) : this(alias, name, min)
    {
        Max = max;
    }
}
