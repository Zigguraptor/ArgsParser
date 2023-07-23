using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser.PropertyGroups;

internal class PropertyGroup
{
    public string? Name;
    public bool Require;
    public readonly List<PropertyInfo> PropertyInfos;

    private PropertyGroup(string? name, bool require, List<PropertyInfo> propertyInfos)
    {
        Name = name;
        Require = require;
        PropertyInfos = propertyInfos;
    }

    public static List<PropertyGroup> CreateGroups<T>()
    {
        var result = new List<PropertyGroup> { new(null, false, new List<PropertyInfo>()) };

        var propertyInfos = typeof(T).GetProperties()
            .Where(p => p.GetCustomAttributes<ArgAttribute>(true).Any());

        foreach (var propertyInfo in propertyInfos)
        {
            var customAttributes = propertyInfo.GetCustomAttributes<OptionGroupAttribute>(true).ToArray();
            if (!customAttributes.Any())
            {
                result[0].PropertyInfos.Add(propertyInfo);
            }
            else
            {
                var attribute = customAttributes[0];
                var propertyGroup = result.Find(g => g.Name == attribute.GroupName);
                if (propertyGroup is null)
                {
                    result.Add(new PropertyGroup(attribute.GroupName, attribute.Required,
                        new List<PropertyInfo> { propertyInfo }));
                }
                else
                {
                    propertyGroup.PropertyInfos.Add(propertyInfo);
                }
            }
        }

        return result;
    }
}
