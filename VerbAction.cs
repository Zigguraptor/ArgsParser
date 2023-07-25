using ArgsParser.Attributes;

namespace ArgsParser;

internal class VerbAction<T> : IVerbAction where T : new()
{
    private readonly Action<T> _verbAction;
    public Schema Schema { get; set; }

    public VerbAction(Action<T> verbAction)
    {
        _verbAction = verbAction;
        Schema = new Schema(typeof(T));
    }

    public VerbAttribute Verb
    {
        get
        {
            var customAttribute = typeof(T).GetCustomAttributes(typeof(VerbAttribute), true)[0];
            if (customAttribute is VerbAttribute verbAttribute)
                return verbAttribute;

            throw new Exception(); //TODO
        }
    }

    public void Invoke(object options)
    {
        _verbAction.Invoke((T)options); //TODO handle cast ex
    }
}
