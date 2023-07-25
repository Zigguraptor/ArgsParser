namespace ArgsParser.Attributes;

public class GroupAttribute : ArgAttribute
{
    public GroupAttribute(string groupName, bool required = false)
    {
        Required = required;
        GroupName = groupName;
    }

    public bool Required { get; set; }
    public string GroupName { get; set; }
}
