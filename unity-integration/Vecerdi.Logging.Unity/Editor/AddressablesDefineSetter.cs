using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace Vecerdi.Logging.Unity.Editor {
    [InitializeOnLoad]
    internal static class AddressablesDefineSetter {
        private const string DefineSymbol = "UNITY_ADDRESSABLES";
        private const string SuppressKey = "UNITY_ADDRESSABLES_SuppressDialog";
        private static readonly ListRequest s_ListRequest;

        static AddressablesDefineSetter() {
            if (EditorPrefs.GetBool(SuppressKey, false)) return;
            s_ListRequest = Client.List(true);
            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate() {
            if (!s_ListRequest.IsCompleted)
                return;
            UpdateDefines(s_ListRequest.Result);
            EditorApplication.update -= OnEditorUpdate;
        }

        private static void UpdateDefines(PackageCollection packages) {
            var hasPkg = packages.Any(p => p.name == "com.unity.addressables");
            var group = EditorUserBuildSettings.selectedBuildTargetGroup;

            var defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';').ToList();
            var changed = false;

            if (hasPkg && !defs.Contains(DefineSymbol)) {
                // only ask if not already suppressed
                if (!EditorPrefs.GetBool(SuppressKey, false)) {
                    var choice = EditorUtility.DisplayDialogComplex(
                        "Addressables package detected",
                        $"The Addressables package was detected. Would you like to add the define '{DefineSymbol}' to the scripting define symbols to enable addressables support? This is required for the logging subsystem to work properly.",
                        "Yes",
                        "No",
                        "No, don't remind me again"
                    );

                    if (choice == 0) {
                        defs.Add(DefineSymbol);
                        changed = true;
                        EditorUtility.DisplayDialog("Addressables package detected", $"The define '{DefineSymbol}' was added to the scripting define symbols.\n\nYou need to replace the default addressables build script with one created using Addressables > Custom Content Builders > Packed Mode (with Logging CodeGen)", "OK");
                    } else if (choice == 2) {
                        EditorPrefs.SetBool(SuppressKey, true);
                    }
                }
            } else if (!hasPkg && defs.Remove(DefineSymbol)) {
                // addressables removed, clean up define
                changed = true;
            }

            if (changed) {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, string.Join(";", defs));
            }
        }
    }
}
