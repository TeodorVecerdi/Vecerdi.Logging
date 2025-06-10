using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Vecerdi.Logging.Extensions {
    /// <summary>
    /// Logger provider that routes Microsoft.Extensions.Logging logs to the Vecerdi.Logging system.
    /// </summary>
    public class VecerdiLoggerProvider : ILoggerProvider {
        private readonly ConcurrentDictionary<string, VecerdiLogger> _loggers = new();
        private bool _disposed;

        /// <summary>
        /// Creates a logger with the specified category name.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by this logger.</param>
        /// <returns>An <see cref="ILogger"/> instance.</returns>
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(VecerdiLoggerProvider));
            }

            return _loggers.GetOrAdd(categoryName, name => new VecerdiLogger(name));
        }

        /// <summary>
        /// Disposes the logger provider.
        /// </summary>
        public void Dispose() {
            if (_disposed) {
                return;
            }

            _disposed = true;
            _loggers.Clear();
        }
    }

    /// <summary>
    /// Microsoft.Extensions.Logging.ILogger implementation that routes logs to Vecerdi.Logging.Log.
    /// </summary>
    internal class VecerdiLogger : Microsoft.Extensions.Logging.ILogger {
        private readonly string _categoryName;

        public VecerdiLogger(string categoryName) {
            _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
            // Vecerdi.Logging doesn't support scopes, so we return a no-op disposable
            return NoOpDisposable.Instance;
        }

        /// <summary>
        /// Checks if the given log level is enabled.
        /// </summary>
        /// <param name="logLevel">Level to be checked.</param>
        /// <returns>true if enabled; false otherwise.</returns>
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) {
            if (logLevel == Microsoft.Extensions.Logging.LogLevel.None) {
                return false;
            }

            var vecerdiLogLevel = ConvertToVecerdiLogLevel(logLevel);
            return vecerdiLogLevel >= Vecerdi.Logging.Log.DefaultLogger.MinimumLogLevel;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a string message of the state and exception.</param>
        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, 
                               Exception? exception, Func<TState, Exception?, string> formatter) {
            if (!IsEnabled(logLevel)) {
                return;
            }

            if (formatter == null) {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception == null) {
                return;
            }

            var vecerdiLogLevel = ConvertToVecerdiLogLevel(logLevel);
            
            // Log the message
            if (!string.IsNullOrEmpty(message)) {
                Vecerdi.Logging.Log.Message(message.AsSpan(), _categoryName.AsSpan(), null, vecerdiLogLevel);
            }

            // Log the exception if present
            if (exception != null) {
                Vecerdi.Logging.Log.Exception(exception, _categoryName.AsSpan(), null, vecerdiLogLevel);
            }
        }

        private static LogLevel ConvertToVecerdiLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel) {
            return logLevel switch {
                Microsoft.Extensions.Logging.LogLevel.Trace => LogLevel.Trace,
                Microsoft.Extensions.Logging.LogLevel.Debug => LogLevel.Debug,
                Microsoft.Extensions.Logging.LogLevel.Information => LogLevel.Information,
                Microsoft.Extensions.Logging.LogLevel.Warning => LogLevel.Warning,
                Microsoft.Extensions.Logging.LogLevel.Error => LogLevel.Error,
                Microsoft.Extensions.Logging.LogLevel.Critical => LogLevel.Critical,
                Microsoft.Extensions.Logging.LogLevel.None => LogLevel.None,
                _ => LogLevel.Information
            };
        }

        private class NoOpDisposable : IDisposable {
            public static readonly NoOpDisposable Instance = new();
            public void Dispose() { }
        }
    }
}