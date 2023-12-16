#nullable enable

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace Vecerdi.Logging.Unity.Editor {
    public class LoggingSettingsProvider : SettingsProvider {
        private LoggingSettings? m_LoggingSettings;
        private SerializedObject? m_SerializedSettings;

        private LoggingSettings LoggingSettings {
            get {
                if (this.m_LoggingSettings == null) {
                    this.m_LoggingSettings = LoggingSettings.GetOrCreateSettings();
                }

                return this.m_LoggingSettings;
            }
        }

        private SerializedObject SerializedSettings {
            get {
                if (this.m_SerializedSettings == null || this.m_SerializedSettings.targetObject != this.LoggingSettings) {
                    this.m_SerializedSettings = new SerializedObject(this.LoggingSettings);
                    this.m_SerializedSettings.Update();
                }

                return this.m_SerializedSettings;
            }
        }

        private static StyleSheet? s_StyleSheet;

        public LoggingSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project, IEnumerable<string>? keywords = null)
            : base(path, scopes, keywords) {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement) {
            this.LoggingSettings.UpdateCategories();
            this.SerializedSettings.Update();

            this.keywords = GetSearchKeywordsFromSerializedObject(this.SerializedSettings);

            this.CreateUIElements(rootElement);
        }

        private void CreateUIElements(VisualElement rootElement) {
            if (!EditorResources.Load(ref s_StyleSheet, "LoggingSettingsStyle")) {
                return;
            }

            rootElement.Clear();
            rootElement.styleSheets.Add(s_StyleSheet);
            rootElement.name = "Vecerdi-Logging-Settings";

            Label title = new("Logging Settings");
            title.AddToClassList("title");
            rootElement.Add(title);

            VisualElement changeTracker = new();
            changeTracker.TrackSerializedObjectValue(this.SerializedSettings, _ => this.LoggingSettings.Save());
            rootElement.Add(changeTracker);

            SerializedProperty globalLogLevel = this.SerializedSettings.FindProperty("m_GlobalLogLevel");
            EnumField globalLogLevelField = new(globalLogLevel.displayName);
            globalLogLevelField.BindProperty(globalLogLevel);
            globalLogLevelField.name = "GlobalLogLevel";
            rootElement.Add(globalLogLevelField);

            SerializedProperty overrideLogLevelInBuilds = this.SerializedSettings.FindProperty("m_OverrideGlobalLogLevelInBuilds");
            SerializedProperty logLevelInBuilds = this.SerializedSettings.FindProperty("m_GlobalLogLevelInBuilds");
            VisualElement overrideContainer = new() { name = "OverrideLogLevelInBuildsContainer" };

            Toggle overrideLogLevelInBuildsToggle = new("Override in Builds");
            overrideLogLevelInBuildsToggle.BindProperty(overrideLogLevelInBuilds);
            overrideLogLevelInBuildsToggle.name = "OverrideLogLevelInBuilds";
            overrideContainer.Add(overrideLogLevelInBuildsToggle);

            EnumField logLevelInBuildsField = new("");
            logLevelInBuildsField.BindProperty(logLevelInBuilds);
            logLevelInBuildsField.name = "LogLevelInBuilds";
            overrideContainer.Add(logLevelInBuildsField);

            overrideLogLevelInBuildsToggle.RegisterValueChangedCallback(_ => {
                logLevelInBuildsField.SetEnabled(overrideLogLevelInBuildsToggle.value);
            });
            logLevelInBuildsField.SetEnabled(overrideLogLevelInBuildsToggle.value);

            rootElement.Add(overrideContainer);

            SerializedProperty categoryNameTransforms = this.SerializedSettings.FindProperty("m_CategoryNameTransforms");
            EnumFlagsField categoryNameTransformsField = new(categoryNameTransforms.displayName);
            categoryNameTransformsField.BindProperty(categoryNameTransforms);
            categoryNameTransformsField.name = "CategoryNameTransforms";
            categoryNameTransformsField.RegisterValueChangedCallback(_ => this.LoggingSettings.TransformAllCategoryNames());
            rootElement.Add(categoryNameTransformsField);

            SerializedProperty enableColoredOutputInEditor = this.SerializedSettings.FindProperty("m_EnableColoredOutputInEditor");
            Toggle enableColoredOutputInEditorToggle = new(enableColoredOutputInEditor.displayName);
            enableColoredOutputInEditorToggle.BindProperty(enableColoredOutputInEditor);
            enableColoredOutputInEditorToggle.name = "EnableColoredOutputInEditor";
            rootElement.Add(enableColoredOutputInEditorToggle);

            Label logCategoriesLabel = new("Log Categories") { name = "LogCategoriesLabel" };
            rootElement.Add(logCategoriesLabel);

            if (this.LoggingSettings.LogCategories.Count == 0) {
                CreateExampleCodeElement(rootElement);
                return;
            }

            this.CreateLogCategoriesListView(rootElement);
        }

        private void CreateLogCategoriesListView(VisualElement rootElement) {
            VisualElement header = new() { name = "LogCategoriesListHeader" };

            Label categoryNameHeader = new("Category Name");
            categoryNameHeader.AddToClassList("category-list-header__category-name");
            header.Add(categoryNameHeader);

            Label logLevelHeader = new("Log Level");
            logLevelHeader.AddToClassList("category-list-header__log-level");
            header.Add(logLevelHeader);

            rootElement.Add(header);

            SerializedProperty logCategories = this.SerializedSettings.FindProperty("m_LogCategories");

            // Create a new item template that mimics a multi-column layout
            Func<VisualElement> makeItem = () => {
                // Create the container
                VisualElement row = new();
                row.AddToClassList("category-list__row-container");

                // Create the category name field
                TextField categoryName = new() { isReadOnly = true };
                categoryName.SetEnabled(false);
                categoryName.AddToClassList("category-list__category-name");
                row.Add(categoryName);

                // Create the log level field
                EnumField logLevel = new();
                logLevel.AddToClassList("category-list__log-level");
                row.Add(logLevel);

                return row;
            };

            Action<VisualElement, int> bindItem = (element, index) => {
                SerializedProperty logCategory = logCategories.GetArrayElementAtIndex(index);

                // Bind the category name field
                SerializedProperty logCategoryName = logCategory.FindPropertyRelative("m_CategoryName");
                TextField categoryName = element.Q<TextField>(className: "category-list__category-name");
                categoryName.BindProperty(logCategoryName);
                categoryName.viewDataKey = logCategoryName.stringValue;

                // Bind the log level field
                SerializedProperty logCategoryLogLevel = logCategory.FindPropertyRelative("m_LogLevel");
                EnumField logLevel = element.Q<EnumField>(className: "category-list__log-level");
                logLevel.BindProperty(logCategoryLogLevel);
                logLevel.viewDataKey = logCategoryLogLevel.enumValueIndex.ToString();
            };

            ListView logCategoriesListView = new() {
                name = "LogCategoriesListView",
                makeItem = makeItem,
                bindItem = bindItem,
                itemsSource = this.LoggingSettings.LogCategories,
                showBoundCollectionSize = false,
            };

            logCategoriesListView.BindProperty(logCategories);
            rootElement.Add(logCategoriesListView);

            VisualElement footer = new() { name = "LogCategoriesListFooter" };
            Label countLabel = new($"{this.LoggingSettings.LogCategories.Count} log categories");
            footer.Add(countLabel);
            rootElement.Add(footer);
        }


        private static void CreateExampleCodeElement(VisualElement rootElement) {
            Label noCategoriesLabel = new("No log categories defined.\nDefine a log category in your code using the [LogCategory] attribute:") { name = "NoCategoriesLabel" };
            rootElement.Add(noCategoriesLabel);

            Label noCategoriesCodeLabel = new(
                $"{Sym("[")}{Typ("LogCategory")}{Sym("(")}{Arg("defaultLogLevel: ")}{Const("LogLevel.Debug")}{Sym(")]")}\n" +
                $"{Kwd("private const string")} {Const("LOG_CATEGORY")} {Sym("=")} {Str("\"My log category\"")}{Sym(";")}\n" +
                "\n" +
                $"{Cmt("// ...")}\n" +
                "\n" +
                $"{Typ("Log")}{Sym(".")}{Call("Information")}{Sym("(")}{Str("\"My log message\"")}{Sym(",")} {Const("LOG_CATEGORY")}{Sym(");")}"
            ) {
                name = "NoCategoriesCodeLabel",
            };
            FontAsset robotoMonoRegularAsset = (FontAsset)EditorGUIUtility.Load("RobotoMono-Regular SDF");
            noCategoriesCodeLabel.style.unityFontDefinition = new StyleFontDefinition(robotoMonoRegularAsset);
            rootElement.Add(noCategoriesCodeLabel);
            return;

            string Call(string call) => $"<color=#82aaff>{call}</color>";
            string Str(string str) => $"<color=#c1e78c>{str}</color>";
            string Kwd(string kwd) => $"<color=#c792ea>{kwd}</color>";
            string Cmt(string cmt) => $"<color=#616161>{cmt}</color>";
            string Arg(string cmt) => $"<color=#808080>{cmt}</color>";
            string Sym(string sym) => $"<color=#89ddff>{sym}</color>";
            string Const(string cnst) => $"<color=#f68b61>{cnst}</color>";
            string Typ(string typ) => $"<color=#ffcb6b>{typ}</color>";
        }

        [SettingsProvider]
        internal static SettingsProvider CreateProvider() => new LoggingSettingsProvider("Project/Logging");
    }
}