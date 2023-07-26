namespace ArgsParser;

public struct ArgsParserSettings
{
    public ArgsParserSettings()
    {
    }

    public ArgsParserSettings(int maximumDisplayWidth, bool throwExceptions)
    {
        MaximumDisplayWidth = maximumDisplayWidth;
        ThrowExceptions = throwExceptions;
    }

    public int MaximumDisplayWidth = 80;
    public bool ThrowExceptions = true;
}
