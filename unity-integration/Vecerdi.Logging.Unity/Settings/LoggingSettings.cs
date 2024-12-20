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
    public class LoggingSettings : ScriptableObject {
        public const string ASSET_NAME = "LoggingSettings.asset";
        public static string LoggingSettingsPath => Path.Combine(CustomProjectSettings.ProjectSettingsPath, ASSET_NAME);
        private static LoggingSettings? s_Instance;

        [Tooltip("This takes precedence over the individual log categories. If set to None, all logging is disabled.")]
        [SerializeField] private LogLevel m_GlobalLogLevel = LogLevel.Debug;
        [SerializeField] private CategoryNameTransform m_CategoryNameTransforms = CategoryNameTransform.AllUppercase | CategoryNameTransform.ReplaceSpacesWithUnderscores;
        [SerializeField] private bool m_EnableColoredOutputInEditor;
        [SerializeField] private List<LogCategory> m_LogCategories = new();
        [SerializeField] private bool m_OverrideGlobalLogLevelInBuilds;
        [SerializeField] private LogLevel m_GlobalLogLevelInBuilds = LogLevel.Information;
        [SerializeField] private bool m_LogMessagesOnMainThread;

        private bool m_IsDirty;

#if UNITY_EDITOR
        public LogLevel GlobalLogLevel {
            get => m_GlobalLogLevel;
            set => m_GlobalLogLevel = value;
        }
#else
        public LogLevel GlobalLogLevel {
            get => m_OverrideGlobalLogLevelInBuilds ? m_GlobalLogLevelInBuilds : m_GlobalLogLevel;
            set {
                if (m_OverrideGlobalLogLevelInBuilds) {
                    m_GlobalLogLevelInBuilds = value;
                } else {
                    m_GlobalLogLevel = value;
                }
            }
        }
#endif

        public bool OverrideGlobalLogLevelInBuilds {
            get => m_OverrideGlobalLogLevelInBuilds;
            set => m_OverrideGlobalLogLevelInBuilds = value;
        }

        public LogLevel GlobalLogLevelInBuilds {
            get => m_GlobalLogLevelInBuilds;
            set => m_GlobalLogLevelInBuilds = value;
        }

        public List<LogCategory> LogCategories => m_LogCategories;

        public Dictionary<string, LogCategory> LogCategoriesByName { get; private set; } = new(StringComparer.OrdinalIgnoreCase);

        public CategoryNameTransform CategoryNameTransforms {
            get => m_CategoryNameTransforms;
            set => m_CategoryNameTransforms = value;
        }

        public bool EnableColoredOutputInEditor {
            get => m_EnableColoredOutputInEditor;
            set => m_EnableColoredOutputInEditor = value;
        }

        public bool LogMessagesOnMainThread {
            get => m_LogMessagesOnMainThread;
            set => m_LogMessagesOnMainThread = value;
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
                    m_LogCategories.Add(new LogCategory(categoryName, value, attribute.DefaultLogLevel));
                    dirty = true;
                }
            }

            LogCategoriesByName = m_LogCategories.ToDictionary(logCategory => logCategory.OriginalCategoryName, logCategory => logCategory);
            dirty |= m_LogCategories.RemoveAll(logCategory => !existingCategories.Contains(logCategory.OriginalCategoryName)) > 0;
            if (dirty) {
                Save();
            }
#else
            this.LogCategoriesByName = this.m_LogCategories.ToDictionary(logCategory => logCategory.OriginalCategoryName, logCategory => logCategory);
#endif
        }

        public bool ContainsCategoryWithName(string categoryName) {
            return m_LogCategories.Any(logCategory => string.Equals(logCategory.OriginalCategoryName, categoryName, StringComparison.OrdinalIgnoreCase));
        }

        public void Save() {
            var json = JsonUtility.ToJson(this, true);
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

#if UNITY_EDITOR
            DirectoryInfo directoryInfo = new(CustomProjectSettings.ProjectSettingsPath);
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }

            if (File.Exists(LoggingSettingsPath)) {
                string json = File.ReadAllText(LoggingSettingsPath);
                s_Instance = CreateInstance<LoggingSettings>();
                JsonUtility.FromJsonOverwrite(json, s_Instance);
            } else {
                s_Instance = CreateInstance<LoggingSettings>();
                s_Instance.Save();
            }
#else
            s_Instance = Resources.Load<LoggingSettings>("Vecerdi.Logging/LoggingSettings");
#endif
            s_Instance.UpdateCategories();
            return s_Instance;
        }

        public void TransformAllCategoryNames() {
            foreach (var logCategory in m_LogCategories) {
                logCategory.CategoryName = TransformCategoryName(logCategory.OriginalCategoryName);
            }

            Save();
        }

        public string TransformCategoryName(string categoryName) {
            if (m_CategoryNameTransforms.HasFlag(CategoryNameTransform.AllUppercase)) {
                categoryName = categoryName.ToUpperInvariant();
            } else if (m_CategoryNameTransforms.HasFlag(CategoryNameTransform.AllLowercase)) {
                categoryName = categoryName.ToLowerInvariant();
            }

            if (m_CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithUnderscores)) {
                categoryName = categoryName.Replace(' ', '_');
            } else if (m_CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithDashes)) {
                categoryName = categoryName.Replace(' ', '-');
            } else if (m_CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithDots)) {
                categoryName = categoryName.Replace(' ', '.');
            } else if (m_CategoryNameTransforms.HasFlag(CategoryNameTransform.ReplaceSpacesWithSlashes)) {
                categoryName = categoryName.Replace(' ', '/');
            }

            return categoryName;
        }
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