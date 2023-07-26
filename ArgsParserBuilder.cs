using System.Reflection;
using ArgsParser.Attributes;

namespace ArgsParser;

public class ArgsParserBuilder
{
    private readonly ArgsParserSettings _argsParserSettings;
    private readonly List<IVerbAction> _verbActions;
    private Action? _noArgsAction = null;
    private IVerbAction? _noVerbAction = null;

    public ArgsParserBuilder() : this(new ArgsParserSettings())
    {
    }

    public ArgsParserBuilder(ArgsParserSettings settings)
    {
        _argsParserSettings = settings;
        _verbActions = new List<IVerbAction>();
    }

    public ArgsParserBuilder AddNoArgsAction(Action action)
    {
        _noArgsAction = action;
        return this;
    }

    public ArgsParserBuilder AddNoVerbAction<T>(Action<T> action) where T : new()
    {
        _noVerbAction = new VerbAction<T>(action, true);
        return this;
    }

    public ArgsParserBuilder AddVerb<T>() where T : new()
    {
        if (typeof(T).GetCustomAttribute(typeof(VerbAttribute)) == null)
            throw new ArgumentException($"Type {typeof(T)} no have VerbAttribute");

        _verbActions.Add(new VerbAction<T>());
        return this;
    }

    public ArgsParserBuilder AddVerb<T>(Action<T> verbAction) where T : new()
    {
        if (typeof(T).GetCustomAttribute(typeof(VerbAttribute)) == null)
            throw new ArgumentException($"Type {typeof(T)} no have VerbAttribute");

        _verbActions.Add(new VerbAction<T>(verbAction));
        return this;
    }

    public ArgsParser Build()
    {
        if (_verbActions.Count == 0 && _noVerbAction == null)
            throw new Exception("Actions not added");

        return new ArgsParser(_argsParserSettings, _verbActions, _noArgsAction, _noVerbAction);
    }
}
