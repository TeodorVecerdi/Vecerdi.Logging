#nullable enable
using UnityEngine;

namespace Vecerdi.Logging.Unity.Editor {
    internal class LoggingSettingsWrapper : ScriptableObject {
        [SerializeField] private LoggingSettingsData m_Data = new();

        public LoggingSettingsData Data => m_Data;

        public void SetData(LoggingSettingsData data) {
            m_Data = data;
        }
    }
}
