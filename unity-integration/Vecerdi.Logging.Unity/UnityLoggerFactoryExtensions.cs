using Microsoft.Extensions.Logging;

namespace Vecerdi.Logging.Unity.Extensions {
    /// <summary>
    /// Extension methods for integrating Unity logging with Microsoft.Extensions.Logging.
    /// </summary>
    public static class LoggerFactoryExtensions {
        /// <summary>
        /// Adds Unity logging support to the logger factory.
        /// This routes Microsoft.Extensions.Logging calls directly to Unity's Debug.Log* methods.
        /// </summary>
        /// <param name="factory">The logger factory.</param>
        /// <returns>The logger factory for chaining.</returns>
        public static ILoggerFactory AddUnity(this ILoggerFactory factory) {
            factory.AddProvider(UnityLoggerProvider.Instance);
            return factory;
        }
    }
}