namespace Vecerdi.Logging;

internal class ConsoleLogger : ILogger {
    /// <summary>
    /// The minimum log level to log.
    /// </summary>
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Logs a message with the specified category, context and log level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    public void Message(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Information) {
        string m = context is not null
            ? StringUtilities.Format("[{}/{}] {}".AsSpan(), category, context.ToString().AsSpan(), message)
            : StringUtilities.Format("[{}] {}".AsSpan(), category, message);
        Console.WriteLine(m);
    }

    /// <summary>
    /// Logs an exception with the specified category, context and log level.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="category">The category of the exception.</param>
    /// <param name="context">The context of the exception.</param>
    /// <param name="logLevel">The log level of the exception message.</param>
    public void Exception(Exception? exception, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Error) {
        string m = context is not null
            ? StringUtilities.Format("[{}/{}] {}".AsSpan(), category, context.ToString().AsSpan(), (exception?.ToString() ?? "null").AsSpan())
            : StringUtilities.Format("[{}] {}".AsSpan(), category, (exception?.ToString() ?? "null").AsSpan());

        Console.WriteLine(m);
    }
}