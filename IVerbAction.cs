using ArgsParser.Attributes;
using ArgsParser.newS;

namespace ArgsParser;

internal interface IVerbAction
{
    public VerbAttribute Verb { get; }
    public NewSchema Schema { get; set; }
    public void Invoke(object options);
}
