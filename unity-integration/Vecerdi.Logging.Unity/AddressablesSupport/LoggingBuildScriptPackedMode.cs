using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEngine;
using Vecerdi.Logging.Unity.Editor.Settings;

namespace Vecerdi.Logging.Unity.AddressablesSupport {
    [CreateAssetMenu(fileName = "LoggingBuildScriptPacked", menuName = "Addressables/Custom Content Builders/Packed Mode (with Logging CodeGen)")]
    public sealed class LoggingBuildScriptPackedMode : BuildScriptPackedMode {
        public override string Name => "Default Build Script (with Logging CodeGen)";

        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput context) {
            LoggingSettingsCodeGenerator.GenerateEmbeddedConfiguration();
            try {
                return base.BuildDataImplementation<TResult>(context);
            } finally {
                LoggingSettingsCodeGenerator.RemoveEmbeddedConfiguration();
            }
        }
    }
}
