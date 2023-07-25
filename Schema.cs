// using System.Reflection;
// using ArgsParser.Attributes;
//
// namespace ArgsParser;
//
// internal class Schema
// {
//     public Type Type { get; }
//
//     private readonly ValueProp[] _valueProperties;
//     private readonly OptionProp[] _optionProperties;
//
//     private string? _valuesPropertyGroup;
//     private int? _maxValues;
//     private int? _minValues;
//
//     public Schema(Type type)
//     {
//         Type = type;
//         var propertyInfos = type.GetProperties();
//         _valueProperties = propertyInfos.Where(i => i.GetCustomAttributes<ValueAttribute>().Any()).ToArray();
//         foreach (var propertyInfo in propertyInfos)
//         {
//             var groupAttributes = propertyInfo.GetCustomAttributes<GroupAttribute>().ToArray();
//             var optionAttributes = propertyInfo.GetCustomAttributes<OptionAttribute>().ToArray();
//             if (optionAttributes.Any())
//             {
//                 _options.Add(groupAttributes.Any()
//                     ? new Option(propertyInfo, optionAttributes[0], groupAttributes[0])
//                     : new Option(propertyInfo, optionAttributes[0]));
//             }
//             else
//             {
//                 var valueAttributes = propertyInfo.GetCustomAttributes<ValueAttribute>().ToArray();
//                 if (valueAttributes.Any())
//                 {
//                     if (groupAttributes.Any())
//                         _valuesPropertyGroup = groupAttributes[0].GroupName;
//
//                     if (valueAttributes[0].Max != null)
//                         _maxValues = valueAttributes[0].Max;
//
//                     if (valueAttributes[0].Min != null)
//                         _minValues = valueAttributes[0].Min;
//                 }
//             }
//         }
//     }
//
//     public object BuildParsedOptions(string[] args)
//     {
//         var expectedArgumet = ExpectedArgumet.Option;
//         var groups = new List<string>();
//         var parsedOptions = new List<ParsedOption>();
//         ParsedOption currentOption = null!;
//         var parsedValues = new List<string>();
//
//         var minVals = 0;
//         var maxVals = 0;
//         foreach (var arg in args)
//         {
//             if (expectedArgumet == ExpectedArgumet.End)
//             {
//                 throw new Exception("Лишние аргументы"); //TODO
//             }
//
//             if (expectedArgumet == ExpectedArgumet.OptionValue)
//             {
//                 if (minVals > 0) minVals--;
//                 
//                 currentOption.Values.Add(arg);
//                 if (maxVals > 0 && --maxVals > 0)
//                 {
//                     if (minVals > 0)
//                         throw new Exception("Недостаточно значений для опции."); // TODO
//                     expectedArgumet = ExpectedArgumet.Option;
//                 }
//
//                 continue;
//             }
//
//             if (expectedArgumet == ExpectedArgumet.Value)
//             {
//                 parsedValues.Add(arg);
//                 continue;
//             }
//
//             if (arg.Length > 0)
//             {
//                 if (arg[0] == '-')
//                 {
//                     if (arg.Length > 1)
//                     {
//                         if (arg[1] == '-')
//                         {
//                             var optionName = arg[2..];
//                             var option =
//                                 _optionProperties.FirstOrDefault(o => o.OptionAttribute.Name == optionName);
//                             if (option is not null)
//                             {
//                                 if (option.Group != null)
//                                 {
//                                     if (groups.Contains(option.Group))
//                                         throw new Exception("Опция из одной группы выбрана дважды"); //TODO
//
//                                     groups.Add(option.Group);
//                                 }
//
//                                 if (minVals > 0)
//                                 {
//                                     throw new Exception(
//                                         $"Недостаточно значений для опции {currentOption.PropertyInfo.Name}");
//                                 }
//
//                                 if (option.OptionAttribute.Min is > 0)
//                                 {
//                                     minVals = option.OptionAttribute.Min.Value;
//                                     expectedArgumet = ExpectedArgumet.OptionValue;
//                                 }
//
//                                 if (option.OptionAttribute.Max is not null)
//                                     maxVals = option.OptionAttribute.Max.Value;
//
//                                 parsedOptions.Add(new ParsedOption(option.PropertyInfo));
//                             }
//                             else
//                             {
//                                 throw new Exception("Unknown option name"); //TODO
//                             }
//                         }
//                         else
//                         {
//                             for (var i = 1; i < arg.Length; i++)
//                             {
//                                 var option = _options.FirstOrDefault(o => o.Alias == arg[i]);
//                                 if (option != null)
//                                 {
//                                     if (option.Min is > 0)
//                                     {
//                                         minVals = option.Min.Value;
//                                         expectedArgumet = ExpectedArgumet.OptionValue;
//                                     }
//
//                                     if (option.Max is not null)
//                                         maxVals = option.Max.Value;
//                                 }
//                             }
//                         }
//                     }
//                 }
//                 else
//                 {
//                     if ()
//                     {
//                     }
//
//                     _values.Add(arg);
//                     if (_maxValues != null && --_maxValues <= 0)
//                     {
//                         expectedArgumet = ExpectedArgumet.End;
//                     }
//                     else
//                     {
//                         expectedArgumet = ExpectedArgumet.Value;
//                     }
//                 }
//             }
//             else
//             {
//                 throw new Exception("0 length argument"); //TODO
//             }
//         }
//
//         return BuildParsedOptions(parsedOptions, parsedValues);
//     }
//
//     private object BuildParsedOptions()
//     {
//         var parsedOptions = Activator.CreateInstance(Type)!;
//         foreach (var property in _valueProperties)
//         {
//         }
//     }
// }
