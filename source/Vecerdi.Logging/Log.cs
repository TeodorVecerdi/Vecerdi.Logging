using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Vecerdi.Logging;

[PublicAPI]
public static class Log {
    private static readonly HashSet<ILogger> s_Loggers = new();

    public static ILogger DefaultLogger { get; } = new ConsoleLogger();

    /// <summary>
    /// Registers a logger to log messages.
    /// </summary>
    /// <param name="logger">The logger to register.</param>
    /// <exception cref="ArgumentNullException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is null.</exception>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void RegisterLogger(ILogger logger) {
        if (logger is null) throw new ArgumentNullException(nameof(logger));

        s_Loggers.Add(logger);
    }
    /// <summary>
    /// Tries to register a logger to log messages.
    /// </summary>
    /// <param name="logger">The logger to register.</param>
    /// <returns><see langword="true"/> if the logger was registered, <see langword="false"/> if it was already registered.</returns>
    /// <exception cref="ArgumentNullException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is null.</exception>
    public static bool TryRegisterLogger(ILogger logger) {
        if (logger is null) throw new ArgumentNullException(nameof(logger));

        return s_Loggers.Add(logger);
    }

    /// <summary>
    /// Unregisters a logger from logging messages.
    /// </summary>
    /// <param name="logger">The logger to unregister.</param>
    /// <exception cref="ArgumentNullException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is null.</exception>
    /// <exception cref="ArgumentException">Given <see cref="T:Vecerdi.Logging.ILogger"/> is not registered.</exception>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void UnregisterLogger(ILogger logger) {
        if (logger is null) throw new ArgumentNullException(nameof(logger));

        if (!s_Loggers.Remove(logger)) {
            throw new ArgumentException("Logger is not registered.", nameof(logger));
        }
    }

    /// <summary>
    /// Logs a message with the specified category, context and log level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Message(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Information) {
        if (s_Loggers.Count == 0) {
            if (logLevel >= DefaultLogger.MinimumLogLevel) {
                DefaultLogger.Message(message, category, context, logLevel);
            }
        } else {
            foreach (ILogger logger in s_Loggers) {
                if (logLevel >= logger.MinimumLogLevel) {
                    logger.Message(message, category, context, logLevel);
                }
            }
        }
    }

    /// <summary>
    /// Logs a formatted message with the specified category, context and log level.
    /// </summary>
    /// <param name="format">The format of the message.</param>
    /// <param name="arg0">The first argument to format.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    [StringFormatMethod(nameof(format))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Information) {
        string message = StringUtilities.Format(format, arg0);
        Message(message, category, context, logLevel);
    }

    /// <summary>
    /// Logs a formatted message with the specified category, context and log level.
    /// </summary>
    /// <param name="format">The format of the message.</param>
    /// <param name="arg0">The first argument to format.</param>
    /// <param name="arg1">The second argument to format.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    [StringFormatMethod(nameof(format))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Information) {
        string message = StringUtilities.Format(format, arg0, arg1);
        Message(message, category, context, logLevel);
    }

    /// <summary>
    /// Logs a formatted message with the specified category, context and log level.
    /// </summary>
    /// <param name="format">The format of the message.</param>
    /// <param name="arg0">The first argument to format.</param>
    /// <param name="arg1">The second argument to format.</param>
    /// <param name="arg2">The third argument to format.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    [StringFormatMethod(nameof(format))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Information) {
        string message = StringUtilities.Format(format, arg0, arg1, arg2);
        Message(message, category, context, logLevel);
    }

    /// <summary>
    /// Logs a formatted message with the specified category, context and log level.
    /// </summary>
    /// <param name="format">The format of the message.</param>
    /// <param name="arg0">The first argument to format.</param>
    /// <param name="arg1">The second argument to format.</param>
    /// <param name="arg2">The third argument to format.</param>
    /// <param name="arg3">The fourth argument to format.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    [StringFormatMethod(nameof(format))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> arg0, ReadOnlySpan<char> arg1, ReadOnlySpan<char> arg2, ReadOnlySpan<char> arg3, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Information) {
        string message = StringUtilities.Format(format, arg0, arg1, arg2, arg3);
        Message(message, category, context, logLevel);
    }

    /// <summary>
    /// Logs a formatted message with the specified category, context and log level.
    /// </summary>
    /// <param name="format">The format of the message.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
    /// <param name="logLevel">The log level of the message.</param>
    /// <param name="args">The arguments to format.</param>
    [StringFormatMethod(nameof(format))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Format(ReadOnlySpan<char> format, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Information, params string?[] args) {
        string message = StringUtilities.Format(format, args);
        Message(message, category, context, logLevel);
    }

    /// <summary>
    /// Logs an exception with the specified category, context and log level.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="category">The category of the exception.</param>
    /// <param name="context">The context of the exception.</param>
    /// <param name="logLevel">The log level of the exception message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Exception(Exception? exception, ReadOnlySpan<char> category = default, IContext? context = null, LogLevel logLevel = LogLevel.Error) {
        if (s_Loggers.Count == 0) {
            if (logLevel >= DefaultLogger.MinimumLogLevel) {
                DefaultLogger.Exception(exception, category, context, logLevel);
            }
        } else {
            foreach (ILogger logger in s_Loggers) {
                if (logLevel >= logger.MinimumLogLevel) {
                    logger.Exception(exception, category, context, logLevel);
                }
            }
        }
    }

    /// <summary>
    /// Logs a Trace message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Trace(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null) => Message(message, category, context, LogLevel.Trace);

    /// <summary>
    /// Logs a Debug message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Debug(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null) => Message(message, category, context, LogLevel.Debug);

    /// <summary>
    /// Logs an Information message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Information(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null) => Message(message, category, context, LogLevel.Information);

    /// <summary>
    /// Logs a Warning message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Warning(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null) => Message(message, category, context, LogLevel.Warning);

    /// <summary>
    /// Logs an Error message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Error(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null) => Message(message, category, context, LogLevel.Error);

    /// <summary>
    /// Logs a Critical message with the specified category and context.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="category">The category of the message.</param>
    /// <param name="context">The context of the message.</param>
#if NO_LOGGING
    [System.Diagnostics.Conditional("")]
#endif
    public static void Critical(ReadOnlySpan<char> message, ReadOnlySpan<char> category = default, IContext? context = null) => Message(message, category, context, LogLevel.Critical);
}