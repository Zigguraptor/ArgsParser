using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser.newS;

public class OptionPack
{
    public readonly PropertyInfo PropertyInfo;
    public readonly OptionAttribute OptionAttribute;
    public readonly GroupAttribute? GroupAttribute;

    public OptionPack(PropertyInfo propertyInfo, OptionAttribute optionAttribute, GroupAttribute? groupAttribute)
    {
        PropertyInfo = propertyInfo;
        OptionAttribute = optionAttribute;
        GroupAttribute = groupAttribute;
    }
}
