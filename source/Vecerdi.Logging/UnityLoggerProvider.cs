using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Vecerdi.Logging.Unity {
    /// <summary>
    /// Logger provider that routes Microsoft.Extensions.Logging calls to Unity's Debug.Log* methods.
    /// This class is designed to be used in environments where both Microsoft.Extensions.Logging 
    /// and Unity are available.
    /// </summary>
    public class UnityLoggerProvider : ILoggerProvider {
        private readonly ConcurrentDictionary<string, UnityMsLogger> _loggers = new();
        private bool _disposed;

        /// <summary>
        /// Singleton instance of the Unity logger provider.
        /// </summary>
        public static UnityLoggerProvider Instance { get; } = new();

        /// <summary>
        /// Creates a logger with the specified category name.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by this logger.</param>
        /// <returns>An <see cref="ILogger"/> instance.</returns>
        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) {
            if (_disposed) {
                throw new ObjectDisposedException(nameof(UnityLoggerProvider));
            }

            return _loggers.GetOrAdd(categoryName, name => new UnityMsLogger(name));
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
    /// Microsoft.Extensions.Logging.ILogger implementation that routes logs to Unity's Debug.Log* methods.
    /// This implementation assumes Unity is available and directly calls Unity methods.
    /// </summary>
    internal class UnityMsLogger : Microsoft.Extensions.Logging.ILogger {
        private readonly string _categoryName;

        public UnityMsLogger(string categoryName) {
            _categoryName = categoryName ?? throw new ArgumentNullException(nameof(categoryName));
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull {
            // Unity doesn't support scopes, so we return a no-op disposable
            return NoOpDisposable.Instance;
        }

        /// <summary>
        /// Checks if the given log level is enabled.
        /// </summary>
        /// <param name="logLevel">Level to be checked.</param>
        /// <returns>true if enabled; false otherwise.</returns>
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) {
            return logLevel != Microsoft.Extensions.Logging.LogLevel.None;
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

            // Format the message with category
            var logMessage = $"[{logLevel}, {_categoryName}] {message}";

            // Route to Unity's Debug.Log methods based on log level
            switch (logLevel) {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    if (exception != null) {
                        // Use reflection to call UnityEngine.Debug.LogException
                        CallUnityDebugMethod("LogException", exception);
                    } else {
                        // Use reflection to call UnityEngine.Debug.Log
                        CallUnityDebugMethod("Log", logMessage);
                    }
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    if (exception != null) {
                        CallUnityDebugMethod("LogWarning", logMessage + Environment.NewLine + exception);
                    } else {
                        CallUnityDebugMethod("LogWarning", logMessage);
                    }
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    if (exception != null) {
                        CallUnityDebugMethod("LogException", exception);
                    } else {
                        CallUnityDebugMethod("LogError", logMessage);
                    }
                    break;
            }
        }

        private static void CallUnityDebugMethod(string methodName, object message) {
            try {
                // Use reflection to find and call Unity's Debug methods
                // This allows the code to work even when Unity assemblies aren't directly referenced
                var unityDebugType = Type.GetType("UnityEngine.Debug, UnityEngine.CoreModule");
                if (unityDebugType != null) {
                    var method = unityDebugType.GetMethod(methodName, new[] { message.GetType() });
                    method?.Invoke(null, new[] { message });
                }
            }
            catch {
                // If Unity isn't available, fall back to console
                Console.WriteLine($"[Unity] {message}");
            }
        }

        private class NoOpDisposable : IDisposable {
            public static readonly NoOpDisposable Instance = new();
            public void Dispose() { }
        }
    }
}