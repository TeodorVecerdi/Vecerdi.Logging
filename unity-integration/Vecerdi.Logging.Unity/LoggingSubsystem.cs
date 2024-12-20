#nullable enable

using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Vecerdi.Logging.Unity {
    internal static partial class LoggingSubsystem {
        internal static class Threading {
            internal static int MainThreadId { get; set; }
            internal static SynchronizationContext? MainThread { get; set; }

            internal static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == MainThreadId;
        }

        [UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() {
            Threading.MainThreadId = Thread.CurrentThread.ManagedThreadId;
            Threading.MainThread = SynchronizationContext.Current;
            Register();
        }

        private static void Register() {
            Log.RegisterLogger(UnityLogger.Instance);
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    internal static partial class LoggingSubsystem {
        static LoggingSubsystem() {
            Register();
            EditorApplication.playModeStateChanged -= RegisterWhenExitingPlayMode;
            EditorApplication.playModeStateChanged += RegisterWhenExitingPlayMode;
        }

        private static void RegisterWhenExitingPlayMode(PlayModeStateChange state) {
            if (state is PlayModeStateChange.EnteredEditMode) {
                Register();
            }
        }
    }
#endif
}
