using ArgsParser.Attributes;

namespace ArgsParser;

internal interface IVerbAction
{
    public VerbAttribute Verb { get; }
    public bool TryInvoke(string[] args, out object parsedOptions);
}
