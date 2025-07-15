#nullable enable

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace Vecerdi.Logging.Unity.Editor {
    // A ScriptableObject wrapper to enable Unity's SerializedObject functionality

    public class LoggingSettingsProvider : SettingsProvider {
        private LoggingSettings? m_LoggingSettings;
        private LoggingSettingsWrapper? m_SettingsWrapper;
        private SerializedObject? m_SerializedSettings;

        private LoggingSettings LoggingSettings {
            get {
                if (m_LoggingSettings == null) {
                    m_LoggingSettings = LoggingSettings.GetOrCreateSettings();
                }

                return m_LoggingSettings;
            }
        }

        private SerializedObject SerializedSettings {
            get {
                if (m_SerializedSettings == null || m_SettingsWrapper == null) {
                    if (m_SettingsWrapper == null) {
                        m_SettingsWrapper = ScriptableObject.CreateInstance<LoggingSettingsWrapper>();
                    }

                    m_SettingsWrapper.SetData(LoggingSettings.GetSerializableData());
                    m_SerializedSettings = new SerializedObject(m_SettingsWrapper);
                    m_SerializedSettings.Update();
                }

                return m_SerializedSettings;
            }
        }

        private static StyleSheet? s_StyleSheet;

        public LoggingSettingsProvider(string path, SettingsScope scopes = SettingsScope.Project, IEnumerable<string>? keywords = null)
            : base(path, scopes, keywords) { }

        public override void OnActivate(string searchContext, VisualElement rootElement) {
            LoggingSettings.UpdateCategories();

            // Refresh the wrapper with current data
            if (m_SettingsWrapper != null) {
                m_SettingsWrapper.SetData(LoggingSettings.GetSerializableData());
            }

            SerializedSettings.Update();

            keywords = GetSearchKeywordsFromSerializedObject(SerializedSettings);

            CreateUIElements(rootElement);
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
            changeTracker.TrackSerializedObjectValue(SerializedSettings, _ => {
                // Update the actual LoggingSettings with the serialized data
                var data = m_SettingsWrapper!.Data;
                var settings = LoggingSettings;
                settings.GlobalLogLevel = data.GlobalLogLevel;
                settings.CategoryNameTransforms = data.CategoryNameTransforms;
                settings.EnableColoredOutputInEditor = data.EnableColoredOutputInEditor;
                settings.OverrideGlobalLogLevelInBuilds = data.OverrideGlobalLogLevelInBuilds;
                settings.GlobalLogLevelInBuilds = data.GlobalLogLevelInBuilds;
                settings.LogMessagesOnMainThread = data.LogMessagesOnMainThread;

                // Update categories - more complex since it's a list
                settings.LogCategories.Clear();
                settings.LogCategories.AddRange(data.LogCategories);

                settings.Save();
            });
            rootElement.Add(changeTracker);

            var globalLogLevel = SerializedSettings.FindProperty("m_Data").FindPropertyRelative("GlobalLogLevel");
            EnumField globalLogLevelField = new(globalLogLevel.displayName);
            globalLogLevelField.BindProperty(globalLogLevel);
            globalLogLevelField.name = "GlobalLogLevel";
            rootElement.Add(globalLogLevelField);

            var dataProperty = SerializedSettings.FindProperty("m_Data");
            var overrideLogLevelInBuilds = dataProperty.FindPropertyRelative("OverrideGlobalLogLevelInBuilds");
            var logLevelInBuilds = dataProperty.FindPropertyRelative("GlobalLogLevelInBuilds");
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

            var categoryNameTransforms = dataProperty.FindPropertyRelative("CategoryNameTransforms");
            EnumFlagsField categoryNameTransformsField = new(categoryNameTransforms.displayName);
            categoryNameTransformsField.BindProperty(categoryNameTransforms);
            categoryNameTransformsField.name = "CategoryNameTransforms";
            categoryNameTransformsField.RegisterValueChangedCallback(_ => LoggingSettings.TransformAllCategoryNames());
            rootElement.Add(categoryNameTransformsField);

            var enableColoredOutputInEditor = dataProperty.FindPropertyRelative("EnableColoredOutputInEditor");
            Toggle enableColoredOutputInEditorToggle = new(enableColoredOutputInEditor.displayName);
            enableColoredOutputInEditorToggle.AddToClassList("vecerdi-logging-toggle");
            enableColoredOutputInEditorToggle.BindProperty(enableColoredOutputInEditor);
            enableColoredOutputInEditorToggle.name = "EnableColoredOutputInEditor";
            rootElement.Add(enableColoredOutputInEditorToggle);

            var logMessagesOnTheMainThread = dataProperty.FindPropertyRelative("LogMessagesOnMainThread");
            Toggle logMessagesOnTheMainThreadToggle = new(logMessagesOnTheMainThread.displayName);
            logMessagesOnTheMainThreadToggle.AddToClassList("vecerdi-logging-toggle");
            logMessagesOnTheMainThreadToggle.BindProperty(logMessagesOnTheMainThread);
            logMessagesOnTheMainThreadToggle.name = "LogMessagesOnMainThread";
            rootElement.Add(logMessagesOnTheMainThreadToggle);

            Label logCategoriesLabel = new("Log Categories") { name = "LogCategoriesLabel" };
            rootElement.Add(logCategoriesLabel);

            if (LoggingSettings.LogCategories.Count == 0) {
                CreateExampleCodeElement(rootElement);
                return;
            }

            CreateLogCategoriesListView(rootElement);
        }

        private void CreateLogCategoriesListView(VisualElement rootElement) {
            var logCategoriesList = new FilterableLogCategoryList(LoggingSettings.LogCategories);
            var logCategoriesProperty = SerializedSettings.FindProperty("m_Data").FindPropertyRelative("LogCategories");

            ToolbarSearchField searchField = new() { name = "LogCategoriesSearchField" };
            searchField.RegisterValueChangedCallback(evt => logCategoriesList.SetFilter(evt.newValue));
            rootElement.Add(searchField);

            VisualElement header = new() { name = "LogCategoriesListHeader" };

            Label categoryNameHeader = new("Category Name");
            categoryNameHeader.AddToClassList("category-list-header__category-name");
            VisualElement categoryNameSorting = new() { name = "category-list-header__sorting" };
            categoryNameHeader.Add(categoryNameSorting);
            header.Add(categoryNameHeader);

            Label logLevelHeader = new("Log Level");
            logLevelHeader.AddToClassList("category-list-header__log-level");
            VisualElement logLevelSorting = new() { name = "category-list-header__sorting" };
            logLevelHeader.Add(logLevelSorting);
            header.Add(logLevelHeader);

            categoryNameHeader.AddManipulator(new Clickable(() => HandleSortableClicked(FilterableLogCategoryList.SortFields.CategoryName, categoryNameSorting, logLevelSorting)));
            logLevelHeader.AddManipulator(new Clickable(() => HandleSortableClicked(FilterableLogCategoryList.SortFields.LogLevel, logLevelSorting, categoryNameSorting)));

            rootElement.Add(header);

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
                var originalIndex = logCategoriesList.GetOriginalIndex(index);
                var logCategory = logCategoriesProperty.GetArrayElementAtIndex(originalIndex);

                // Bind the category name field
                var logCategoryName = logCategory.FindPropertyRelative("m_CategoryName");
                var categoryName = element.Q<TextField>(className: "category-list__category-name");
                categoryName.BindProperty(logCategoryName);
                categoryName.viewDataKey = logCategoryName.stringValue;

                // Bind the log level field
                var logCategoryLogLevel = logCategory.FindPropertyRelative("m_LogLevel");
                var logLevel = element.Q<EnumField>(className: "category-list__log-level");
                logLevel.BindProperty(logCategoryLogLevel);
                logLevel.viewDataKey = logCategoryLogLevel.enumValueIndex.ToString();
            };

            ListView logCategoriesListView = new() {
                name = "LogCategoriesListView",
                makeItem = makeItem,
                bindItem = bindItem,
                showBoundCollectionSize = false,
                itemsSource = logCategoriesList,
            };
            rootElement.Add(logCategoriesListView);

            logCategoriesList.CollectionView = logCategoriesListView;

            var sortModes = (FilterableLogCategoryList.SortModes)EditorPrefs.GetInt("Vecerdi.Logging.SortMode", (int)FilterableLogCategoryList.SortModes.None);
            var sortFields = (FilterableLogCategoryList.SortFields)EditorPrefs.GetInt("Vecerdi.Logging.SortField", (int)FilterableLogCategoryList.SortFields.CategoryName);
            logCategoriesList.SetSort(sortModes, sortFields);

            ApplySortVisuals(categoryNameSorting, sortFields is FilterableLogCategoryList.SortFields.CategoryName ? sortModes : FilterableLogCategoryList.SortModes.None);
            ApplySortVisuals(logLevelSorting, sortFields is FilterableLogCategoryList.SortFields.LogLevel ? sortModes : FilterableLogCategoryList.SortModes.None);

            // Adjust the header margin based on the presence of the vertical scroller
            var scrollView = (ScrollView)logCategoriesListView.hierarchy[0];
            scrollView.verticalScroller.RegisterCallback<GeometryChangedEvent, ScrollView>((_, view) => {
                if (view.userData != null && (bool)view.userData == view.verticalScroller.enabledSelf) return;
                view.userData = view.verticalScroller.enabledSelf;
                logLevelHeader.style.marginRight = view.verticalScroller.enabledSelf ? view.verticalScroller.resolvedStyle.width : 0;
            }, scrollView);

            VisualElement footer = new() { name = "LogCategoriesListFooter" };
            Label countLabel = new($"{LoggingSettings.LogCategories.Count} log categories");
            footer.Add(countLabel);
            rootElement.Add(footer);

            return;

            void HandleSortableClicked(FilterableLogCategoryList.SortFields field, VisualElement sortingElement, VisualElement otherSortingElement) {
                var nextSortMode = GetNextSortMode(logCategoriesList.SortField == field ? logCategoriesList.SortMode : FilterableLogCategoryList.SortModes.None);
                logCategoriesList.SetSort(nextSortMode, field);

                ApplySortVisuals(sortingElement, nextSortMode);
                ApplySortVisuals(otherSortingElement, FilterableLogCategoryList.SortModes.None);

                EditorPrefs.SetInt("Vecerdi.Logging.SortMode", (int)nextSortMode);
                EditorPrefs.SetInt("Vecerdi.Logging.SortField", (int)field);
            }

            void ApplySortVisuals(VisualElement sortingElement, FilterableLogCategoryList.SortModes sortMode) {
                sortingElement.ClearClassList();
                if (sortMode is not FilterableLogCategoryList.SortModes.None) {
                    sortingElement.AddToClassList(sortMode is FilterableLogCategoryList.SortModes.Ascending ? "ascending" : "descending");
                }
            }

            FilterableLogCategoryList.SortModes GetNextSortMode(FilterableLogCategoryList.SortModes currentMode) {
                return currentMode switch {
                    FilterableLogCategoryList.SortModes.None => FilterableLogCategoryList.SortModes.Ascending,
                    FilterableLogCategoryList.SortModes.Ascending => FilterableLogCategoryList.SortModes.Descending,
                    FilterableLogCategoryList.SortModes.Descending => FilterableLogCategoryList.SortModes.None,
                    _ => throw new ArgumentOutOfRangeException(nameof(currentMode), currentMode, null),
                };
            }
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
            var robotoMonoRegularAsset = (FontAsset)EditorGUIUtility.Load("RobotoMono-Regular SDF");
            noCategoriesCodeLabel.style.unityFontDefinition = new StyleFontDefinition(robotoMonoRegularAsset);
            noCategoriesCodeLabel.selection.isSelectable = true;
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
        internal static SettingsProvider CreateProvider() => new LoggingSettingsProvider("Project/Vecerdi.Logging");
    }
}
