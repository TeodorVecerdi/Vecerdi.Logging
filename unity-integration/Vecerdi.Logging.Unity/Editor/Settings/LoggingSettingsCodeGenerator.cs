using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Vecerdi.Logging.Unity.Editor.Settings {
    public static class LoggingSettingsCodeGenerator {
        public static void GenerateEmbeddedConfiguration() {
            var settings = LoggingSettings.GetOrCreateSettings();
            settings.UpdateCategories();
            settings.Save();

            // Create the Generated folder if it doesn't exist
            var generatedPath = Path.Combine(Application.dataPath, "Generated");
            if (!Directory.Exists(generatedPath)) {
                Directory.CreateDirectory(generatedPath);
            }

            var loggingGeneratedPath = Path.Combine(generatedPath, "Vecerdi.Logging");
            if (!Directory.Exists(loggingGeneratedPath)) {
                Directory.CreateDirectory(loggingGeneratedPath);
            }

            // Read the JSON configuration
            string json = "{}";
            if (File.Exists(LoggingSettings.LoggingSettingsPath)) {
                json = File.ReadAllText(LoggingSettings.LoggingSettingsPath);
            }

            // Generate the C# class using verbatim string literal
            var classContent = $@"// This file is auto-generated during build. Do not modify manually.
#nullable enable

namespace Vecerdi.Logging.Unity {{
    internal static class GeneratedLoggingConfiguration {{
        public const string EmbeddedJson = @""{json.Replace("\"", "\"\"")}"";
    }}
}}";

            var classPath = Path.Combine(loggingGeneratedPath, "GeneratedLoggingConfiguration.cs");
            File.WriteAllText(classPath, classContent);

            // Create asmref file to include this in the Vecerdi.Logging.Unity assembly
            var asmrefContent = @"{
    ""reference"": ""Vecerdi.Logging.Unity""
}";
            var asmrefPath = Path.Combine(loggingGeneratedPath, "Vecerdi.Logging.Unity.asmref");
            File.WriteAllText(asmrefPath, asmrefContent);

            AssetDatabase.Refresh();
        }

        public static void RemoveEmbeddedConfiguration() {
            // Clean up generated files after build
            var generatedPath = Path.Combine(Application.dataPath, "Generated", "Vecerdi.Logging");
            if (Directory.Exists(generatedPath)) {
                Directory.Delete(generatedPath, true);

                // Delete meta files
                var metaPath = $"{generatedPath}.meta";
                if (File.Exists(metaPath)) {
                    File.Delete(metaPath);
                }

                // Clean up parent directory if empty
                var parentPath = Path.Combine(Application.dataPath, "Generated");
                if (Directory.Exists(parentPath) && !Directory.EnumerateFileSystemEntries(parentPath).Any()) {
                    Directory.Delete(parentPath);
                    var parentMetaPath = $"{parentPath}.meta";
                    if (File.Exists(parentMetaPath)) {
                        File.Delete(parentMetaPath);
                    }
                }
            }

            AssetDatabase.Refresh();
        }
    }
}
