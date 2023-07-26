using ArgsParser.Attributes;

namespace ArgsParser;

internal interface IVerbAction
{
    public VerbAttribute Verb { get; }
    public Schema Schema { get; set; }
    public void Invoke(string[] args);
}
