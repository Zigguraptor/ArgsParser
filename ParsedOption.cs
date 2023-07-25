using System.Reflection;

namespace ArgsParser;

public class ParsedOption
{
    public readonly PropertyInfo PropertyInfo;
    public readonly List<string> Values = new();

    public ParsedOption(PropertyInfo propertyInfo)
    {
        PropertyInfo = propertyInfo;
    }
}
