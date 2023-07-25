using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser.newS;

public class ValuePack
{
    public readonly PropertyInfo PropertyInfo;
    public readonly ValueAttribute ValueAttribute;
    public readonly GroupAttribute? GroupAttribute;

    public ValuePack(PropertyInfo propertyInfo, ValueAttribute valueAttribute, GroupAttribute? groupAttribute)
    {
        PropertyInfo = propertyInfo;
        ValueAttribute = valueAttribute;
        GroupAttribute = groupAttribute;
    }
}
