using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Vecerdi.Logging;

/// <summary>
/// Static logging class that provides convenient logging methods using Microsoft.Extensions.Logging as the underlying implementation.
/// </summary>
public static class Log {
    private static ILoggerFactory? s_LoggerFactory;
    private static Microsoft.Extensions.Logging.ILogger? s_Logger;
    
    /// <summary>
    /// Gets or sets the logger factory used for creating loggers.
    /// When set, a new logger is created for the "Vecerdi.Logging.Log" category.
    /// </summary>
    public static ILoggerFactory? LoggerFactory {
        get => s_LoggerFactory;
        set {
            s_LoggerFactory = value;
            s_Logger = value?.CreateLogger("Vecerdi.Logging.Log");
        }
    }

    /// <summary>
    /// Gets the current logger. If no logger factory is set, returns null.
    /// </summary>
    public static Microsoft.Extensions.Logging.ILogger? Logger => s_Logger;

    // Legacy support for backwards compatibility - marked as obsolete
#pragma warning disable CS0618 // Type or member is obsolete
    internal static readonly HashSet<ILogger> s_LegacyLoggers = new();

#pragma warning disable CS0618 // Type or member is obsolete
    [System.Obsolete("Use LoggerFactory property instead. This property will be removed in a future version.", false)]
    public static ILogger DefaultLogger { get; } = new ConsoleLogger();
#pragma warning restore CS0618 // Type or member is obsolete

    /// <summary>
    /// Registers a legacy logger to log messages.
    /// </summary>
    /// <param name="logger">The logger to register.</param>
    /// <exception cref="ArgumentNullException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is null.</exception>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    [System.Obsolete("Use Microsoft.Extensions.Logging LoggerFactory instead. This method will be removed in a future version.", false)]
    public static void RegisterLogger(ILogger logger) {
        if (logger is null) throw new ArgumentNullException(nameof(logger));
        s_LegacyLoggers.Add(logger);
    }

    /// <summary>
    /// Tries to register a legacy logger to log messages.
    /// </summary>
    /// <param name="logger">The logger to register.</param>
    /// <returns><see langword="true"/> if the logger was registered, <see langword="false"/> if it was already registered.</returns>
    /// <exception cref="ArgumentNullException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is null.</exception>
    [System.Obsolete("Use Microsoft.Extensions.Logging LoggerFactory instead. This method will be removed in a future version.", false)]
    public static bool TryRegisterLogger(ILogger logger) {
        if (logger is null) throw new ArgumentNullException(nameof(logger));
        return s_LegacyLoggers.Add(logger);
    }

    /// <summary>
    /// Unregisters a legacy logger from logging messages.
    /// </summary>
    /// <param name="logger">The logger to unregister.</param>
    /// <exception cref="ArgumentNullException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is null.</exception>
    /// <exception cref="ArgumentException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is not registered.</exception>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    [System.Obsolete("Use Microsoft.Extensions.Logging LoggerFactory instead. This method will be removed in a future version.", false)]
    public static void UnregisterLogger(ILogger logger) {
        if (logger is null) throw new ArgumentNullException(nameof(logger));
        if (!s_LegacyLoggers.Remove(logger)) {
            throw new ArgumentException("Logger is not registered.", nameof(logger));
        }
    }

    /// <summary>
    /// Logs a message with the specified category, context, and log level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Message(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Information) {
        // Route to Microsoft.Extensions.Logging if available
        if (s_Logger != null) {
            var msLogLevel = ConvertLogLevel(logLevel);
            var categoryStr = category.IsEmpty ? null : category.ToString();
            var messageStr = message.ToString();
            
            if (categoryStr != null) {
                messageStr = $"[{categoryStr}] {messageStr}";
            }
            
            s_Logger.Log(msLogLevel, 0, messageStr, null, (msg, ex) => msg);
        }
        
        // Also route to legacy loggers for backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
        if (s_LegacyLoggers.Count == 0) {
            if (logLevel >= DefaultLogger.MinimumLogLevel) {
                DefaultLogger.Message(message, category, context, logLevel);
            }
        } else {
            foreach (ILogger logger in s_LegacyLoggers) {
                if (logLevel >= logger.MinimumLogLevel) {
                    logger.Message(message, category, context, logLevel);
                }
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }

    /// <summary>
    /// Logs an exception with the specified category, context, and log level.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Exception(Exception? exception, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Error) {
        // Route to Microsoft.Extensions.Logging if available
        if (s_Logger != null && exception != null) {
            var msLogLevel = ConvertLogLevel(logLevel);
            var categoryStr = category.IsEmpty ? null : category.ToString();
            var message = categoryStr != null ? $"[{categoryStr}] Exception occurred" : "Exception occurred";
            
            s_Logger.Log(msLogLevel, 0, message, exception, (msg, ex) => msg);
        }
        
        // Also route to legacy loggers for backwards compatibility
#pragma warning disable CS0618 // Type or member is obsolete
        if (s_LegacyLoggers.Count == 0) {
            if (logLevel >= DefaultLogger.MinimumLogLevel) {
                DefaultLogger.Exception(exception, category, context, logLevel);
            }
        } else {
            foreach (ILogger logger in s_LegacyLoggers) {
                if (logLevel >= logger.MinimumLogLevel) {
                    logger.Exception(exception, category, context, logLevel);
                }
            }
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }

    /// <summary>
    /// Converts Vecerdi.Logging.LogLevel to Microsoft.Extensions.Logging.LogLevel.
    /// </summary>
    /// <param name="logLevel">The Vecerdi.Logging.LogLevel to convert.</param>
    /// <returns>The equivalent Microsoft.Extensions.Logging.LogLevel.</returns>
    private static Microsoft.Extensions.Logging.LogLevel ConvertLogLevel(LogLevel logLevel) {
        return logLevel switch {
            LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
            LogLevel.Critical => Microsoft.Extensions.Logging.LogLevel.Critical,
            LogLevel.None => Microsoft.Extensions.Logging.LogLevel.None,
            _ => Microsoft.Extensions.Logging.LogLevel.Information
        };
    }

    /// <summary>
    /// Logs a formatted message with the specified category, context, and log level.
    /// </summary>
    /// <param name="format">The format of the message.</param>
    /// <param name="arg0">The first argument to format.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format, arg0).AsSpan();
        Message(message, category, context, logLevel);
    }

    /// <summary>
    /// Logs a formatted message with the specified category, context, and log level.
    /// </summary>
    /// <param name="format">The format of the message.</param>
    /// <param name="arg0">The first argument to format.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(string format, string arg0, string category = "", object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format.AsSpan(), arg0.AsSpan());
        Message(message.AsSpan(), category.AsSpan(), context, logLevel);
    }

    // Continue with all the other Format method overloads...
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format, arg0, arg1).AsSpan();
        Message(message, category, context, logLevel);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(string format, string arg0, string arg1, string category = "", object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format.AsSpan(), arg0.AsSpan(), arg1.AsSpan());
        Message(message.AsSpan(), category.AsSpan(), context, logLevel);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format, arg0, arg1, arg2).AsSpan();
        Message(message, category, context, logLevel);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(string format, string arg0, string arg1, string arg2, string category = "", object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format.AsSpan(), arg0.AsSpan(), arg1.AsSpan(), arg2.AsSpan());
        Message(message.AsSpan(), category.AsSpan(), context, logLevel);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> category = default, object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format, arg0, arg1, arg2, arg3).AsSpan();
        Message(message, category, context, logLevel);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(string format, string arg0, string arg1, string arg2, string arg3, string category = "", object? context = null, LogLevel logLevel = LogLevel.Information) {
        var message = StringUtilities.Format(format.AsSpan(), arg0.AsSpan(), arg1.AsSpan(), arg2.AsSpan(), arg3.AsSpan());
        Message(message.AsSpan(), category.AsSpan(), context, logLevel);
    }

    // Convenience methods for different log levels
    /// <summary>
    /// Logs a Trace message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Trace(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null) => Message(message, category, context, LogLevel.Trace);

    /// <summary>
    /// Logs a Trace message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Trace(string message, string category = "", object? context = null) => Message(message.AsSpan(), category.AsSpan(), context, LogLevel.Trace);

    /// <summary>
    /// Logs a Debug message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Debug(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null) => Message(message, category, context, LogLevel.Debug);

    /// <summary>
    /// Logs a Debug message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Debug(string message, string category = "", object? context = null) => Message(message.AsSpan(), category.AsSpan(), context, LogLevel.Debug);

    /// <summary>
    /// Logs an Information message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Information(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null) => Message(message, category, context, LogLevel.Information);

    /// <summary>
    /// Logs an Information message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Information(string message, string category = "", object? context = null) => Message(message.AsSpan(), category.AsSpan(), context, LogLevel.Information);

    /// <summary>
    /// Logs a Warning message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Warning(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null) => Message(message, category, context, LogLevel.Warning);

    /// <summary>
    /// Logs a Warning message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Warning(string message, string category = "", object? context = null) => Message(message.AsSpan(), category.AsSpan(), context, LogLevel.Warning);

    /// <summary>
    /// Logs an Error message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Error(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null) => Message(message, category, context, LogLevel.Error);

    /// <summary>
    /// Logs an Error message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Error(string message, string category = "", object? context = null) => Message(message.AsSpan(), category.AsSpan(), context, LogLevel.Error);

    /// <summary>
    /// Logs a Critical message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Critical(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, object? context = null) => Message(message, category, context, LogLevel.Critical);

    /// <summary>
    /// Logs a Critical message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Critical(string message, string category = "", object? context = null) => Message(message.AsSpan(), category.AsSpan(), context, LogLevel.Critical);
}