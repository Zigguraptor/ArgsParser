using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser;

public class ValuePack
{
    public readonly PropertyInfo PropertyInfo;

    public readonly ValueAttribute ValueAttribute;

    //TODO check this
    public readonly GroupAttribute? GroupAttribute;

    public ValuePack(PropertyInfo propertyInfo, ValueAttribute valueAttribute, GroupAttribute? groupAttribute)
    {
        PropertyInfo = propertyInfo;
        ValueAttribute = valueAttribute;
        GroupAttribute = groupAttribute;
    }
}
