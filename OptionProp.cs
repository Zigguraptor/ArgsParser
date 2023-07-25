using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser;

public class OptionProp
{
    public readonly PropertyInfo PropertyInfo;
    public readonly OptionAttribute OptionAttribute;
    public readonly string? Group;

    public OptionProp(PropertyInfo propertyInfo, OptionAttribute optionAttribute, string? group)
    {
        PropertyInfo = propertyInfo;
        OptionAttribute = optionAttribute;
        Group = group;
    }
}
