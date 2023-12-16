#nullable enable

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Vecerdi.Logging.Unity.Editor {
    internal class LoggingSettingsPreprocessBuild : UnityEditor.Build.IPreprocessBuildWithReport {
        public int callbackOrder => -200;

        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report) {
            LoggingSettings settings = LoggingSettings.GetOrCreateSettings();
            settings.UpdateCategories();

            string resourcesPath = Path.Combine(Application.dataPath, "Resources", "Vecerdi.Logging");
            if (!Directory.Exists(resourcesPath)) {
                Directory.CreateDirectory(resourcesPath);
            }

            string destinationPath = Path.Combine(resourcesPath, LoggingSettings.ASSET_NAME);
            if (File.Exists(destinationPath)) {
                File.Delete(destinationPath);
            }

            string relativePath = Path.Combine("Assets", "Resources", LoggingSettings.ASSET_NAME);
            AssetDatabase.CreateAsset(settings, relativePath);
            AssetDatabase.Refresh();
        }
    }

    internal class LoggingSettingsPostprocessBuild : UnityEditor.Build.IPostprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report) {
            string resourcesPath = Path.Combine(Application.dataPath, "Resources", "Vecerdi.Logging");
            string assetPath = Path.Combine(resourcesPath, LoggingSettings.ASSET_NAME);

            if (File.Exists(assetPath)) {
                File.Delete(assetPath);
                File.Delete($"{assetPath}.meta");
            }

            if (!Directory.EnumerateFiles(resourcesPath).Any()) {
                Directory.Delete(resourcesPath);
                File.Delete($"{resourcesPath}.meta");
            }

            AssetDatabase.Refresh();
        }
    }
}