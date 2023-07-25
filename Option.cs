using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser;

public class Option
{
    public readonly bool IsIEnumerable;
    public bool IsChecked { get; private set; }
    public List<string> Values { get; }
    public PropertyInfo PropertyInfo { get; }
    private readonly GroupAttribute? _groupAttribute;
    private readonly OptionAttribute _optionAttribute;

    public char? Alias => _optionAttribute.Alias;
    public string Name => _optionAttribute.Name;
    public int? Max => _optionAttribute.Max;
    public int? Min => _optionAttribute.Min;

    public string? Group => _groupAttribute?.GroupName;
    public bool? GroupRequired => _groupAttribute?.Required;

    public Option(PropertyInfo propertyInfo, OptionAttribute optionAttribute, GroupAttribute groupAttribute) :
        this(propertyInfo, optionAttribute)
    {
        _groupAttribute = groupAttribute;
    }

    public Option(PropertyInfo propertyInfo, OptionAttribute optionAttribute)
    {
        PropertyInfo = propertyInfo;
        _optionAttribute = optionAttribute;
        _groupAttribute = null;
        Values = Max != null ? new List<string>((int)Max) : new List<string>();
        IsIEnumerable =
            PropertyInfo.PropertyType.IsGenericType &&
            PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
    }
}
