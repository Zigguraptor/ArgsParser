namespace ArgsParser;

public class ArgsParser
{
    private readonly ArgsParserSettings _argsParserSettings;
    private readonly Dictionary<string, IVerbAction> _verbActionsByName;
    private readonly IVerbAction? _noVerbAction;
    private readonly Action? _noArgsAction;
    private readonly Action<string>? _error;

    internal ArgsParser(
        ArgsParserSettings argsParserSettings,
        List<IVerbAction> verbActions,
        Action? noArgsAction,
        IVerbAction? noVerbAction,
        Action<string>? error = null)
    {
        _argsParserSettings = argsParserSettings;
        _noArgsAction = noArgsAction;
        _noVerbAction = noVerbAction;
        _error = error;
        _verbActionsByName = new Dictionary<string, IVerbAction>();
        foreach (var verbAction in verbActions)
        {
            var verb = verbAction.Verb;
            _verbActionsByName.Add(verb.Name, verbAction);
        }
    }

    public void ParseArgs(string[] args)
    {
        if (_noArgsAction == null && args.Length == 0)
        {
            ThrowException("No Args and \"NoArgsAction\" not defined.");
            return;
        }

        if (_verbActionsByName.TryGetValue(args[0], out var verbAction))
        {
            verbAction.Invoke(args);
        }
        else
        {
            if (_noVerbAction != null)
            {
                _noVerbAction.Invoke(args);
            }
            else
            {
                ThrowException($"Unknown verb: \"{args[0]}\"");
            }
        }
    }

    private void ThrowException(string message)
    {
        if (_argsParserSettings.ThrowExceptions)
        {
            throw new Exception(message);
        }
        else
        {
            _error?.Invoke(message);
        }
    }
}
