namespace ArgsParser.Attributes;

public class OptionAttribute : ArgAttribute
{
    public OptionAttribute(string name)
    {
        Name = name;
    }

    public OptionAttribute(char alias, string name) : this(name)
    {
        Alias = alias;
    }

    public char? Alias { get; set; }
    public string Name { get; set; }
    public int? Min { get; set; }
    public int? Max { get; set; }
    public string HelpText { get; set; } = string.Empty;
}
