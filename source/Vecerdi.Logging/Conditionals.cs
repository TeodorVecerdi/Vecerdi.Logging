namespace Vecerdi.Logging;

internal static class Conditionals {
#if NO_LOGGING
    public const string ENABLE_LOGGING = "NO_LOGGING_8BB6260701404046B1BF269D15ED609A__7A1B24E50B684F1DBCCE57B02B82F233";
#else
    public const string ENABLE_LOGGING = "NETSTANDARD";
#endif
}