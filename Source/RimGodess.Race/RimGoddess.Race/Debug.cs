using System.Diagnostics;

namespace RimGoddess.Race;

internal class Debug
{
    [Conditional("DEBUG")]
    internal static void Log(string s)
    {
        Verse.Log.Message(s);
    }

    [Conditional("DEBUG")]
    internal static void Warning(string s)
    {
        Verse.Log.Warning(s);
    }

    [Conditional("DEBUG")]
    internal static void Error(string s)
    {
        Verse.Log.Error(s);
    }
}