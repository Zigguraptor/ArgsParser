namespace ArgsParser.Attributes;

public class OptionGroupAttribute : ArgAttribute
{
    public OptionGroupAttribute(string groupName, bool required = false)
    {
        Required = required;
        GroupName = groupName;
    }

    public bool Required { get; set; }
    public string GroupName { get; set; }
}
