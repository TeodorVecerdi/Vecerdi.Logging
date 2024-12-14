namespace Vecerdi.Logging;

public interface ILogger {
    /// <summary>
    /// The minimum log level to log.
    /// </summary>
    LogLevel MinimumLogLevel { get; set; }

    /// <summary>
    /// Logs a message with the specified category, context and log level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    void Message(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Information);

    /// <summary>
    /// Logs an exception with the specified category, context and log level.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="category">The category of the exception.</param>
    /// <param name="context">The context of the exception.</param>
    /// <param name="logLevel">The log level of the exception message.</param>
    void Exception(Exception? exception, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Error);
}