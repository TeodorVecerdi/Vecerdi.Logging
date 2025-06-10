using Microsoft.Extensions.Logging;

namespace Vecerdi.Logging.Extensions {
    /// <summary>
    /// Extension methods for integrating Vecerdi.Logging with Microsoft.Extensions.Logging.
    /// </summary>
    public static class LoggerFactoryExtensions {
        /// <summary>
        /// Adds Vecerdi.Logging support to the logger factory.
        /// This routes Microsoft.Extensions.Logging calls to the Vecerdi.Logging system.
        /// </summary>
        /// <param name="factory">The logger factory.</param>
        /// <returns>The logger factory for chaining.</returns>
        public static ILoggerFactory AddVecerdi(this ILoggerFactory factory) {
            factory.AddProvider(new VecerdiLoggerProvider());
            return factory;
        }

        /// <summary>
        /// Adds Unity logging support to the logger factory.
        /// This routes Microsoft.Extensions.Logging calls directly to Unity's Debug.Log* methods.
        /// </summary>
        /// <param name="factory">The logger factory.</param>
        /// <returns>The logger factory for chaining.</returns>
        public static ILoggerFactory AddUnity(this ILoggerFactory factory) {
            factory.AddProvider(Unity.UnityLoggerProvider.Instance);
            return factory;
        }
    }
}