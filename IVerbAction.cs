using ArgsParser.Attributes;

namespace ArgsParser;

internal interface IVerbAction
{
    public VerbAttribute Verb { get; }
    public void Invoke(string[] args);
}
