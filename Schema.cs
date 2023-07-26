using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser;

public class Schema
{
    public Type Type { get; }

    private readonly int _startIndex = 1;
    private readonly SortedDictionary<string, OptionPack> _optionPacksByName = new();
    private readonly SortedDictionary<char, OptionPack> _optionPacksByAlias = new();
    private readonly List<ValuePack> _valuePacks = new();
    private readonly SortedSet<string> _requiredGroups = new();

    public Schema(Type type)
    {
        Type = type;

        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            var groupAttribute = property.GetCustomAttribute<GroupAttribute>();
            var optionAttribute = property.GetCustomAttribute<OptionAttribute>();
            var valueAttribute = property.GetCustomAttribute<ValueAttribute>();

            if (groupAttribute is { Required: true })
                _requiredGroups.Add(groupAttribute.GroupName);

            if (optionAttribute != null)
            {
                var optionPack = new OptionPack(property, optionAttribute, groupAttribute);
                _optionPacksByName.Add(optionAttribute.Name, optionPack);
                if (optionAttribute.Alias != null)
                    _optionPacksByAlias.Add(optionAttribute.Alias.Value, optionPack);
                if (valueAttribute != null)
                    throw new Exception($"Не совместимые атрибуты на {property.Name} in {type.FullName}"); //TODO
            }
            else if (valueAttribute != null)
                _valuePacks.Add(new ValuePack(property, valueAttribute, groupAttribute));
        }

        _valuePacks.Sort((p1, p2) => p1.ValueAttribute.Index.CompareTo(p2.ValueAttribute.Index));
    }

    public Schema(Type type, bool noVerb) : this(type)
    {
        if (noVerb)
            _startIndex = 0;
    }

    //TODO rename
    private object CreatePobj(List<(List<string>, OptionPack)> parsedOptions, List<string> parsedValues)
    {
        var obj = Activator.CreateInstance(Type);
        if (obj == null)
        {
            throw new Exception("Не получилось создать экземпляр класса");
        }

        var parsedValuesPointer = 0;

        foreach (var parsedOption in parsedOptions)
        {
            var propType = parsedOption.Item2.PropertyInfo.PropertyType;

            if (propType.IsGenericType)
            {
                if (TryCreateInstanceOfList(propType, out var genericParameterType, out var listInstance))
                {
                    if (genericParameterType == typeof(string))
                    {
                        if (parsedOption.Item1.Count > 0)
                        {
                            foreach (var arg in parsedOption.Item1)
                            {
                                listInstance!.Add(arg);
                            }
                        }
                        else
                        {
                            while (parsedValues.Count > parsedValuesPointer)
                            {
                                var arg = parsedValues[parsedValuesPointer++];
                                listInstance!.Add(arg);
                            }
                        }
                    }
                    else
                    {
                        var parsMethod = propType.GenericTypeArguments[0]
                            .GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });

                        if (parsMethod == null)
                        {
                            //TODO
                            throw new Exception(
                                $"Тип {genericParameterType} in {propType} in {Type} не имеет метод Parse()");
                        }

                        if (parsedOption.Item1.Count > 0)
                        {
                            foreach (var arg in parsedOption.Item1)
                            {
                                listInstance!.Add(parsMethod.Invoke(null, new object[] { arg }));
                            }
                        }
                        else
                        {
                            while (parsedValues.Count > parsedValuesPointer)
                            {
                                var arg = parsedValues[parsedValuesPointer++];
                                listInstance!.Add(parsMethod.Invoke(null, new object[] { arg }));
                            }
                        }
                    }

                    parsedOption.Item2.PropertyInfo.SetValue(obj, listInstance);
                }
                else
                {
                    throw new Exception(
                        $"Не поддерживаемый тип в {Type} {propType}"); //TODO
                }
            }
            else
            {
                if (propType == typeof(bool))
                {
                    parsedOption.Item2.PropertyInfo.SetValue(obj, true);
                }
                else
                {
                    if (parsedOption.Item1.Count > 0)
                    {
                        AddArgToObj(propType, parsedOption.Item2.PropertyInfo, parsedOption.Item1[0]);
                    }
                    else
                    {
                        if (parsedValues.Count > parsedValuesPointer)
                        {
                            AddArgToObj(propType, parsedOption.Item2.PropertyInfo, parsedValues[parsedValuesPointer++]);
                        }
                        else
                        {
                            //TODO
                            throw new Exception(
                                "Тип определён отличным от bool или IEnumerable<> и значений не поступило");
                        }
                    }
                }
            }
        }

        foreach (var valuePack in _valuePacks)
        {
            if (parsedValues.Count > valuePack.ValueAttribute.Index + parsedValuesPointer)
            {
                var currentPointer = parsedValuesPointer;
                var propertyType = valuePack.PropertyInfo.PropertyType;
                if (TryCreateInstanceOfList(propertyType, out var genericParameterType, out var listInstance))
                {
                    if (valuePack.ValueAttribute.Min.HasValue)
                    {
                        if (parsedValues.Count >= currentPointer + valuePack.ValueAttribute.Min.Value)
                        {
                            if (valuePack.ValueAttribute.Max.HasValue)
                            {
                                var limit = valuePack.ValueAttribute.Max.Value + currentPointer;
                                if (parsedValues.Count > limit)
                                {
                                    for (; currentPointer < limit; currentPointer++)
                                    {
                                        ParsAndAddToList(parsedValues[currentPointer], listInstance,
                                            genericParameterType);
                                    }
                                }
                                else
                                {
                                    for (; currentPointer < parsedValues.Count; currentPointer++)
                                    {
                                        ParsAndAddToList(parsedValues[currentPointer], listInstance,
                                            genericParameterType);
                                    }
                                }
                            }
                            else
                            {
                                for (; currentPointer < parsedValues.Count; currentPointer++)
                                {
                                    ParsAndAddToList(parsedValues[currentPointer], listInstance, genericParameterType);
                                }
                            }
                        }
                        else
                        {
                            //TODO
                            throw new Exception($"{valuePack.PropertyInfo.Name} Не хватает параметров");
                        }
                    }
                    else
                    {
                        if (valuePack.ValueAttribute.Max.HasValue)
                        {
                            var limit = currentPointer + valuePack.ValueAttribute.Max.Value;
                            if (parsedValues.Count > limit)
                            {
                                for (;
                                     currentPointer < limit;
                                     currentPointer++)
                                {
                                    ParsAndAddToList(parsedValues[currentPointer], listInstance,
                                        genericParameterType);
                                }
                            }
                            else
                            {
                                for (; currentPointer < parsedValues.Count; currentPointer++)
                                {
                                    ParsAndAddToList(parsedValues[currentPointer], listInstance,
                                        genericParameterType);
                                }
                            }
                        }
                        else
                        {
                            for (; currentPointer < parsedValues.Count; currentPointer++)
                            {
                                ParsAndAddToList(parsedValues[currentPointer], listInstance,
                                    genericParameterType);
                            }
                        }
                    }

                    //TODO Если остались не использованные значения, то прокинуть исключение или добавить ошибку.
                    valuePack.PropertyInfo.SetValue(obj, listInstance);
                }
                else
                {
                    AddArgToObj(propertyType, valuePack.PropertyInfo, parsedValues[parsedValuesPointer++]);
                }
            }
            else
            {
                //TODO
                throw new Exception("Нехватает значений");
            }
        }

        return obj;

        void AddArgToObj(Type propType, PropertyInfo propertyInfo, string arg)
        {
            if (propType == typeof(string))
            {
                propertyInfo.SetValue(obj, arg);
            }
            else
            {
                var parsMethod = propertyInfo.PropertyType
                    .GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
                        new[] { typeof(string) });
                if (parsMethod == null)
                {
                    //TODO
                    throw new Exception(
                        $"Тип {propType.FullName} in {propertyInfo.PropertyType} in {Type.FullName} не имеет метод Parse()");
                }

                var parsedArg = parsMethod.Invoke(null, new object[] { arg });
                propertyInfo.SetValue(obj, parsedArg);
            }
        }
    }

    public object BuildParsedOptions(string[] args)
    {
        var parsedOptions = new List<(List<string>, OptionPack)>();
        var parsedValues = new List<string>();

        var requiredGroups = new SortedSet<string>(_requiredGroups);
        var groups = new SortedSet<string>();

        var expectedArgumet = ExpectedArgumet.Option;

        (List<string>, OptionPack) currentOption = default;
        int expectedValsMax = default;
        int expectedValsMin = default;

        for (var index = _startIndex; index < args.Length; index++)
        {
            var arg = args[index];
            if (expectedArgumet == ExpectedArgumet.Option)
            {
                if (arg.Length > 0)
                {
                    if (arg[0] == '-')
                    {
                        if (args.Length > 1)
                        {
                            if (arg[1] == '-')
                            {
                                var opt = arg[2..];
                                if (_optionPacksByName.TryGetValue(opt, out var optionPack))
                                {
                                    AddOption(optionPack);
                                }
                                else
                                {
                                    throw new Exception("Unknown option name"); //TODO
                                }
                            }
                            else
                            {
                                var opt = arg[1..];
                                foreach (var ch in opt)
                                {
                                    if (_optionPacksByAlias.TryGetValue(ch, out var optionPack))
                                    {
                                        AddOption(optionPack);
                                    }
                                    else
                                    {
                                        throw new Exception($"Unknown option alias: \"{ch}\""); //TODO
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Не указан алиас или опция.");
                        }
                    }
                    else
                    {
                        expectedArgumet = ExpectedArgumet.Option;
                        parsedValues.Add(arg);
                    }
                }
                else
                {
                    continue;
                }
            }
            else if (expectedArgumet == ExpectedArgumet.OptionValue)
            {
                currentOption.Item1!.Add(arg);
                if (expectedValsMin > 0 && --expectedValsMin == 0)
                    expectedArgumet = ExpectedArgumet.Option;
                if (expectedValsMax > 0 && --expectedValsMax == 0)
                    expectedArgumet = ExpectedArgumet.Option;
            }
            else if (expectedArgumet == ExpectedArgumet.Value)
            {
                parsedValues.Add(arg);
            }
            else if (expectedArgumet == ExpectedArgumet.End)
            {
                throw new Exception("Лишние аргументы"); //TODO
            }
        }

        if (requiredGroups.Count > 0)
        {
            foreach (var group in _requiredGroups)
                Console.WriteLine($"Нет группы: {group}"); //TODO

            throw new Exception("Указаны не все обяательные группы.");
        }

        if (expectedValsMin > 0)
        {
            throw new Exception("Недостаточно аргументов"); //TODO
        }

        return CreatePobj(parsedOptions, parsedValues);

        void AddOption(OptionPack optionPack)
        {
            var opt = new ValueTuple<List<string>, OptionPack>(new List<string>(),
                optionPack);
            parsedOptions.Add(opt);

            if (optionPack.GroupAttribute != null)
            {
                if (optionPack.GroupAttribute.Required)
                    requiredGroups.Remove(optionPack.GroupAttribute.GroupName);

                if (!groups.Add(optionPack.GroupAttribute.GroupName))
                {
                    throw new Exception("Опция из одной группы выбрана дважды"); //TODO
                }
            }

            if (optionPack.OptionAttribute.Min != null)
            {
                expectedValsMin = optionPack.OptionAttribute.Min.Value;
                expectedArgumet = ExpectedArgumet.OptionValue;
                currentOption = opt;
            }

            if (optionPack.OptionAttribute.Max != null)
                expectedValsMax = optionPack.OptionAttribute.Max.Value;
        }
    }

    private bool TryCreateInstanceOfList(Type type, out Type? genericParameterType, out dynamic? listInstance)
    {
        if (!type.IsGenericType)
        {
            listInstance = null;
            genericParameterType = null;
            return false;
        }

        if (type.IsGenericType &&
            (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
             type.GetInterfaces().Any(t =>
                 t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))) &&
            type.GenericTypeArguments.Length == 1)
        {
            genericParameterType = type.GenericTypeArguments[0];
            var listPropT = typeof(List<>).MakeGenericType(genericParameterType);
            //Ошибка? Ну я хз ¯\_(ツ)_/¯ это dynamic type
            listInstance = Activator.CreateInstance(listPropT) ?? //TODO
                           throw new InvalidOperationException(
                               $"Не удалось создать экземпляр типа {listPropT} in {Type}");
            return true;
        }

        listInstance = null;
        genericParameterType = null;
        return false;
    }

    private void ParsAndAddToList(string arg, dynamic list, Type genericType)
    {
        if (genericType == typeof(string))
        {
            list.Add(arg);
        }
        else
        {
            var parsMethod = genericType
                .GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
                    new[] { typeof(string) });

            if (parsMethod == null)
            {
                //TODO
                throw new Exception(
                    $"Тип {genericType} in {Type} не имеет метод Parse()");
            }

            list.Add(parsMethod.Invoke(null, new object[] { arg }));
        }
    }
}
