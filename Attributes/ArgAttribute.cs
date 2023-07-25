namespace ArgsParser.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public abstract class ArgAttribute : Attribute
{
}
