#nullable enable

namespace Vecerdi.Logging.Unity {
    /// <summary>
    /// Unity-specific logging utilities that integrate with Microsoft.Extensions.Logging when available.
    /// </summary>
    public static class UnityLogging {
        /// <summary>
        /// Gets the global Unity logger instance for backwards compatibility.
        /// </summary>
        public static UnityLogger Logger => UnityLogger.Instance;
        
        /// <summary>
        /// Gets or sets the global log level for Unity logging.
        /// </summary>
        public static LogLevel LogLevel {
            get => UnityLogger.LogLevel;
            set => UnityLogger.LogLevel = value;
        }
    }
}