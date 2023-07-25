using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser;

public class ValueProp
{
    public readonly PropertyInfo PropertyInfo;
    public readonly ValueAttribute ValueAttribute;

    public ValueProp(PropertyInfo propertyInfo, ValueAttribute valueAttribute)
    {
        PropertyInfo = propertyInfo;
        ValueAttribute = valueAttribute;
    }
}
