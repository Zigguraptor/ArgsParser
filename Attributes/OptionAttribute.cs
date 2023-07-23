namespace ArgsParser.Attributes;

public class OptionAttribute : Attribute, IArgAttribute
{
    public OptionAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
    public string? GroupName { get; set; }
    public string? HelpText { get; set; }
}
