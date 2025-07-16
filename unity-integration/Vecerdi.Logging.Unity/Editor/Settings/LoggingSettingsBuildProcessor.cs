#nullable enable

using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Vecerdi.Logging.Unity.Editor.Settings {
    internal class LoggingSettingsPreprocessBuild : IPreprocessBuildWithReport {
        public int callbackOrder => -200;

        public void OnPreprocessBuild(BuildReport report) {
            LoggingSettingsCodeGenerator.GenerateEmbeddedConfiguration();
        }
    }

    internal class LoggingSettingsPostprocessBuild : IPostprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report) {
            LoggingSettingsCodeGenerator.RemoveEmbeddedConfiguration();
        }
    }
}
