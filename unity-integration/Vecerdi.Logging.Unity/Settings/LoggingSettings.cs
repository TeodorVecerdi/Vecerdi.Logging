#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Pool;
using System.Reflection;
#endif

namespace Vecerdi.Logging.Unity {
    [Serializable]
    public class LoggingSettingsData {
        [Tooltip("This takes precedence over the individual log categories. If set to None, all logging is disabled.")]
        [SerializeField] public LogLevel GlobalLogLevel = LogLevel.Debug;
        [SerializeField] public CategoryNameTransform CategoryNameTransforms = CategoryNameTransform.AllUppercase | CategoryNameTransform.ReplaceSpacesWithUnderscores;
        [SerializeField] public bool EnableColoredOutputInEditor;
        [SerializeField] public List<LogCategory> LogCategories = new();
        [SerializeField] public bool OverrideGlobalLogLevelInBuilds;
        [SerializeField] public LogLevel GlobalLogLevelInBuilds = LogLevel.Information;
        [SerializeField] public bool LogMessagesOnMainThread;
    }

    public class LoggingSettings {
        public const string SETTINGS_FILE_NAME = "LoggingSettings.json";
        public static string LoggingSettingsPath => Path.Combine(CustomProjectSettings.ProjectSettingsPath, SETTINGS_FILE_NAME);
        public static string RuntimeSettingsPath => Path.Combine(Application.streamingAssetsPath, "Vecerdi.Logging", SETTINGS_FILE_NAME);
        private static LoggingSettings? s_Instance;

        private LoggingSettingsData m_Data = new();

#if UNITY_EDITOR
        public LogLevel GlobalLogLevel {
            get => m_Data.GlobalLogLevel;
            set => m_Data.GlobalLogLevel = value;
        }
#else
        public LogLevel GlobalLogLevel {
            get => m_Data.OverrideGlobalLogLevelInBuilds ? m_Data.GlobalLogLevelInBuilds : m_Data.GlobalLogLevel;
            set {
                if (m_Data.OverrideGlobalLogLevelInBuilds) {
                    m_Data.GlobalLogLevelInBuilds = value;
                } else {
                    m_Data.GlobalLogLevel = value;
                }
            }
        }
#endif

        public bool OverrideGlobalLogLevelInBuilds {
            get => m_Data.OverrideGlobalLogLevelInBuilds;
            set => m_Data.OverrideGlobalLogLevelInBuilds = value;
        }

        public LogLevel GlobalLogLevelInBuilds {
            get => m_Data.GlobalLogLevelInBuilds;
            set => m_Data.GlobalLogLevelInBuilds = value;
        }

        public List<LogCategory> LogCategories => m_Data.LogCategories;

        public Dictionary<string, LogCategory> LogCategoriesByName { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public CategoryNameTransform CategoryNameTransforms {
            get => m_Data.CategoryNameTransforms;
            set => m_Data.CategoryNameTransforms = value;
        }

        public bool EnableColoredOutputInEditor {
            get => m_Data.EnableColoredOutputInEditor;
            set => m_Data.EnableColoredOutputInEditor = value;
        }

        public bool LogMessagesOnMainThread {
            get => m_Data.LogMessagesOnMainThread;
            set => m_Data.LogMessagesOnMainThread = value;
        }

        public void UpdateCategories() {
#if UNITY_EDITOR
            using PooledObject<HashSet<string>> _ = CollectionPool<HashSet<string>, string>.Get(out HashSet<string> existingCategories);
            bool dirty = false;

            foreach (FieldInfo fieldInfo in TypeCache.GetFieldsWithAttribute<LogCategoryAttribute>()) {
                if (!fieldInfo.IsStatic || fieldInfo.FieldType != typeof(string)) {
                    continue;
                }

                LogCategoryAttribute attribute = fieldInfo.GetCustomAttribute<LogCategoryAttribute>();
                string value = (fieldInfo.GetRawConstantValue() as string)!;
                existingCategories.Add(value);

                if (!ContainsCategoryWithName(value)) {
                    string categoryName = TransformCategoryName(value);
                    m_Data.LogCategories.Add(new LogCategory(categoryName, value, attribute.DefaultLogLevel));
                    dirty = true;
                }
            }

            LogCategoriesByName = m_Data.LogCategories.ToDictionary(logCategory => logCategory.OriginalCategoryName, logCategory => logCategory);
            dirty |= m_Data.LogCategories.RemoveAll(logCategory => !existingCategories.Contains(logCategory.OriginalCategoryName)) > 0;
            if (dirty) {
                Save();
            }
#else
            this.LogCategoriesByName = this.m_Data.LogCategories.ToDictionary(logCategory => logCategory.OriginalCategoryName, logCategory => logCategory);
#endif
        }

        public bool ContainsCategoryWithName(string categoryName) {
            return m_Data.LogCategories.Any(logCategory => string.Equals(logCategory.OriginalCategoryName, categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public void Save() {
            var json = JsonUtility.ToJson(m_Data, true);
            DirectoryInfo directoryInfo = new(CustomProjectSettings.ProjectSettingsPath);
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }

            File.WriteAllText(LoggingSettingsPath, json);
        }

        public static LoggingSettings GetOrCreateSettings() {
            if (s_Instance != null) {
                return s_Instance;
            }

            s_Instance = new LoggingSettings();

#if UNITY_EDITOR
            DirectoryInfo directoryInfo = new(CustomProjectSettings.ProjectSettingsPath);
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }

            if (File.Exists(LoggingSettingsPath)) {
                string json = File.ReadAllText(LoggingSettingsPath);
                JsonUtility.FromJsonOverwrite(json, s_Instance.m_Data);
            } else {
                s_Instance.Save();
            }
#else
            // In builds, try StreamingAssets first, then fallback to Resources folder
            string settingsPath = RuntimeSettingsPath;
            if (File.Exists(settingsPath)) {
                string json = File.ReadAllText(settingsPath);
                JsonUtility.FromJsonOverwrite(json, s_Instance.m_Data);
            } else {
                // Fallback to legacy Resources loading for backward compatibility
                var legacySettings = Resources.Load<ScriptableObject>("Vecerdi.Logging/LoggingSettings");
                if (legacySettings != null) {
                    string json = JsonUtility.ToJson(legacySettings);
                    JsonUtility.FromJsonOverwrite(json, s_Instance.m_Data);
                }
            }
#endif
            s_Instance.UpdateCategories();
            return s_Instance;
        }

        // Internal method for testing purposes only
        internal static void _ResetInstance() {
            s_Instance = null;
        }

        public void TransformAllCategoryNames() {
            foreach (var logCategory in m_Data.LogCategories) {
                logCategory.CategoryName = TransformCategoryName(logCategory.OriginalCategoryName);
            }

            Save();
        }

        public string TransformCategoryName(string categoryName) {
            if (m_Data.CategoryNameTransforms.HasFlag(CategoryNameTransform.AllUppercase)) {
                categoryName = categoryName.ToUpperInvariant();
            } else if (m_Data.CategoryNameTransforms.HasFlag(CategoryNameTransform.AllLowercase)) {
                categoryName = categoryName.ToLowerInvariant();
            }

            if (m_Data.CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithUnderscores)) {
                categoryName = categoryName.Replace(' ', '_');
            } else if (m_Data.CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithDashes)) {
                categoryName = categoryName.Replace(' ', '-');
            } else if (m_Data.CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithDots)) {
                categoryName = categoryName.Replace(' ', '.');
            } else if (m_Data.CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithSlashes)) {
                categoryName = categoryName.Replace(' ', '/');
            }

            return categoryName;
        }

        // For compatibility with SerializedObject in the settings provider
        internal LoggingSettingsData GetSerializableData() => m_Data;
    }

    [Flags]
    public enum CategoryNameTransform {
        // None = 0,
        AllUppercase = 1 << 0,
        AllLowercase = 1 << 1,
        ReplaceSpacesWithUnderscores = 1 << 2,
        ReplaceSpacesWithDashes = 1 << 3,
        ReplaceSpacesWithDots = 1 << 4,
        ReplaceSpacesWithSlashes = 1 << 5,
    }

    [Serializable]
    public class LogCategory {
        [SerializeField] private string m_CategoryName;
        [SerializeField] private string m_OriginalCategoryName;
        [SerializeField] private LogLevel m_LogLevel;

        public string CategoryName {
            get => m_CategoryName;
            set => m_CategoryName = value;
        }
        public string OriginalCategoryName => m_OriginalCategoryName;
        public LogLevel LogLevel => m_LogLevel;

        public LogCategory(string categoryName, string originalCategoryName, LogLevel logLevel) {
            m_CategoryName = categoryName;
            m_OriginalCategoryName = originalCategoryName;
            m_LogLevel = logLevel;
        }
    }
}