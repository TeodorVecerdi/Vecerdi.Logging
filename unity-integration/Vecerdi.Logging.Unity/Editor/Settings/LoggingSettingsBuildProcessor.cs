#nullable enable

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Vecerdi.Logging.Unity.Editor {
    internal class LoggingSettingsPreprocessBuild : IPreprocessBuildWithReport {
        public int callbackOrder => -200;

        public void OnPreprocessBuild(BuildReport report) {
            var settings = LoggingSettings.GetOrCreateSettings();
            settings.UpdateCategories();

            // Copy JSON settings file to StreamingAssets
            var resourcesPath = Path.Combine(Application.dataPath, "Resources", "Vecerdi.Logging");
            if (!Directory.Exists(resourcesPath)) {
                Directory.CreateDirectory(resourcesPath);
            }

            var destinationPath = Path.Combine(resourcesPath, LoggingSettings.SETTINGS_FILE_NAME);
            if (File.Exists(destinationPath)) {
                File.Delete(destinationPath);
            }

            // Copy the JSON file
            if (File.Exists(LoggingSettings.LoggingSettingsPath)) {
                File.Copy(LoggingSettings.LoggingSettingsPath, destinationPath);
            }

            AssetDatabase.Refresh();
        }
    }

    internal class LoggingSettingsPostprocessBuild : IPostprocessBuildWithReport {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report) {
            var resourcesPath = Path.Combine(Application.dataPath, "Resources", "Vecerdi.Logging");
            var settingsPath = Path.Combine(resourcesPath, LoggingSettings.SETTINGS_FILE_NAME);

            if (File.Exists(settingsPath)) {
                File.Delete(settingsPath);
                File.Delete($"{settingsPath}.meta");
            }

            if (Directory.Exists(resourcesPath) && !Directory.EnumerateFiles(resourcesPath).Any()) {
                Directory.Delete(resourcesPath);
                File.Delete($"{resourcesPath}.meta");
            }

            var parentDirectory = Directory.GetParent(resourcesPath)!.FullName;
            if (Directory.Exists(parentDirectory) && !Directory.EnumerateFiles(parentDirectory).Any()) {
                Directory.Delete(parentDirectory);
                File.Delete($"{parentDirectory}.meta");
            }

            AssetDatabase.Refresh();
        }
    }
}