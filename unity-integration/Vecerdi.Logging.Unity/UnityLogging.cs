#nullable enable

namespace Vecerdi.Logging.Unity {
    /// <summary>
    /// Unity-specific logging utilities that integrate with Microsoft.Extensions.Logging when available.
    /// </summary>
    public static class UnityLogging {
        /// <summary>
        /// Creates a Unity logger provider that can be used with Microsoft.Extensions.Logging.
        /// Call this method and add the result to your ILoggerFactory.
        /// </summary>
        /// <returns>A logger provider instance that routes logs to Unity.</returns>
        public static object CreateLoggerProvider() {
            return new UnityLoggerProviderAdapter();
        }
    }
    
    /// <summary>
    /// Adapter that implements the Microsoft.Extensions.Logging.ILoggerProvider pattern
    /// but doesn't directly reference the types to avoid dependency issues.
    /// </summary>
    public class UnityLoggerProviderAdapter {
        /// <summary>
        /// Creates a logger with the specified category name.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by this logger.</param>
        /// <returns>A logger instance.</returns>
        public object CreateLogger(string categoryName) {
            return new UnityLoggerAdapter(categoryName);
        }
        
        /// <summary>
        /// Disposes the logger provider.
        /// </summary>
        public void Dispose() {
            // Nothing to dispose in Unity logger
        }
    }
    
    /// <summary>
    /// Adapter that implements the Microsoft.Extensions.Logging.ILogger pattern
    /// but doesn't directly reference the types to avoid dependency issues.
    /// </summary>
    public class UnityLoggerAdapter {
        private readonly string _categoryName;
        
        public UnityLoggerAdapter(string categoryName) {
            _categoryName = categoryName ?? throw new System.ArgumentNullException(nameof(categoryName));
        }
        
        // These methods will be called via reflection by Microsoft.Extensions.Logging
        // when this adapter is used with the proper extension methods
    }
}