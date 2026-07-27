using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UIElements;

namespace YummyDev.PlayerPrefsTool
{
    public sealed class PlayerPrefsEditorView : IDisposable
    {
        // ─── Asset paths ──────────────────────────────────────────────────────

        public const string StyleSheetResourcePath = "PlayerPrefsEditor/PlayerPrefsEditorStyles";
        public const string UxmlResourcePath = "PlayerPrefsEditor/PlayerPrefsEditor";
        public const string CellTemplatesResourcePath = "PlayerPrefsEditor/PlayerPrefsEditor.Templates";

        // ─── Element names ────────────────────────────────────────────────────

        private const string NameStatusLabel   = "status-label";
        private const string NameErrorBanner   = "error-banner";
        private const string NameFilterKey     = "filter-key";
        private const string NameFilterType    = "filter-type";
        private const string NameFilterValue   = "filter-value";
        private const string NameSelectedCountLabel = "selected-count-label";
        private const string NameListContainer = "list-container";
        private const string NameSnapshotsPanel = "snapshots-panel";
        private const string NameSnapshotsListContainer = "snapshots-list-container";
        private const string NameSnapshotNameField = "snapshot-name";
        private const string NameSnapshotRowName = "snapshot-row-name";
        private const string NameSnapshotRowCount = "snapshot-row-count";
        private const string NameSnapshotRowSize = "snapshot-row-size";
        private const string NameSnapshotRowCreated = "snapshot-row-created";
        private const string NameSnapshotRowLoad = "snapshot-row-load";
        private const string NameSnapshotRowDelete = "snapshot-row-delete";
        private const string NameList          = "ppe-list";
        private const string NameStatusRow     = "status-row";
        private const string NameMobileQuickActions = "mobile-quick-actions";
        private const string NameTabsScrollView = "tabs-scroll";
        private const string NameTabsVisualElement   = "tabs-toolbar";
        private const string NameTabSnapshots  = "tab-snapshots";
        private const string NameTabKeys       = "tab-keys";
        private const string NameEditBtn       = "edit-btn";
        private const string NameFavBtn        = "fav-btn";
        private const string NameRestoreBtn    = "restore-btn";
        private const string NameKeyField      = "key-field";
        private const string NameTypeField     = "type-field";
        private const string NameValueField    = "value-field";
        private const string NameDelBtn        = "del-btn";
        private const string NameDupIcon       = "dup-icon";
        private const string NameErrIcon       = "err-icon";

        // ─── VisualElement button names ─────────────────────────────────────────────

        private const string NameBtnAddNew    = "btn-add-new";
        private const string NameBtnAddNewMobile = "btn-add-new-mobile";
        private const string NameBtnRestoreSelected = "btn-restore-selected";
        private const string NameBtnDeleteSelected = "btn-delete-selected";
        private const string NameBtnDeleteAll = "btn-delete-all";
        private const string NameBtnSave      = "btn-save";
        private const string NameBtnRefresh   = "btn-refresh";
        private const string NameBtnSaveMobile = "btn-save-mobile";
        private const string NameBtnRefreshMobile = "btn-refresh-mobile";
        private const string NameBtnExport    = "btn-export";
        private const string NameBtnImport    = "btn-import";
        private const string NameBtnActionsFold = "btn-actions-fold";
        private const string NameBtnAddGroup  = "btn-add-group";
        private const string NameBtnMoveSelected = "btn-move-selected";
        private const string NameBtnSaveSnapshot = "btn-save-snapshot";
        private const string NameBtnRefreshSnapshots = "btn-refresh-snapshots";

        // ─── Cell template names ──────────────────────────────────────────────

        private const string TplCellSelect  = "tpl-cell-select";
        private const string TplCellKey     = "tpl-cell-key";
        private const string TplCellFavorite = "tpl-cell-favorite";
        private const string TplCellType    = "tpl-cell-type";
        private const string TplCellValue   = "tpl-cell-value";
        private const string TplCellEdit    = "tpl-cell-edit";
        private const string TplCellRowAction = "tpl-cell-row-action";

        // ─── USS class names ──────────────────────────────────────────────────

        private const string ClassRoot          = "ppe-root";
        private const string ClassHidden        = "ppe-hidden";
        private const string ClassPhonePortrait = "ppe-phone-portrait";
        private const string ClassPhoneNarrow = "ppe-phone-narrow";
        private const string ClassSelectAllHeaderRegistered = "ppe-select-header--registered";
        private const string ClassFieldReadonly = "ppe-field--readonly";
        private const string ClassFieldInvalid  = "ppe-field--invalid";
        private const string ClassRowNew        = "ppe-row--new";
        private const string ClassRowDeleted    = "ppe-row--deleted";
        private const string ClassRowEdited     = "ppe-row--edited";
        private const string ClassRowDuplicate  = "ppe-row--duplicate";
        private const string ClassRowOdd        = "ppe-row--odd";
        private const string ClassRowSelected   = "ppe-row--selected";
        private const string ClassTabActive     = "ppe-tab--active";
        private const string ClassDialogOverlay = "ppe-dialog-overlay";
        private const string ClassDialog = "ppe-dialog";
        private const string ClassDialogTitle = "ppe-dialog-title";
        private const string ClassDialogMessage = "ppe-dialog-message";
        private const string ClassDialogBody = "ppe-dialog-body";
        private const string ClassDialogActions = "ppe-dialog-actions";
        private const string ClassDialogButton = "ppe-dialog-button";
        private const string ClassDialogDangerButton = "ppe-dialog-button--danger";
        private const string ClassDialogField = "ppe-dialog-field";
        private const string ClassDialogError = "ppe-dialog-error";
        private const string ClassDialogScroll = "ppe-dialog-scroll";
        private const string ClassMoveGroupList = "ppe-move-group-list";
        private const string ClassMoveGroupButton = "ppe-move-group-button";

        // ─── Column names ─────────────────────────────────────────────────────

        private const string ColSelect   = "select";
        private const string ColFavorite = "favorite";
        private const string ColKey     = "key";
        private const string ColType    = "type";
        private const string ColValue   = "value";
        private const string ColEdit    = "edit";
        private const string ColRowAction = "row-action";
        private const string ColSnapshotName = "snapshot-name";
        private const string ColSnapshotCount = "snapshot-count";
        private const string ColSnapshotSize = "snapshot-size";
        private const string ColSnapshotCreated = "snapshot-created";
        private const string ColSnapshotActions = "snapshot-actions";

        // ─── Column titles ────────────────────────────────────────────────────

        private const string ColTitleKey   = "Key";
        private const string ColTitleType  = "Type";
        private const string ColTitleValue = "Value";
        private const string ColTitleSnapshotName = "File";
        private const string ColTitleSnapshotCount = "Rows";
        private const string ColTitleSnapshotSize = "Size";
        private const string ColTitleSnapshotCreated = "Created";

        // ─── Filter-header sync ───────────────────────────────────────────────

        private const string ClassMultiColumnHeader = "unity-multi-column-header";
        private const string ClassFilterColKey      = "ppe-filter-col-key";
        private const string ClassFilterColType     = "ppe-filter-col-type";
        private const string ClassFilterColValue    = "ppe-filter-col-value";

        // ─── Action-button texts / tooltips ───────────────────────────────────

        private const string BtnTextRestore = "R";
        private const string BtnTextDelete  = "X";
        private const string BtnTextDeleteSelected = "X Delete Selected";
        private const string BtnTextRestoreSelected = "Restore Selected";
        private const string BtnTextMoveSelected = "Move To Group";
        private const string BtnTextEdit = "✎";
        private const string SelectedCountFmt = "Selected: {0}";
        private const string TooltipRestore = "Restore";
        private const string TooltipDelete  = "Delete";
        private const string TooltipRestoreValue = "Revert value";
        private const string TooltipEditValue = "Edit value in separate window";
        private const string BtnTextFavorite = "★";
        private const string BtnTextNotFavorite = "☆";
        private const string TooltipFavorite = "Remove from favorites";
        private const string TooltipNotFavorite = "Add to favorites";

        // ─── Filters / tabs ───────────────────────────────────────────────────

        private const string TypeFilterAll = "All";
        private const string GroupMain = "Main";
        private const string GroupIgnored = "Ignored";
        private const string TabTextSnapshots = "Snapshots";
        private const string TabTextKeys = "Main";
        private const string DialogTitleInvalidGroup = "Invalid Group";
        private const string DialogTitleDeleteGroup = "Delete Group";
        private const string DialogTitleNewGroup = "New Group";
        private const string MsgGroupNameRequired = "Enter a group name.";
        private const string MsgGroupNameReserved = "This group already exists.";
        private const string MsgDeleteGroupFmt = "Delete group \"{0}\"?\n\nAll keys in it will be moved to Main.";
        private const string DialogTitleEditValue = "Edit PlayerPref Value";
        private const string DialogTitleDeleteAll = "Delete All PlayerPrefs";
        private const string MsgDeleteAll =
            "Delete all PlayerPrefs and all PlayerPrefs editor groups now?\n\nThis cannot be undone.";
        private const string DialogBtnDelete = "Delete";

        // ─── New-entry defaults ───────────────────────────────────────────────

        private const string DefaultNewKey    = "new_key";
        private const string DefaultNewTypeId = "string";

        // ─── Validation messages ──────────────────────────────────────────────

        private const string MsgDuplicateKeys    = "Warning: Duplicate keys: {0}. Save is blocked until all keys are unique.";
        private const string TooltipDuplicateKey = "Duplicate key \"{0}\"";
        private const string TooltipInvalidValue = "Invalid value for type {0}";

        // ─── Status bar ───────────────────────────────────────────────────────

        private const string StatusProjectFmt  = "unity.{0}.{1}";
        private const string StatusCountFmt    = "{0} / {1} entries";
        private const string StatusTotalFmt    = "{0} entries";
        private const string StatusNewFmt      = "{0} new";
        private const string StatusEditedFmt   = "{0} edited";
        private const string StatusDeletedFmt  = "{0} to delete";
        private const string StatusSeparator   = "  |  ";

        // ─── Export / Import dialogs ──────────────────────────────────────────

        private const string FileExtJson             = "json";
        private const string ExportFileNameFmt       = "PlayerPrefs_{0}";
        private const string DialogTitleExport       = "Export PlayerPrefs to JSON";
        private const string DialogTitleExportGroups = "Export Groups";
        private const string DialogTitleExportDone   = "Export Complete";
        private const string DialogTitleExportError  = "Export Error";
        private const string MsgExportSuccess        = "Exported {0} entries to:\n{1}";
        private const string MsgExportGroupsEmpty    = "Select at least one group to export.";
        private const string DialogTitleImport       = "Import PlayerPrefs from JSON";
        private const string DialogTitleImportError  = "Import Error";
        private const string DialogTitleImportResult = "Import";
        private const string DialogTitleImportChoice = "Import PlayerPrefs";
        private const string MsgImportEmpty          = "No valid entries found in the file.";
        private const string MsgImportParseError     = "Failed to parse JSON:\n{0}";
        private const string MsgImportChoice         =
            "Found {0} entries in:\n{1}\n\n" +
            "Merge – adds new keys and edits existing ones (shown in green/blue).\n" +
            "Replace All – discards current prefs and loads the file (all shown in green).";
        private const string DialogBtnOk         = "OK";
        private const string DialogBtnMerge      = "Merge";
        private const string DialogBtnCancel     = "Cancel";
        private const string DialogBtnReplaceAll = "Replace All";
        private const string DialogBtnOverwrite  = "Overwrite";

        // ─── Snapshots ───────────────────────────────────────────────────────

        private const string SnapshotDirectoryName = "PlayerPrefsSnapshots";
        private const string SnapshotFileExtension = ".playerprefs-snapshot.json";
        private const string SnapshotDefaultNameFmt = "Snapshot {0}";
        private const string SnapshotDefaultDateFormat = "yyyy-MM-dd HH-mm-ss";
        private const string SnapshotDisplayDateFormat = "yyyy-MM-dd HH:mm";
        private const string SnapshotEmptyNameFallback = "PlayerPrefs";
        private const string DialogTitleSnapshotSaveError = "Save Snapshot Error";
        private const string DialogTitleSnapshotOverwrite = "Overwrite Snapshot";
        private const string DialogTitleSnapshotLoad = "Load Snapshot";
        private const string DialogTitleSnapshotLoadError = "Load Snapshot Error";
        private const string DialogTitleSnapshotDelete = "Delete Snapshot";
        private const string DialogTitleSnapshotDeleteError = "Delete Snapshot Error";
        private const string MsgSnapshotSavedFmt = "Saved {0} entries to snapshot \"{1}\".";
        private const string MsgSnapshotDuplicateBlocked = "Snapshot save is blocked until duplicate keys are fixed.";
        private const string MsgSnapshotOverwriteConfirmFmt =
            "Snapshot \"{0}\" already exists.\n\nOverwrite it with the current PlayerPrefs?";
        private const string MsgSnapshotLoadConfirmFmt =
            "Load snapshot \"{0}\"?\n\nCurrent PlayerPrefs will be replaced.";
        private const string MsgSnapshotLoadedFmt = "Loaded {0} entries from snapshot \"{1}\".";
        private const string MsgSnapshotDeleteConfirmFmt = "Delete snapshot \"{0}\"?";
        private const string MsgSnapshotDeletedFmt = "Deleted snapshot \"{0}\".";
        private const string MsgSnapshotsEmpty = "No snapshots found.";
        private const string DialogBtnLoad = "Load";

        // ─── Row-height resize handle ─────────────────────────────────────────

        private const string NameRowResizeHandle  = "row-resize-handle";
        private const string EditorPrefsRowHeight = "Yummy.PlayerPrefs.RowHeight";
        private const string EditorPrefsFavorites = "Yummy.PlayerPrefs.FavoriteKeys";
        private const string EditorPrefsIgnored   = "Yummy.PlayerPrefs.IgnoredKeys";
        private const string EditorPrefsGroups    = "Yummy.PlayerPrefs.Groups";
        private const string EditorPrefsKeyGroups = "Yummy.PlayerPrefs.KeyGroups";

        private const int FocusKeyFieldMaxAttempts = 16;
        private const int FocusKeyFieldRetryDelayMs = 50;
        private const float  RowHeightMin         = 18f;
        private const float  RowHeightMax         = 120f;
        private const float  RowHeightDefault     = 24f;
        private const float  PhoneRowHeight = 54f;
        private const float  SnapshotRowHeightDefault = 28f;
        private const float  PhoneSnapshotRowHeight = 46f;
        private const float  PhoneTabsScrollMaxHeight = 168f;
        private const float  NarrowPhoneTabsScrollMaxHeight = 148f;
        private const float  NarrowPhoneLayoutMaxWidth = 520f;
        private const float  EditorSimulatorPortraitMinAspect = 1.08f;
        private const long   FilterDebounceMs     = 80;

        // ─── State ────────────────────────────────────────────────────────────

        private List<PlayerPrefStore> _prefs          = new List<PlayerPrefStore>();
        private List<PlayerPrefStore> _displayedPrefs = new List<PlayerPrefStore>();
        private readonly List<PlayerPrefsSnapshotInfo> _snapshots =
            new List<PlayerPrefsSnapshotInfo>();
        private readonly HashSet<string> _favoriteKeys =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _customGroups =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _keyGroups =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<PlayerPrefStore> _selectedPrefs =
            new HashSet<PlayerPrefStore>();
        private string _currentGroup = GroupMain;
        private bool _showingSnapshots;
        private bool _syncingListSelection;
        private bool _mobileActionsFolded = true;
        private int _selectionAnchorIndex = -1;
        private int _pendingSelectionAnchorIndex = -1;
        private PlayerPrefStore _dragStartPref;
        private Vector2 _dragStartPosition;
        private bool _draggingRows;
        private const string DragPrefsGenericDataKey = "Yummy.PlayerPrefs.DragPrefs";
        private readonly VisualElement _root;
        private string _pendingConfirmationKey;
        private float _pendingConfirmationUntil;

        // ─── Per-column filter state ──────────────────────────────────────────

        private string _keyFilter   = "";
        private string _keyFilterSearch = "";
        private string _typeFilter  = "";
        private string _valueFilter = "";
        private string _valueFilterSearch = "";
        private int    _filterRequestVersion;

        private bool IsFilterActive =>
            !string.IsNullOrEmpty(_keyFilter)   ||
            !string.IsNullOrEmpty(_typeFilter)  ||
            !string.IsNullOrEmpty(_valueFilter);

        // ─── UI refs ──────────────────────────────────────────────────────────

        private MultiColumnListView _listView;
        private MultiColumnListView _snapshotsListView;
        private Label               _statusLabel;
        private VisualElement       _statusRow;
        private VisualElement       _mobileQuickActions;
        private Label               _errorBanner;
        private Label               _selectedCountLabel;
        private Button       _deleteSelectedButton;
        private Button       _restoreSelectedButton;
        private Button       _moveSelectedButton;
        private Button       _addGroupButton;
        private ScrollView   _tabsScrollView;
        private VisualElement             _tabsVisualElement;
        private Toggle       _tabSnapshots;
        private Toggle       _tabKeys;
        private VisualElement       _mainToolbar;
        private VisualElement       _filterRow;
        private VisualElement       _rowResizeHandle;
        private VisualElement       _listContainer;
        private VisualElement       _snapshotsPanel;
        private VisualElement       _snapshotsListContainer;
        private TextField           _snapshotNameField;
        private TextField           _filterKeyField;
        private DropdownField       _filterTypeField;
        private TextField           _filterValueField;
        private VisualTreeAsset     _cellTemplatesAsset;
        private VisualElement       _dialogOverlay;
        private bool                _headerSyncRegistered;
        private VisualElement       _filterColKeyCell;
        private VisualElement       _filterColTypeCell;
        private VisualElement       _filterColValueCell;
        private float               _rowHeight = RowHeightDefault;
        private bool                _isPhonePortraitLayout;
        private Column              _selectColumn;
        private Column              _favoriteColumn;
        private Column              _keyColumn;
        private Column              _typeColumn;
        private Column              _valueColumn;
        private Column              _editColumn;
        private Column              _rowActionColumn;
        private Column              _snapshotNameColumn;
        private Column              _snapshotCountColumn;
        private Column              _snapshotSizeColumn;
        private Column              _snapshotCreatedColumn;
        private Column              _snapshotActionsColumn;
        private bool?               _phonePortraitLayoutOverride;
        private bool                _isNarrowPhoneLayout;

        // ─── Services (injected / replaceable) ───────────────────────────────

        private readonly IPlayerPrefsSerializer _serializer = new JsonPlayerPrefsSerializer();
        private readonly IPlayerPrefsRuntimeFetcher _prefsFetcher;

        // ─── Validation state ─────────────────────────────────────────────────

        private readonly HashSet<string> _duplicateKeys =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public PlayerPrefsEditorView(
            VisualElement root,
            VisualTreeAsset uxmlAsset = null,
            VisualTreeAsset cellTemplatesAsset = null,
            StyleSheet styleSheet = null,
            IPlayerPrefsRuntimeFetcher prefsFetcher = null,
            bool? phonePortraitLayoutOverride = null)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));
            _prefsFetcher = prefsFetcher ?? PlayerPrefsRuntimeFetcherFactory.Create();
            _phonePortraitLayoutOverride = phonePortraitLayoutOverride;

            styleSheet ??= Resources.Load<StyleSheet>(StyleSheetResourcePath);
            uxmlAsset ??= Resources.Load<VisualTreeAsset>(UxmlResourcePath);
            cellTemplatesAsset ??= Resources.Load<VisualTreeAsset>(CellTemplatesResourcePath);

            if (styleSheet != null)
                _root.styleSheets.Add(styleSheet);

            _root.AddToClassList(ClassRoot);

            if (uxmlAsset != null)
                uxmlAsset.CloneTree(_root);

            _cellTemplatesAsset = cellTemplatesAsset;

            ApplyLayoutFallbackStyles();
            LoadPersistentKeySets();
            ConnectUxmlElements();
            BuildListView();
            SetupRowResizeHandle();
            RegisterAdaptiveLayout();
            RegisterKeyboardShortcuts();
            RefreshPlayerPrefs();
        }

        public void Dispose()
        {
            PlayerPrefsEditorMetadata.SetFloat(EditorPrefsRowHeight, _rowHeight);
            SavePersistentKeySets();
        }

        public void RefreshAdaptiveLayout()
        {
            UpdateAdaptiveLayout();
        }

        public void SetPhonePortraitLayoutOverride(bool? enabled)
        {
            if (_phonePortraitLayoutOverride == enabled)
                return;

            _phonePortraitLayoutOverride = enabled;
            UpdateAdaptiveLayout();
        }

        // =====================================================================
        // UXML wiring
        // =====================================================================

        private void ConnectUxmlElements()
        {
            _statusLabel = _root.Q<Label>(NameStatusLabel);
            _statusRow = _root.Q<VisualElement>(NameStatusRow);
            _mobileQuickActions = _root.Q<VisualElement>(NameMobileQuickActions);
            _errorBanner = _root.Q<Label>(NameErrorBanner);
            _selectedCountLabel = _root.Q<Label>(NameSelectedCountLabel);
            // error banner starts hidden via ppe-hidden class in UXML

            _filterColKeyCell  = _root.Q<VisualElement>(null, ClassFilterColKey);
            _filterColTypeCell = _root.Q<VisualElement>(null, ClassFilterColType);
            _filterColValueCell = _root.Q<VisualElement>(null, ClassFilterColValue);
            _mainToolbar = _root.Q<VisualElement>(className: "ppe-toolbar");
            _filterRow = _root.Q<VisualElement>(className: "ppe-filter-row");
            _rowResizeHandle = _root.Q<VisualElement>(NameRowResizeHandle);
            _listContainer = _root.Q<VisualElement>(NameListContainer);
            _snapshotsPanel = _root.Q<VisualElement>(NameSnapshotsPanel);
            _snapshotsListContainer = _root.Q<VisualElement>(NameSnapshotsListContainer);
            _snapshotNameField = _root.Q<TextField>(NameSnapshotNameField);

            _deleteSelectedButton = _root.Q<Button>(NameBtnDeleteSelected);
            _restoreSelectedButton = _root.Q<Button>(NameBtnRestoreSelected);
            _moveSelectedButton = _root.Q<Button>(NameBtnMoveSelected);
            _addGroupButton = _root.Q<Button>(NameBtnAddGroup);
            _tabsScrollView = _root.Q<ScrollView>(NameTabsScrollView);
            _tabsVisualElement = _root.Q<VisualElement>(NameTabsVisualElement);
            _tabSnapshots         = _root.Q<Toggle>(NameTabSnapshots);
            _tabKeys              = _root.Q<Toggle>(NameTabKeys);

            _root.Q<Button>(NameBtnAddNew).clicked    += AddNewPref;
            _root.Q<Button>(NameBtnAddNewMobile).clicked += AddNewPref;
            _restoreSelectedButton.clicked += RestoreSelectedItems;
            _moveSelectedButton.clicked += ShowMoveSelectedMenu;
            _deleteSelectedButton.clicked += DeleteSelectedItems;
            _root.Q<Button>(NameBtnDeleteAll).clicked += DeleteAllPrefsImmediately;
            _root.Q<Button>(NameBtnSave).clicked      += SaveAll;
            _root.Q<Button>(NameBtnRefresh).clicked   += RefreshPlayerPrefs;
            _root.Q<Button>(NameBtnSaveMobile).clicked += SaveAll;
            _root.Q<Button>(NameBtnRefreshMobile).clicked += RefreshPlayerPrefs;
            _root.Q<Button>(NameBtnExport).clicked    += ExportToJson;
            _root.Q<Button>(NameBtnImport).clicked    += ImportFromJson;
            _root.Q<Button>(NameBtnActionsFold).clicked += ToggleMobileActionsFolded;
            _addGroupButton.clicked += BeginAddCustomGroup;
            _root.Q<Button>(NameBtnSaveSnapshot).clicked += SaveSnapshot;
            _root.Q<Button>(NameBtnRefreshSnapshots).clicked += RefreshSnapshots;

            _tabSnapshots.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                    SetSnapshotsTabActive();
                else if (_showingSnapshots)
                    _tabSnapshots.SetValueWithoutNotify(true);
            });

            _tabKeys.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                    SetCurrentGroup(GroupMain);
                else if (IsMainGroup(_currentGroup))
                    _tabKeys.SetValueWithoutNotify(true);
            });
            RegisterGroupDropTarget(_tabKeys, GroupMain);

            _filterKeyField = _root.Q<TextField>(NameFilterKey);
            _filterKeyField.RegisterValueChangedCallback(
                evt =>
                {
                    _keyFilter       = evt.newValue ?? "";
                    _keyFilterSearch = _keyFilter.ToLowerInvariant();
                    RequestApplyFilter();
                });

            _errorBanner.RegisterCallback<PointerDownEvent>(_ => FocusFirstDuplicateKey());

            _filterTypeField = _root.Q<DropdownField>(NameFilterType);
            _filterTypeField.choices = new List<string> { TypeFilterAll };
            _filterTypeField.choices.AddRange(PrefValue.AllTypeDisplayNames);
            _filterTypeField.SetValueWithoutNotify(TypeFilterAll);
            _filterTypeField.RegisterValueChangedCallback(evt =>
            {
                _typeFilter = evt.newValue == TypeFilterAll
                    ? ""
                    : PrefValue.DisplayToTypeId(evt.newValue);
                ApplyFilter();
            });

            _filterValueField = _root.Q<TextField>(NameFilterValue);
            _filterValueField.RegisterValueChangedCallback(
                evt =>
                {
                    _valueFilter       = evt.newValue ?? "";
                    _valueFilterSearch = _valueFilter.ToLowerInvariant();
                    RequestApplyFilter();
                });

            if (_snapshotNameField != null)
                _snapshotNameField.SetValueWithoutNotify(GetDefaultSnapshotName());

            BuildSnapshotsListView();
            RefreshSnapshots();
            UpdateTabLabels();
            RebuildGroupControls();
            SyncTabToggles();
            UpdateSelectedControls();
        }

        private void ApplyLayoutFallbackStyles()
        {
            _root.style.flexDirection = FlexDirection.Column;
            _root.style.flexGrow = 1;

            foreach (var toolbar in _root.Query<VisualElement>(className: "ppe-toolbar").ToList())
                ApplyToolbarFallback(toolbar);

            foreach (var tabs in _root.Query<VisualElement>(className: "ppe-tabs").ToList())
                ApplyToolbarFallback(tabs);

            var tabsScroll = _root.Q<ScrollView>(NameTabsScrollView);
            if (tabsScroll != null)
            {
                tabsScroll.style.flexGrow = 0;
                tabsScroll.style.flexShrink = 0;
                tabsScroll.style.width = Length.Percent(100);
                tabsScroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
                tabsScroll.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            }

            var status = _root.Q<Label>(NameStatusLabel);
            if (status != null)
                status.style.flexShrink = 0;

            var mobileQuickActions = _root.Q<VisualElement>(NameMobileQuickActions);
            if (mobileQuickActions != null)
                mobileQuickActions.style.flexShrink = 0;

            var error = _root.Q<Label>(NameErrorBanner);
            if (error != null)
                error.style.flexShrink = 0;

            var filterRow = _root.Q<VisualElement>(className: "ppe-filter-row");
            if (filterRow != null)
                filterRow.style.flexShrink = 0;

            var resizeHandle = _root.Q<VisualElement>(NameRowResizeHandle);
            if (resizeHandle != null)
                resizeHandle.style.flexShrink = 0;

            var snapshotsPanel = _root.Q<VisualElement>(NameSnapshotsPanel);
            if (snapshotsPanel != null)
            {
                snapshotsPanel.style.flexGrow = 1;
                snapshotsPanel.style.flexShrink = 1;
                snapshotsPanel.style.minHeight = 100;
            }

            var snapshotActions = _root.Q<VisualElement>(className: "ppe-snapshot-actions");
            if (snapshotActions != null)
                snapshotActions.style.flexShrink = 0;

            var snapshotsListContainer = _root.Q<VisualElement>(NameSnapshotsListContainer);
            if (snapshotsListContainer != null)
            {
                snapshotsListContainer.style.flexGrow = 1;
                snapshotsListContainer.style.flexShrink = 1;
                snapshotsListContainer.style.minHeight = 100;
            }

            var listContainer = _root.Q<VisualElement>(NameListContainer);
            if (listContainer != null)
            {
                listContainer.style.flexGrow = 1;
                listContainer.style.flexShrink = 1;
                listContainer.style.minHeight = 100;
            }
        }

        private static void ApplyToolbarFallback(VisualElement toolbar)
        {
            toolbar.style.flexDirection = FlexDirection.Row;
            toolbar.style.alignItems = Align.Center;
            toolbar.style.flexGrow = 0;
            toolbar.style.flexShrink = 0;
            toolbar.style.minHeight = 24;

            foreach (var button in toolbar.Query<Button>().ToList())
            {
                button.style.flexGrow = 0;
                button.style.flexShrink = 0;
                button.style.alignSelf = Align.Center;
            }

            foreach (var spacer in toolbar.Query<VisualElement>(className: "ppe-toolbar-spacer").ToList())
            {
                spacer.style.flexGrow = 1;
                spacer.style.flexShrink = 1;
            }
        }

        private void RegisterAdaptiveLayout()
        {
            _root.RegisterCallback<GeometryChangedEvent>(_ => UpdateAdaptiveLayout());
            _root.schedule.Execute(UpdateAdaptiveLayout).StartingIn(0);
        }

        private void UpdateAdaptiveLayout()
        {
            float width = _root.resolvedStyle.width;
            float height = _root.resolvedStyle.height;
            if (width <= 0f || height <= 0f)
            {
                width = _root.layout.width;
                height = _root.layout.height;
            }

            bool phonePortrait = _phonePortraitLayoutOverride ??
                                 IsPhonePortraitLayout(width, height);
            bool narrowPhone = phonePortrait && IsNarrowPhoneLayout(width);
            bool classMatches = _root.ClassListContains(ClassPhonePortrait) == phonePortrait;
            bool narrowClassMatches = _root.ClassListContains(ClassPhoneNarrow) == narrowPhone;
            if (phonePortrait == _isPhonePortraitLayout &&
                narrowPhone == _isNarrowPhoneLayout &&
                classMatches &&
                narrowClassMatches)
            {
                ApplyAdaptiveToolbarLayout();
                ApplyAdaptiveTabsLayout();
                ApplyAdaptiveFilterLayout();
                ApplyAdaptiveRootOrder();
                ApplyAdaptiveMainOnlyVisibility();
                ApplyAdaptiveRowHeights();
                return;
            }

            _isPhonePortraitLayout = phonePortrait;
            _isNarrowPhoneLayout = narrowPhone;
            _root.EnableInClassList(ClassPhonePortrait, _isPhonePortraitLayout);
            _root.EnableInClassList(ClassPhoneNarrow, _isNarrowPhoneLayout);
            ApplyAdaptiveToolbarLayout();
            ApplyAdaptiveTabsLayout();
            ApplyAdaptiveFilterLayout();
            ApplyAdaptiveRootOrder();
            ApplyAdaptiveMainOnlyVisibility();
            ApplyAdaptiveColumns();
            ApplyAdaptiveRowHeights();
            _listView?.Rebuild();
            _snapshotsListView?.Rebuild();
            _root.schedule.Execute(TryRegisterHeaderSync).StartingIn(0);
        }

        private static bool IsPhonePortraitLayout(float width, float height)
        {
            bool rootPortrait = IsPortraitAspect(width, height, 1f);
            bool rootEditorSimulatorPortrait =
                IsPortraitAspect(width, height, EditorSimulatorPortraitMinAspect);
            bool screenPortrait = IsPortraitAspect(Screen.width, Screen.height, 1f);
            bool screenEditorSimulatorPortrait =
                IsPortraitAspect(Screen.width, Screen.height, EditorSimulatorPortraitMinAspect);
            Rect safeArea = Screen.safeArea;
            bool safeAreaPortrait = IsPortraitAspect(safeArea.width, safeArea.height, 1f);
            bool safeAreaEditorSimulatorPortrait =
                IsPortraitAspect(safeArea.width, safeArea.height, EditorSimulatorPortraitMinAspect);

            bool rootNarrow = IsNarrowWidth(width);
            bool screenNarrow = IsNarrowWidth(Screen.width);
            bool safeAreaNarrow = IsNarrowWidth(safeArea.width);

            if (Application.isMobilePlatform)
                return rootPortrait || screenPortrait || safeAreaPortrait ||
                       rootNarrow || screenNarrow || safeAreaNarrow;

#if UNITY_EDITOR
            return (rootEditorSimulatorPortrait && rootNarrow) ||
                   (screenEditorSimulatorPortrait && screenNarrow) ||
                   (safeAreaEditorSimulatorPortrait && safeAreaNarrow);
#else
            return false;
#endif
        }

        private static bool IsPortraitAspect(float width, float height, float minAspect)
        {
            return width > 0f && height >= width * minAspect;
        }

        private static bool IsNarrowPhoneLayout(float rootWidth)
        {
            if (IsNarrowWidth(rootWidth))
                return true;

            Rect safeArea = Screen.safeArea;
            return IsNarrowWidth(Screen.width) || IsNarrowWidth(safeArea.width);
        }

        private static bool IsNarrowWidth(float width)
        {
            return width > 0f && width <= NarrowPhoneLayoutMaxWidth;
        }

        private void ApplyAdaptiveToolbarLayout()
        {
            if (_mainToolbar == null)
                return;

            ApplyAdaptiveMobileQuickActionsLayout();
            ApplyAdaptiveToolbarOrder();
            ApplyAdaptiveToolbarTexts();

            _mainToolbar.style.flexDirection = FlexDirection.Row;
            _mainToolbar.style.flexWrap = _isPhonePortraitLayout ? Wrap.Wrap : Wrap.NoWrap;
            _mainToolbar.style.alignItems = _isPhonePortraitLayout ? Align.Stretch : Align.Center;

            foreach (var child in _mainToolbar.Children())
            {
                if (_isPhonePortraitLayout)
                    ApplyPhoneToolbarChildLayout(child);
                else
                    ApplyDesktopToolbarChildLayout(child);
            }
        }

        private void ApplyAdaptiveToolbarOrder()
        {
            if (_mainToolbar == null)
                return;

            var spacer = _mainToolbar.Query<VisualElement>(className: "ppe-toolbar-spacer").First();

            if (_isPhonePortraitLayout)
            {
                foreach (string name in new[]
                {
                    NameBtnActionsFold,
                    NameBtnExport,
                    NameBtnImport,
                    NameBtnDeleteAll,
                    NameBtnMoveSelected,
                    NameBtnRestoreSelected,
                    NameBtnDeleteSelected,
                    NameBtnAddNew,
                })
                {
                    AddToolbarChildToEnd(_mainToolbar.Q<VisualElement>(name));
                }

                AddToolbarChildToEnd(spacer);
                return;
            }

            foreach (string name in new[]
            {
                NameBtnActionsFold,
                NameBtnAddNew,
                NameSelectedCountLabel,
                NameBtnRestoreSelected,
                NameBtnMoveSelected,
                NameBtnDeleteSelected,
            })
            {
                AddToolbarChildToEnd(_mainToolbar.Q<VisualElement>(name));
            }

            AddToolbarChildToEnd(spacer);

            foreach (string name in new[]
            {
                NameBtnDeleteAll,
                NameBtnSave,
                NameBtnRefresh,
                NameBtnExport,
                NameBtnImport,
            })
            {
                AddToolbarChildToEnd(_mainToolbar.Q<VisualElement>(name));
            }
        }

        private void AddToolbarChildToEnd(VisualElement child)
        {
            if (child == null || child.parent != _mainToolbar)
                return;

            _mainToolbar.Add(child);
        }

        private void ApplyAdaptiveToolbarTexts()
        {
            SetButtonText(NameBtnExport, _isPhonePortraitLayout ? "Export" : "Export JSON");
            SetButtonText(NameBtnImport, _isPhonePortraitLayout ? "Import" : "Import JSON");
            SetButtonText(NameBtnAddGroup, _isPhonePortraitLayout ? "+" : "+ Group");
            SetButtonText(NameBtnDeleteAll, "X Delete All");
            SetButtonText(NameBtnRestoreSelected, _isPhonePortraitLayout ? "Restore Selected" : BtnTextRestoreSelected);
            SetButtonText(NameBtnDeleteSelected, BtnTextDeleteSelected);
            SetButtonText(NameBtnActionsFold, _mobileActionsFolded ? "Actions +" : "Actions -");
        }

        private void ApplyAdaptiveMobileQuickActionsLayout()
        {
            if (_mobileQuickActions == null)
                return;

            bool visible = _isPhonePortraitLayout && !_showingSnapshots;
            _mobileQuickActions.EnableInClassList(ClassHidden, !visible);
            _mobileQuickActions.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            if (!visible)
                return;

            _mobileQuickActions.style.flexDirection = FlexDirection.Row;
            _mobileQuickActions.style.flexWrap = Wrap.NoWrap;
            _mobileQuickActions.style.alignItems = Align.Stretch;

            SetMobileQuickButtonLayout(NameBtnAddNewMobile, 44f);
            SetMobileQuickButtonLayout(NameBtnSaveMobile, 27f);
            SetMobileQuickButtonLayout(NameBtnRefreshMobile, 29f);
        }

        private void SetMobileQuickButtonLayout(string name, float widthPercent)
        {
            var button = _mobileQuickActions?.Q<Button>(name);
            if (button == null)
                return;

            button.style.display = DisplayStyle.Flex;
            button.style.width = Length.Percent(widthPercent);
            button.style.minWidth = 0;
            button.style.flexGrow = 1;
            button.style.flexShrink = 1;
        }

        private void ToggleMobileActionsFolded()
        {
            _mobileActionsFolded = !_mobileActionsFolded;
            ApplyAdaptiveToolbarLayout();
        }

        private void SetButtonText(string name, string text)
        {
            var button = _root.Q<Button>(name);
            if (button != null && button.text != text)
                button.text = text;
        }

        private void ApplyPhoneToolbarChildLayout(VisualElement child)
        {
            if (child.ClassListContains("ppe-toolbar-spacer"))
            {
                child.style.display = DisplayStyle.None;
                return;
            }

            if (child.name == NameSelectedCountLabel)
            {
                child.style.display = DisplayStyle.None;
                return;
            }

            if (child.name == NameBtnAddNew)
            {
                child.style.display = DisplayStyle.None;
                return;
            }

            if (child.name == NameBtnSave || child.name == NameBtnRefresh)
            {
                child.style.display = DisplayStyle.None;
                return;
            }

            if (_mobileActionsFolded &&
                child.name != NameBtnActionsFold)
            {
                child.style.display = DisplayStyle.None;
                return;
            }

            child.style.display = DisplayStyle.Flex;
            child.style.flexGrow = 1;
            child.style.flexShrink = 1;
            child.style.alignSelf = Align.Stretch;
            child.style.width = child.name == NameBtnActionsFold
                ? Length.Percent(100f)
                : Length.Percent(47f);

            if (child is Button)
                child.style.minWidth = 0;
            else
                child.style.minWidth = 0;
        }

        private static void ApplyDesktopToolbarChildLayout(VisualElement child)
        {
            if (child.name == NameBtnActionsFold)
            {
                child.style.display = DisplayStyle.None;
                return;
            }

            child.style.display = DisplayStyle.Flex;
            child.style.width = StyleKeyword.Auto;
            child.style.minWidth = StyleKeyword.Null;
            child.style.alignSelf = Align.Center;
            child.style.marginTop = StyleKeyword.Null;
            child.style.marginBottom = StyleKeyword.Null;
            child.style.marginLeft = StyleKeyword.Null;
            child.style.marginRight = StyleKeyword.Null;

            if (child.ClassListContains("ppe-toolbar-spacer"))
            {
                child.style.flexGrow = 1;
                child.style.flexShrink = 1;
                return;
            }

            child.style.flexGrow = 0;
            child.style.flexShrink = 0;
        }

        private void ApplyAdaptiveFilterLayout()
        {
            if (_filterRow == null)
                return;

            if (_isPhonePortraitLayout)
            {
                AddFilterChildToEnd(_filterColKeyCell);
                AddFilterChildToEnd(_filterColTypeCell);
                AddFilterChildToEnd(_filterColValueCell);

                ApplyPhoneFilterCellLayout(_filterColKeyCell, 36f);
                ApplyPhoneFilterCellLayout(_filterColTypeCell, 24f);
                ApplyPhoneFilterCellLayout(_filterColValueCell, 36f);
                return;
            }

            RestoreDesktopFilterOrder();
            ClearPhoneFilterCellLayout(_filterColKeyCell);
            ClearPhoneFilterCellLayout(_filterColValueCell);
            ClearPhoneFilterCellLayout(_filterColTypeCell);
        }

        private void AddFilterChildToEnd(VisualElement child)
        {
            if (child == null || child.parent != _filterRow)
                return;

            _filterRow.Add(child);
        }

        private void RestoreDesktopFilterOrder()
        {
            AddFilterChildToEnd(_filterRow.Q<VisualElement>(null, "ppe-filter-col-select"));
            AddFilterChildToEnd(_filterRow.Q<VisualElement>(null, "ppe-filter-col-favorite"));
            AddFilterChildToEnd(_filterColKeyCell);
            AddFilterChildToEnd(_filterColTypeCell);
            AddFilterChildToEnd(_filterColValueCell);
            AddFilterChildToEnd(_filterRow.Q<VisualElement>(null, "ppe-filter-col-edit"));
            AddFilterChildToEnd(_filterRow.Q<VisualElement>(null, "ppe-filter-col-action"));
        }

        private static void ApplyPhoneFilterCellLayout(VisualElement child, float widthPercent)
        {
            if (child == null)
                return;

            child.style.width = Length.Percent(widthPercent);
            child.style.minWidth = 0;
            child.style.flexGrow = 1;
            child.style.flexShrink = 1;
        }

        private static void ClearPhoneFilterCellLayout(VisualElement child)
        {
            if (child == null)
                return;

            child.style.width = StyleKeyword.Null;
            child.style.minWidth = StyleKeyword.Null;
            child.style.flexGrow = StyleKeyword.Null;
            child.style.flexShrink = StyleKeyword.Null;
        }

        private void ApplyAdaptiveRootOrder()
        {
            if (_statusRow == null || _filterRow == null ||
                _statusRow.parent != _root || _filterRow.parent != _root)
            {
                return;
            }

            _statusRow.RemoveFromHierarchy();
            int filterIndex = _root.IndexOf(_filterRow);
            int targetIndex;
            if (_isPhonePortraitLayout)
            {
                targetIndex = filterIndex + 1;
            }
            else if (_errorBanner != null && _errorBanner.parent == _root)
            {
                targetIndex = Mathf.Max(0, _root.IndexOf(_errorBanner));
            }
            else
            {
                targetIndex = Mathf.Min(_root.childCount, 2);
            }

            _root.Insert(targetIndex, _statusRow);
        }

        private void ApplyAdaptiveTabsLayout()
        {
            ApplyAdaptiveTabsScrollLayout();

            if (_tabsVisualElement == null)
                return;

            _tabsVisualElement.style.width = Length.Percent(100);
            _tabsVisualElement.style.flexGrow = 0;
            _tabsVisualElement.style.flexShrink = 0;

            if (_isPhonePortraitLayout)
            {
                _tabsVisualElement.style.flexDirection = FlexDirection.Row;
                _tabsVisualElement.style.flexWrap = Wrap.NoWrap;
                _tabsVisualElement.style.alignItems = Align.Stretch;
                _tabsVisualElement.style.width = StyleKeyword.Auto;

                foreach (var child in _tabsVisualElement.Children())
                    ApplyPhoneTabChildLayout(child);

                return;
            }

            _tabsVisualElement.style.flexDirection = FlexDirection.Row;
            _tabsVisualElement.style.flexWrap = Wrap.NoWrap;
            _tabsVisualElement.style.alignItems = Align.Center;

            foreach (var child in _tabsVisualElement.Children())
                ApplyDesktopTabChildLayout(child);
        }

        private void ApplyAdaptiveTabsScrollLayout()
        {
            if (_tabsScrollView == null)
                return;

            _tabsScrollView.style.flexGrow = 0;
            _tabsScrollView.style.flexShrink = 0;
            _tabsScrollView.style.width = Length.Percent(100);

            if (_isPhonePortraitLayout)
            {
                _tabsScrollView.mode = ScrollViewMode.Horizontal;
                _tabsScrollView.style.maxHeight = StyleKeyword.Null;
                _tabsScrollView.style.minHeight = 0;
                _tabsScrollView.horizontalScrollerVisibility = ScrollerVisibility.Auto;
                _tabsScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
                _tabsScrollView.contentContainer.style.flexDirection = FlexDirection.Row;
                _tabsScrollView.contentContainer.style.flexGrow = 0;
                _tabsScrollView.contentContainer.style.flexShrink = 0;
                return;
            }

            _tabsScrollView.mode = ScrollViewMode.Vertical;
            _tabsScrollView.style.maxHeight = StyleKeyword.Null;
            _tabsScrollView.style.minHeight = StyleKeyword.Null;
            _tabsScrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            _tabsScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _tabsScrollView.contentContainer.style.flexDirection = FlexDirection.Column;
            _tabsScrollView.contentContainer.style.flexGrow = 0;
            _tabsScrollView.contentContainer.style.flexShrink = 0;
        }

        private static void ApplyPhoneTabChildLayout(VisualElement child)
        {
            if (child.ClassListContains("ppe-toolbar-spacer"))
            {
                child.style.display = DisplayStyle.None;
                return;
            }

            child.style.display = DisplayStyle.Flex;
            child.style.flexGrow = 0;
            child.style.flexShrink = 0;
            child.style.alignSelf = Align.Stretch;
            child.style.width = StyleKeyword.Auto;
            child.style.minWidth = child is Button ? 58 : 150;
            child.style.marginLeft = 4;
            child.style.marginRight = 4;
            child.style.marginTop = 0;
            child.style.marginBottom = 0;

            if (child.ClassListContains("ppe-tab-with-close"))
            {
                child.style.flexDirection = FlexDirection.Row;
                child.style.alignItems = Align.Stretch;

                var toggle = child.Q<Toggle>();
                if (toggle != null)
                {
                    toggle.style.flexGrow = 1;
                    toggle.style.flexShrink = 1;
                    toggle.style.width = StyleKeyword.Auto;
                    toggle.style.minWidth = 136;
                    toggle.style.marginLeft = 0;
                    toggle.style.marginRight = 0;
                    toggle.style.marginTop = 0;
                    toggle.style.marginBottom = 0;
                }

                var close = child.Q<Button>(className: "ppe-tab-close-btn");
                if (close != null)
                {
                    close.style.flexGrow = 0;
                    close.style.flexShrink = 0;
                    close.style.width = 34;
                    close.style.minWidth = 34;
                    close.style.marginLeft = 4;
                }

                return;
            }

            if (child is Toggle directToggle)
            {
                directToggle.style.flexGrow = 0;
                directToggle.style.flexShrink = 0;
                directToggle.style.width = StyleKeyword.Auto;
                directToggle.style.minWidth = 150;
            }

            if (child is Button directButton)
            {
                directButton.style.flexGrow = 0;
                directButton.style.flexShrink = 0;
                directButton.style.width = StyleKeyword.Auto;
                directButton.style.minWidth = 58;
            }
        }

        private static void ApplyDesktopTabChildLayout(VisualElement child)
        {
            child.style.display = DisplayStyle.Flex;
            child.style.width = StyleKeyword.Auto;
            child.style.alignSelf = StyleKeyword.Null;
            child.style.marginTop = StyleKeyword.Null;
            child.style.marginBottom = StyleKeyword.Null;
            child.style.marginLeft = StyleKeyword.Null;
            child.style.marginRight = StyleKeyword.Null;

            if (child.ClassListContains("ppe-toolbar-spacer"))
            {
                child.style.flexGrow = 1;
                child.style.flexShrink = 1;
                return;
            }

            child.style.flexGrow = 0;
            child.style.flexShrink = 0;

            if (child.ClassListContains("ppe-tab-with-close"))
            {
                child.style.flexDirection = FlexDirection.Row;
                child.style.alignItems = Align.Stretch;

                var toggle = child.Q<Toggle>();
                if (toggle != null)
                {
                    toggle.style.flexGrow = StyleKeyword.Null;
                    toggle.style.flexShrink = StyleKeyword.Null;
                    toggle.style.width = StyleKeyword.Null;
                    toggle.style.minWidth = StyleKeyword.Null;
                    toggle.style.marginLeft = StyleKeyword.Null;
                    toggle.style.marginRight = StyleKeyword.Null;
                    toggle.style.marginTop = StyleKeyword.Null;
                    toggle.style.marginBottom = StyleKeyword.Null;
                }

                var close = child.Q<Button>(className: "ppe-tab-close-btn");
                if (close != null)
                {
                    close.style.flexGrow = StyleKeyword.Null;
                    close.style.flexShrink = StyleKeyword.Null;
                    close.style.width = StyleKeyword.Null;
                    close.style.minWidth = StyleKeyword.Null;
                    close.style.marginLeft = StyleKeyword.Null;
                }
            }
        }

        private void ApplyAdaptiveColumns()
        {
            if (_isNarrowPhoneLayout)
            {
                SetColumnWidth(_selectColumn, 24, 24, 24);
                SetColumnWidth(_favoriteColumn, 48, 48, 48);
                SetColumnWidth(_keyColumn, 230, 190, null);
                SetColumnWidth(_typeColumn, 72, 66, 82);
                SetColumnWidth(_valueColumn, 150, 120, null);
                SetColumnWidth(_editColumn, 50, 50, 50);
                SetColumnWidth(_rowActionColumn, 50, 50, 50);

                SetColumnWidth(_snapshotNameColumn, 190, 150, null);
                SetColumnWidth(_snapshotCountColumn, 70, 58, null);
                SetColumnWidth(_snapshotSizeColumn, 78, 68, null);
                SetColumnWidth(_snapshotCreatedColumn, 124, 112, null);
                SetColumnWidth(_snapshotActionsColumn, 116, 116, 116);
            }
            else if (_isPhonePortraitLayout)
            {
                SetColumnWidth(_selectColumn, 28, 28, 28);
                SetColumnWidth(_favoriteColumn, 54, 54, 54);
                SetColumnWidth(_keyColumn, 260, 210, null);
                SetColumnWidth(_typeColumn, 76, 70, 90);
                SetColumnWidth(_valueColumn, 165, 130, null);
                SetColumnWidth(_editColumn, 54, 54, 54);
                SetColumnWidth(_rowActionColumn, 54, 54, 54);

                SetColumnWidth(_snapshotNameColumn, 260, 180, null);
                SetColumnWidth(_snapshotCountColumn, 86, 78, null);
                SetColumnWidth(_snapshotSizeColumn, 96, 86, null);
                SetColumnWidth(_snapshotCreatedColumn, 156, 142, null);
                SetColumnWidth(_snapshotActionsColumn, 148, 148, 148);
            }
            else
            {
                SetColumnWidth(_selectColumn, 28, 28, 28);
                SetColumnWidth(_favoriteColumn, 28, 28, 28);
                SetColumnWidth(_keyColumn, 200, 60, null);
                SetColumnWidth(_typeColumn, 80, 60, null);
                SetColumnWidth(_valueColumn, 200, 60, null);
                SetColumnWidth(_editColumn, 28, 28, 28);
                SetColumnWidth(_rowActionColumn, 28, 28, 28);

                SetColumnWidth(_snapshotNameColumn, 260, 120, null);
                SetColumnWidth(_snapshotCountColumn, 74, 58, null);
                SetColumnWidth(_snapshotSizeColumn, 86, 68, null);
                SetColumnWidth(_snapshotCreatedColumn, 150, 126, null);
                SetColumnWidth(_snapshotActionsColumn, 124, 124, 124);
            }
        }

        private static void SetColumnWidth(Column column, float width, float minWidth, float? maxWidth)
        {
            if (column == null)
                return;

            column.width = width;
            column.minWidth = minWidth;
            if (maxWidth.HasValue)
                column.maxWidth = maxWidth.Value;
        }

        private void ApplyAdaptiveRowHeights()
        {
            if (_listView != null)
                _listView.fixedItemHeight = GetEffectiveRowHeight();
            if (_snapshotsListView != null)
                _snapshotsListView.fixedItemHeight = GetEffectiveSnapshotRowHeight();
        }

        private float GetEffectiveRowHeight() =>
            _isPhonePortraitLayout ? PhoneRowHeight : _rowHeight;

        private float GetEffectiveSnapshotRowHeight() =>
            _isPhonePortraitLayout ? PhoneSnapshotRowHeight : SnapshotRowHeightDefault;

        // =====================================================================
        // Cell template factory
        // =====================================================================

        /// <summary>
        /// Clones <c>PlayerPrefsEditor.Templates.uxml</c> into a temp container,
        /// extracts the named root element and detaches it for use in makeCell.
        /// Called once per virtual row (≈ visible row count).
        /// </summary>
        private VisualElement CloneCellTemplate(string templateName)
        {
            var container = new VisualElement();
            if (_cellTemplatesAsset == null)
                return container;

            _cellTemplatesAsset.CloneTree(container);
            var tpl = container.Q<VisualElement>(templateName);
            if (tpl == null)
                return container;

            tpl.RemoveFromHierarchy();
            return tpl;
        }

        private static void DisableFocusRecursive(VisualElement element)
        {
            if (element == null) return;

            element.focusable = false;
            element.tabIndex = -1;

            foreach (var child in element.Children())
                DisableFocusRecursive(child);
        }

        // =====================================================================
        // Persisted editor-only key sets
        // =====================================================================

        private void LoadPersistentKeySets()
        {
            LoadStringSet(ProjectEditorPrefsKey(EditorPrefsFavorites), _favoriteKeys);
            string groupsKey = ProjectEditorPrefsKey(EditorPrefsGroups);
            string keyGroupsKey = ProjectEditorPrefsKey(EditorPrefsKeyGroups);
            bool hasSavedGroups = PlayerPrefsEditorMetadata.HasString(groupsKey);
            bool hasSavedKeyGroups = PlayerPrefsEditorMetadata.HasString(keyGroupsKey);

            LoadStringSet(groupsKey, _customGroups);
            if (!hasSavedGroups)
                _customGroups.Add(GroupIgnored);

            LoadStringDictionary(keyGroupsKey, _keyGroups);
            foreach (string group in _keyGroups.Values.Where(g => !IsMainGroup(g)).ToList())
                _customGroups.Add(group);

            var legacyIgnored = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            LoadStringSet(ProjectEditorPrefsKey(EditorPrefsIgnored), legacyIgnored);
            if (!hasSavedKeyGroups && legacyIgnored.Count > 0)
            {
                _customGroups.Add(GroupIgnored);
                foreach (string key in legacyIgnored)
                    if (!_keyGroups.ContainsKey(key))
                        _keyGroups[key] = GroupIgnored;
            }
        }

        private void SavePersistentKeySets()
        {
            SaveStringSet(ProjectEditorPrefsKey(EditorPrefsFavorites), _favoriteKeys);
            SaveStringSet(ProjectEditorPrefsKey(EditorPrefsGroups), _customGroups);
            SaveStringDictionary(ProjectEditorPrefsKey(EditorPrefsKeyGroups), _keyGroups);
            SaveStringSet(ProjectEditorPrefsKey(EditorPrefsIgnored),
                new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }

        private void PruneMetadataForCurrentPrefs()
        {
            var keys = new HashSet<string>(
                _prefs
                    .Where(p => p != null && !string.IsNullOrEmpty(p.name))
                    .Select(p => p.name),
                StringComparer.OrdinalIgnoreCase);

            _favoriteKeys.RemoveWhere(key => !keys.Contains(key));

            foreach (string key in _keyGroups.Keys.ToList())
                if (!keys.Contains(key))
                    _keyGroups.Remove(key);
        }

        private static void LoadStringSet(string key, HashSet<string> target)
        {
            target.Clear();
            string data = PlayerPrefsEditorMetadata.GetString(key, "");
            if (string.IsNullOrEmpty(data)) return;

            try
            {
                var values = JsonConvert.DeserializeObject<List<string>>(data);
                if (values == null) return;
                foreach (var value in values)
                    if (!string.IsNullOrEmpty(value))
                        target.Add(value);
            }
            catch
            {
                // Ignore corrupt editor-only metadata; PlayerPrefs data remains untouched.
            }
        }

        private static void SaveStringSet(string key, HashSet<string> source)
        {
            string data = JsonConvert.SerializeObject(
                source.Where(s => !string.IsNullOrEmpty(s)).OrderBy(s => s).ToList());
            PlayerPrefsEditorMetadata.SetString(key, data);
        }

        private static void LoadStringDictionary(string key, Dictionary<string, string> target)
        {
            target.Clear();
            string data = PlayerPrefsEditorMetadata.GetString(key, "");
            if (string.IsNullOrEmpty(data)) return;

            try
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(data);
                if (values == null) return;
                foreach (var pair in values)
                    if (!string.IsNullOrEmpty(pair.Key) && !string.IsNullOrEmpty(pair.Value))
                        target[pair.Key] = pair.Value;
            }
            catch
            {
                // Ignore corrupt editor-only metadata; PlayerPrefs data remains untouched.
            }
        }

        private static void SaveStringDictionary(string key, Dictionary<string, string> source)
        {
            string data = JsonConvert.SerializeObject(
                source
                    .Where(pair => !string.IsNullOrEmpty(pair.Key) && !IsMainGroup(pair.Value))
                    .OrderBy(pair => pair.Key)
                    .ToDictionary(pair => pair.Key, pair => pair.Value));
            PlayerPrefsEditorMetadata.SetString(key, data);
        }

        private static string ProjectEditorPrefsKey(string baseKey) => baseKey;

        private bool IsFavorite(PlayerPrefStore pref) =>
            pref != null && _favoriteKeys.Contains(pref.name);

        private string GetGroup(PlayerPrefStore pref)
        {
            if (pref == null || string.IsNullOrEmpty(pref.name))
                return GroupMain;
            return _keyGroups.TryGetValue(pref.name, out string group) && IsKnownGroup(group)
                ? group
                : GroupMain;
        }

        private static bool IsMainGroup(string group) =>
            string.Equals(group, GroupMain, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrEmpty(group);

        private bool IsDefaultGroup(string group) =>
            IsMainGroup(group);

        private bool IsKnownGroup(string group) =>
            IsMainGroup(group) || _customGroups.Contains(group);

        private List<string> GetAllGroups() =>
            new[] { GroupMain }
                .Concat(_customGroups.OrderBy(g => g, StringComparer.OrdinalIgnoreCase))
                .ToList();

        private void SetGroup(PlayerPrefStore pref, string group)
        {
            if (pref == null || string.IsNullOrEmpty(pref.name)) return;

            group = IsKnownGroup(group) ? group : GroupMain;
            if (IsMainGroup(group))
                _keyGroups.Remove(pref.name);
            else
                _keyGroups[pref.name] = group;
        }

        private void ReplaceTrackedKey(string oldKey, string newKey)
        {
            if (string.Equals(oldKey, newKey, StringComparison.OrdinalIgnoreCase))
                return;

            bool changed = false;
            if (!string.IsNullOrEmpty(oldKey) && _favoriteKeys.Remove(oldKey))
            {
                if (!string.IsNullOrEmpty(newKey)) _favoriteKeys.Add(newKey);
                changed = true;
            }
            if (!string.IsNullOrEmpty(oldKey) && _keyGroups.TryGetValue(oldKey, out string group))
            {
                _keyGroups.Remove(oldKey);
                if (!string.IsNullOrEmpty(newKey) && !IsMainGroup(group)) _keyGroups[newKey] = group;
                changed = true;
            }

            if (changed) SavePersistentKeySets();
        }

        // =====================================================================
        // Status bar
        // =====================================================================

        private void UpdateStatus()
        {
            if (_statusLabel == null) return;

            if (_showingSnapshots)
            {
                UpdateSnapshotsStatus();
                return;
            }

            int total   = 0;
            int shown   = _displayedPrefs.Count;
            int newCnt  = 0;
            int delCnt  = 0;
            int editCnt = 0;

            for (int i = 0; i < _prefs.Count; i++)
            {
                var pref = _prefs[i];
                if (!IsInCurrentTab(pref)) continue;

                total++;
                if (pref.isNew) newCnt++;
                if (pref.isMarkedForDelete) delCnt++;
                if (pref.Changed) editCnt++;
            }

            string countPart = IsFilterActive
                ? string.Format(StatusCountFmt, shown, total)
                : string.Format(StatusTotalFmt, total);

            string extra = "";
            if (newCnt  > 0) extra += StatusSeparator + string.Format(StatusNewFmt,     newCnt);
            if (editCnt > 0) extra += StatusSeparator + string.Format(StatusEditedFmt,  editCnt);
            if (delCnt  > 0) extra += StatusSeparator + string.Format(StatusDeletedFmt, delCnt);

            _statusLabel.text = countPart + GetMobileSelectedStatusPart() + extra;
            UpdateTabLabels();
            UpdateSelectedControls(false);
        }

        private void UpdateSnapshotsStatus()
        {
            if (_statusLabel != null)
            {
                string count = _snapshots.Count == 0
                    ? MsgSnapshotsEmpty
                    : $"{_snapshots.Count} snapshots";
                _statusLabel.text = count;
            }

            UpdateTabLabels();
            UpdateSelectedControls(false);
        }

        private string GetMobileSelectedStatusPart()
        {
            return _isPhonePortraitLayout
                ? StatusSeparator + string.Format(SelectedCountFmt, _selectedPrefs.Count)
                : "";
        }

        private bool IsInCurrentTab(PlayerPrefStore pref) =>
            string.Equals(GetGroup(pref), _currentGroup, StringComparison.OrdinalIgnoreCase);

        private void SetSnapshotsTabActive()
        {
            if (_showingSnapshots)
            {
                SyncTabToggles();
                return;
            }

            _showingSnapshots = true;
            ClearSelectedPrefs();
            SyncTabToggles();
            RefreshSnapshots();
        }

        private void SetStatusMessage(string message)
        {
            if (_statusLabel != null)
                _statusLabel.text = message ?? "";
            Debug.Log(message);
        }

        private void SetCurrentGroup(string group)
        {
            if (!IsKnownGroup(group))
                group = GroupMain;

            if (string.Equals(_currentGroup, group, StringComparison.OrdinalIgnoreCase))
            {
                bool wasShowingSnapshots = _showingSnapshots;
                _showingSnapshots = false;
                SyncTabToggles();
                if (wasShowingSnapshots)
                    ApplyFilter();
                return;
            }

            _currentGroup = group;
            _showingSnapshots = false;
            ClearSelectedPrefs();
            SyncTabToggles();
            ApplyFilter();
        }

        private void SyncTabToggles()
        {
            _tabSnapshots?.SetValueWithoutNotify(_showingSnapshots);
            _tabSnapshots?.EnableInClassList(ClassTabActive, _showingSnapshots);

            bool mainActive = !_showingSnapshots && IsMainGroup(_currentGroup);
            _tabKeys?.SetValueWithoutNotify(mainActive);
            _tabKeys?.EnableInClassList(ClassTabActive, mainActive);

            SetMainUiVisible(!_showingSnapshots);
            UpdateCustomGroupTabs();
        }

        private void UpdateTabLabels()
        {
            if (_tabSnapshots != null)
                _tabSnapshots.label = $"{TabTextSnapshots} ({_snapshots.Count})";
            if (_tabKeys != null)
                _tabKeys.label = $"{TabTextKeys} ({_prefs.Count(p => IsMainGroup(GetGroup(p)))})";
            UpdateCustomGroupTabs();
        }

        private void SetMainUiVisible(bool visible)
        {
            _mainToolbar?.EnableInClassList(ClassHidden, !visible);
            _filterRow?.EnableInClassList(ClassHidden, !visible);
            _rowResizeHandle?.EnableInClassList(ClassHidden, !visible);
            _listContainer?.EnableInClassList(ClassHidden, !visible);
            _snapshotsPanel?.EnableInClassList(ClassHidden, visible);
            ApplyAdaptiveMainOnlyVisibility();
        }

        private void ApplyAdaptiveMainOnlyVisibility()
        {
            bool mainVisible = !_showingSnapshots;

            if (_mobileQuickActions != null)
            {
                bool showQuickActions = _isPhonePortraitLayout && mainVisible;
                _mobileQuickActions.EnableInClassList(ClassHidden, !showQuickActions);
                _mobileQuickActions.style.display =
                    showQuickActions ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (_statusRow != null)
                _statusRow.EnableInClassList(ClassHidden, _isPhonePortraitLayout && !mainVisible);
        }

        private void RebuildGroupControls()
        {
            if (_tabsVisualElement == null) return;

            var oldCustomTabs = _tabsVisualElement.Children()
                .Where(e => e.userData is string)
                .ToList();
            foreach (var tab in oldCustomTabs)
                tab.RemoveFromHierarchy();

            int insertIndex = _addGroupButton == null
                ? _tabsVisualElement.childCount
                : _tabsVisualElement.IndexOf(_addGroupButton);

            foreach (string group in _customGroups.OrderBy(g => g, StringComparer.OrdinalIgnoreCase))
            {
                var tab = new VisualElement { userData = group };
                tab.AddToClassList("ppe-tab-with-close");

                var toggle = new Toggle
                {
                    label = GetGroupTabText(group),
                    userData = group,
                };
                toggle.AddToClassList("ppe-tab");
                toggle.RegisterValueChangedCallback(evt =>
                {
                    if (toggle.userData is not string groupName) return;
                    if (evt.newValue)
                        SetCurrentGroup(groupName);
                    else if (string.Equals(_currentGroup, groupName, StringComparison.OrdinalIgnoreCase))
                        toggle.SetValueWithoutNotify(true);
                });
                tab.Add(toggle);
                RegisterGroupDropTarget(tab, group);

                var close = new Button(() => DeleteGroup(group))
                {
                    text = "x",
                    tooltip = string.Format(DialogTitleDeleteGroup + ": {0}", group),
                };
                close.AddToClassList("ppe-tab-close-btn");
                tab.Add(close);

                _tabsVisualElement.Insert(insertIndex++, tab);
            }

            SyncTabToggles();
            ApplyAdaptiveTabsLayout();
        }

        private void UpdateCustomGroupTabs()
        {
            if (_tabsVisualElement == null) return;

            foreach (var tabContainer in _tabsVisualElement.Children().Where(e => e.userData is string))
            {
                if (tabContainer.userData is not string group) continue;
                bool active = !_showingSnapshots &&
                              string.Equals(_currentGroup, group, StringComparison.OrdinalIgnoreCase);
                var tab = tabContainer.Q<Toggle>();
                if (tab == null) continue;
                tab.label = GetGroupTabText(group);
                tab.SetValueWithoutNotify(active);
                tab.EnableInClassList(ClassTabActive, active);
            }
        }

        private string GetGroupTabText(string group) =>
            $"{group} ({_prefs.Count(p => string.Equals(GetGroup(p), group, StringComparison.OrdinalIgnoreCase))})";

        private void RegisterGroupDropTarget(VisualElement target, string group)
        {
#if UNITY_EDITOR
            target.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                if (DragAndDrop.GetGenericData(DragPrefsGenericDataKey) is not List<PlayerPrefStore>)
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                evt.StopPropagation();
            });

            target.RegisterCallback<DragPerformEvent>(evt =>
            {
                if (DragAndDrop.GetGenericData(DragPrefsGenericDataKey) is not List<PlayerPrefStore> prefs)
                    return;

                DragAndDrop.AcceptDrag();
                MovePrefsToGroup(prefs, group);
                DragAndDrop.SetGenericData(DragPrefsGenericDataKey, null);
                _draggingRows = false;
                evt.StopPropagation();
            });
#endif
        }

        private void UpdateSelectedControls(bool refreshStatus = true)
        {
            int selectedCount = _selectedPrefs.Count;
            bool hasSelected = selectedCount > 0;

            if (_selectedCountLabel != null)
                _selectedCountLabel.text = string.Format(SelectedCountFmt, selectedCount);

            if (_restoreSelectedButton != null)
            {
                _restoreSelectedButton.text = _isPhonePortraitLayout
                    ? "Restore Selected"
                    : BtnTextRestoreSelected;
                _restoreSelectedButton.SetEnabled(_selectedPrefs.Any(CanRestoreSelected));
            }

            if (_moveSelectedButton != null)
            {
                _moveSelectedButton.text = BtnTextMoveSelected;
                _moveSelectedButton.SetEnabled(hasSelected);
            }

            if (_deleteSelectedButton != null)
            {
                _deleteSelectedButton.text = BtnTextDeleteSelected;
                _deleteSelectedButton.SetEnabled(hasSelected);
            }

            RefreshSelectionStyles();

            if (refreshStatus && _isPhonePortraitLayout && !_showingSnapshots)
                UpdateStatus();
        }

        // =====================================================================
        // Error banner  (duplicate-key alerts)
        // =====================================================================

        private bool ValidateDuplicates()
        {
            _duplicateKeys.Clear();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var p in _prefs)
            {
                if (p.isMarkedForDelete || string.IsNullOrEmpty(p.name)) continue;
                if (!seen.Add(p.name))
                    _duplicateKeys.Add(p.name);
            }

            bool hasDups = _duplicateKeys.Count > 0;

            if (_errorBanner != null)
            {
                _errorBanner.EnableInClassList(ClassHidden, !hasDups);
                if (hasDups)
                {
                    string keys = string.Join(", ",
                        _duplicateKeys.OrderBy(k => k).Select(k => $"\"{k}\""));
                    _errorBanner.text = string.Format(MsgDuplicateKeys, keys);
                }
            }

            return hasDups;
        }

        private void FocusFirstDuplicateKey()
        {
            string duplicateKey = _duplicateKeys
                .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(duplicateKey))
                return;

            var target = _prefs.FirstOrDefault(p =>
                !p.isMarkedForDelete &&
                string.Equals(p.name, duplicateKey, StringComparison.OrdinalIgnoreCase));
            if (target == null)
                return;

            ClearFiltersWithoutNotify();
            SetCurrentGroup(GetGroup(target));
            ApplyFilter();
            FocusPrefInList(target);
        }

        private void ClearFiltersWithoutNotify()
        {
            _keyFilter = "";
            _keyFilterSearch = "";
            _typeFilter = "";
            _valueFilter = "";
            _valueFilterSearch = "";

            _filterKeyField?.SetValueWithoutNotify("");
            _filterTypeField?.SetValueWithoutNotify(TypeFilterAll);
            _filterValueField?.SetValueWithoutNotify("");
        }

        private void FocusPrefInList(PlayerPrefStore pref)
        {
            if (_listView == null || pref == null)
                return;

            int index = _displayedPrefs.IndexOf(pref);
            if (index < 0)
                return;

            _selectedPrefs.Clear();
            _selectedPrefs.Add(pref);
            _selectionAnchorIndex = index;
            _syncingListSelection = true;
            _listView.SetSelection(index);
            _syncingListSelection = false;
            _listView.ScrollToItem(index);
            _listView.Focus();
            UpdateSelectedControls();
        }

        private void FocusPrefKeyField(PlayerPrefStore pref)
        {
            if (_listView == null || pref == null)
                return;

            FocusPrefInList(pref);
            ScheduleFocusPrefKeyField(pref, 0);
        }

        private void ScheduleFocusPrefKeyField(PlayerPrefStore pref, int attempt)
        {
            if (_listView == null || pref == null)
                return;

            _listView.schedule.Execute(() =>
            {
                if (_listView == null || pref == null)
                    return;

                int index = _displayedPrefs.IndexOf(pref);
                if (index < 0)
                    return;

                var field = FindVisibleKeyField(pref);
                if (field != null)
                {
                    FocusAndSelectText(field);
                    field.schedule.Execute(() => FocusAndSelectText(field)).StartingIn(0);
                    return;
                }

                if (attempt >= FocusKeyFieldMaxAttempts)
                    return;

                _listView.ScrollToItem(index);
                ScheduleFocusPrefKeyField(pref, attempt + 1);
            }).StartingIn(attempt == 0 ? 0 : FocusKeyFieldRetryDelayMs);
        }

        private TextField FindVisibleKeyField(PlayerPrefStore pref)
        {
            if (_listView == null || pref == null)
                return null;

            foreach (var field in _listView.Query<TextField>(NameKeyField).ToList())
            {
                if (ReferenceEquals(field.userData, pref))
                    return field;
            }

            return null;
        }

        private static void FocusAndSelectText(TextField field)
        {
            if (field == null)
                return;

            field.Focus();
            var selectAll = field.GetType().GetMethod(
                "SelectAll",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null);
            selectAll?.Invoke(field, null);
        }

        // =====================================================================
        // Filter / Search
        // =====================================================================

        private void ApplyFilter()
        {
            _filterRequestVersion++;
            _displayedPrefs.Clear();
            _selectedPrefs.RemoveWhere(p => p == null || !IsInCurrentTab(p));

            if (!IsFilterActive)
            {
                for (int i = 0; i < _prefs.Count; i++)
                {
                    var pref = _prefs[i];
                    if (IsInCurrentTab(pref))
                        _displayedPrefs.Add(pref);
                }
            }
            else
            {
                for (int i = 0; i < _prefs.Count; i++)
                {
                    var pref = _prefs[i];
                    if (IsInCurrentTab(pref) && MatchesAllFilters(pref))
                        _displayedPrefs.Add(pref);
                }
            }

            if (_listView != null)
            {
                _listView.itemsSource = _displayedPrefs;
                _listView.Rebuild();
                SyncListSelectionToSelectedPrefs();
            }

            UpdateStatus();
        }

        private void SyncListSelectionToSelectedPrefs()
        {
            if (_listView == null) return;

            _syncingListSelection = true;
            _listView.ClearSelection();
            for (int i = 0; i < _displayedPrefs.Count; i++)
                if (_selectedPrefs.Contains(_displayedPrefs[i]))
                    _listView.AddToSelection(i);
            _syncingListSelection = false;
        }

        private void RequestApplyFilter()
        {
            int version = ++_filterRequestVersion;
            _root.schedule
                .Execute(() =>
                {
                    if (version == _filterRequestVersion)
                        ApplyFilter();
                })
                .StartingIn(FilterDebounceMs);
        }

        private bool MatchesAllFilters(PlayerPrefStore pref)
        {
            if (!string.IsNullOrEmpty(_keyFilterSearch) &&
                pref.SearchName.IndexOf(_keyFilterSearch, StringComparison.Ordinal) < 0)
                return false;

            if (!string.IsNullOrEmpty(_typeFilter) &&
                !string.Equals(pref.value.TypeId, _typeFilter,
                    StringComparison.OrdinalIgnoreCase))
                return false;

            if (!string.IsNullOrEmpty(_valueFilterSearch) &&
                pref.SearchValue.IndexOf(_valueFilterSearch, StringComparison.Ordinal) < 0)
                return false;

            return true;
        }

        // =====================================================================
        // Snapshot list
        // =====================================================================

        private void BuildSnapshotsListView()
        {
            if (_snapshotsListContainer == null)
                return;

            var columns = new Columns();

            var nameCol = new Column
            {
                name = ColSnapshotName,
                title = ColTitleSnapshotName,
                width = 260,
                minWidth = 120,
                sortable = true,
                resizable = true,
                stretchable = true,
            };
            _snapshotNameColumn = nameCol;
            nameCol.makeCell = () =>
            {
                var cell = MakeSnapshotTextCell(NameSnapshotRowName, "ppe-snapshot-name");
                var field = cell.Q<TextField>(NameSnapshotRowName);
                RegisterSnapshotNameTap(cell, field);
                return cell;
            };
            nameCol.bindCell = (element, index) =>
            {
                var snapshot = GetSnapshotAt(index);
                var field = element.Q<TextField>(NameSnapshotRowName);
                if (field == null) return;
                if (snapshot == null)
                {
                    field.SetValueWithoutNotify("");
                    field.tooltip = "";
                    field.userData = null;
                    field.isReadOnly = true;
                    return;
                }
                field.userData = snapshot;
                field.isReadOnly = !_isPhonePortraitLayout;
                field.SetValueWithoutNotify(snapshot.DisplayName);
                field.tooltip = snapshot.FilePath;
            };
            nameCol.unbindCell = (element, _) =>
            {
                var field = element.Q<TextField>(NameSnapshotRowName);
                if (field != null)
                {
                    field.userData = null;
                    field.isReadOnly = true;
                }
            };
            columns.Add(nameCol);

            var countCol = new Column
            {
                name = ColSnapshotCount,
                title = ColTitleSnapshotCount,
                width = 74,
                minWidth = 58,
                sortable = true,
                resizable = true,
            };
            _snapshotCountColumn = countCol;
            countCol.makeCell = () => MakeSnapshotTextCell(NameSnapshotRowCount, "ppe-snapshot-count");
            countCol.bindCell = (element, index) =>
            {
                var snapshot = GetSnapshotAt(index);
                var field = element.Q<TextField>(NameSnapshotRowCount);
                if (field == null) return;
                if (snapshot == null)
                {
                    field.SetValueWithoutNotify("");
                    field.userData = null;
                    field.isReadOnly = true;
                    return;
                }
                field.userData = snapshot;
                field.isReadOnly = !_isPhonePortraitLayout;
                field.SetValueWithoutNotify(snapshot.RowCount.ToString(CultureInfo.InvariantCulture));
            };
            countCol.unbindCell = (element, _) => ClearSnapshotTextCellUserData(element, NameSnapshotRowCount);
            columns.Add(countCol);

            var sizeCol = new Column
            {
                name = ColSnapshotSize,
                title = ColTitleSnapshotSize,
                width = 86,
                minWidth = 68,
                sortable = true,
                resizable = true,
            };
            _snapshotSizeColumn = sizeCol;
            sizeCol.makeCell = () => MakeSnapshotTextCell(NameSnapshotRowSize, "ppe-snapshot-size");
            sizeCol.bindCell = (element, index) =>
            {
                var snapshot = GetSnapshotAt(index);
                var field = element.Q<TextField>(NameSnapshotRowSize);
                if (field == null) return;
                if (snapshot == null)
                {
                    field.SetValueWithoutNotify("");
                    field.userData = null;
                    field.isReadOnly = true;
                    return;
                }
                field.userData = snapshot;
                field.isReadOnly = !_isPhonePortraitLayout;
                field.SetValueWithoutNotify(FormatFileSize(snapshot.SizeBytes));
            };
            sizeCol.unbindCell = (element, _) => ClearSnapshotTextCellUserData(element, NameSnapshotRowSize);
            columns.Add(sizeCol);

            var createdCol = new Column
            {
                name = ColSnapshotCreated,
                title = ColTitleSnapshotCreated,
                width = 150,
                minWidth = 126,
                sortable = true,
                resizable = true,
            };
            _snapshotCreatedColumn = createdCol;
            createdCol.makeCell = () => MakeSnapshotTextCell(NameSnapshotRowCreated, "ppe-snapshot-created");
            createdCol.bindCell = (element, index) =>
            {
                var snapshot = GetSnapshotAt(index);
                var field = element.Q<TextField>(NameSnapshotRowCreated);
                if (field == null) return;
                if (snapshot == null)
                {
                    field.SetValueWithoutNotify("");
                    field.userData = null;
                    field.isReadOnly = true;
                    return;
                }
                field.userData = snapshot;
                field.isReadOnly = !_isPhonePortraitLayout;
                field.SetValueWithoutNotify(snapshot.CreatedLocal.ToString(
                    SnapshotDisplayDateFormat,
                    CultureInfo.InvariantCulture));
            };
            createdCol.unbindCell = (element, _) => ClearSnapshotTextCellUserData(element, NameSnapshotRowCreated);
            columns.Add(createdCol);

            var actionsCol = new Column
            {
                name = ColSnapshotActions,
                title = "",
                width = 124,
                minWidth = 124,
                maxWidth = 124,
                sortable = false,
                resizable = false,
            };
            _snapshotActionsColumn = actionsCol;
            actionsCol.makeCell = MakeSnapshotActionsCell;
            actionsCol.bindCell = BindSnapshotActionsCell;
            actionsCol.unbindCell = (element, _) =>
            {
                var load = element.Q<Button>(NameSnapshotRowLoad);
                var delete = element.Q<Button>(NameSnapshotRowDelete);
                if (load != null) load.userData = null;
                if (delete != null) delete.userData = null;
            };
            columns.Add(actionsCol);

            _snapshotsListView = new MultiColumnListView(columns)
            {
                itemsSource = _snapshots,
                fixedItemHeight = GetEffectiveSnapshotRowHeight(),
                selectionType = SelectionType.None,
                sortingMode = ColumnSortingMode.Custom,
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight,
                showAlternatingRowBackgrounds = AlternatingRowBackground.None,
            };
            _snapshotsListView.AddToClassList("ppe-snapshots-list");
            _snapshotsListView.columnSortingChanged += OnSnapshotColumnSortingChanged;
            LockColumnConfiguration(columns);
            _snapshotsListContainer.Add(_snapshotsListView);
        }

        private VisualElement MakeSnapshotTextCell(string name, string className)
        {
            var cell = new VisualElement();
            cell.AddToClassList("ppe-snapshot-cell");
            var field = new TextField { name = name, isReadOnly = true };
            field.AddToClassList("ppe-snapshot-field");
            field.AddToClassList(className);
            field.RegisterValueChangedCallback(_ => RestoreSnapshotTextFieldValue(field));
            cell.Add(field);
            return cell;
        }

        private void RegisterSnapshotNameTap(VisualElement cell, TextField field)
        {
            if (cell == null || field == null)
                return;

            cell.RegisterCallback<PointerDownEvent>(_ =>
            {
                if (field.userData is PlayerPrefsSnapshotInfo snapshot)
                    _snapshotNameField?.SetValueWithoutNotify(snapshot.DisplayName);
            }, TrickleDown.TrickleDown);
        }

        private void RestoreSnapshotTextFieldValue(TextField field)
        {
            if (field?.userData is not PlayerPrefsSnapshotInfo snapshot)
                return;

            string value = field.name switch
            {
                NameSnapshotRowName => snapshot.DisplayName,
                NameSnapshotRowCount => snapshot.RowCount.ToString(CultureInfo.InvariantCulture),
                NameSnapshotRowSize => FormatFileSize(snapshot.SizeBytes),
                NameSnapshotRowCreated => snapshot.CreatedLocal.ToString(
                    SnapshotDisplayDateFormat,
                    CultureInfo.InvariantCulture),
                _ => field.value,
            };

            if (!string.Equals(field.value, value, StringComparison.Ordinal))
                field.SetValueWithoutNotify(value);
        }

        private static void ClearSnapshotTextCellUserData(VisualElement element, string fieldName)
        {
            var field = element.Q<TextField>(fieldName);
            if (field == null)
                return;

            field.userData = null;
            field.isReadOnly = true;
        }

        private VisualElement MakeSnapshotActionsCell()
        {
            var cell = new VisualElement();
            cell.AddToClassList("ppe-snapshot-action-cell");
            var load = new Button
            {
                name = NameSnapshotRowLoad,
                text = DialogBtnLoad,
                tooltip = DialogTitleSnapshotLoad,
            };
            load.AddToClassList("ppe-snapshot-action");
            load.clicked += () =>
            {
                if (load.userData is PlayerPrefsSnapshotInfo snapshot)
                    LoadSnapshot(snapshot);
            };
            cell.Add(load);

            var delete = new Button
            {
                name = NameSnapshotRowDelete,
                text = DialogBtnDelete,
                tooltip = DialogTitleSnapshotDelete,
            };
            delete.AddToClassList("ppe-snapshot-action");
            delete.AddToClassList("ppe-snapshot-delete");
            delete.clicked += () =>
            {
                if (delete.userData is PlayerPrefsSnapshotInfo snapshot)
                    DeleteSnapshot(snapshot);
            };
            cell.Add(delete);
            return cell;
        }

        private void BindSnapshotActionsCell(VisualElement element, int index)
        {
            var snapshot = GetSnapshotAt(index);
            var load = element.Q<Button>(NameSnapshotRowLoad);
            var delete = element.Q<Button>(NameSnapshotRowDelete);
            if (load != null) load.userData = snapshot;
            if (delete != null) delete.userData = snapshot;
        }

        private PlayerPrefsSnapshotInfo GetSnapshotAt(int index) =>
            index >= 0 && index < _snapshots.Count ? _snapshots[index] : null;

        private void OnSnapshotColumnSortingChanged()
        {
            SortSnapshots();
            _snapshotsListView?.Rebuild();
        }

        private void SortSnapshots()
        {
            var descs = _snapshotsListView?.sortedColumns?.ToList()
                        ?? new List<SortColumnDescription>();

            if (descs.Count == 0)
            {
                SortSnapshotsByCreatedDescending();
                return;
            }

            var primary = descs[0];
            int Compare(PlayerPrefsSnapshotInfo a, PlayerPrefsSnapshotInfo b)
            {
                int cmp = primary.columnName switch
                {
                    ColSnapshotCount => a.RowCount.CompareTo(b.RowCount),
                    ColSnapshotSize => a.SizeBytes.CompareTo(b.SizeBytes),
                    ColSnapshotCreated => a.CreatedUtc.CompareTo(b.CreatedUtc),
                    _ => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase),
                };
                return primary.direction == SortDirection.Descending ? -cmp : cmp;
            }

            _snapshots.Sort(Compare);
        }

        private static void SortSnapshotsByCreatedDescending(List<PlayerPrefsSnapshotInfo> snapshots)
        {
            snapshots.Sort((a, b) =>
            {
                int dateCompare = b.CreatedUtc.CompareTo(a.CreatedUtc);
                return dateCompare != 0
                    ? dateCompare
                    : string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase);
            });
        }

        private void SortSnapshotsByCreatedDescending()
        {
            SortSnapshotsByCreatedDescending(_snapshots);
        }

        // =====================================================================
        // List View  (MultiColumnListView stays in C# — cell callbacks need code)
        // =====================================================================

        private void BuildListView()
        {
            var columns = new Columns();

            // ── Selection helper ─────────────────────────────────────────────
            var selectCol = new Column
            {
                name = ColSelect, title = "",
                width = 28, minWidth = 28, maxWidth = 28,
                sortable = false, resizable = false,
            };
            _selectColumn = selectCol;
            selectCol.makeHeader = () =>
            {
                var header = new VisualElement();
                header.AddToClassList("ppe-select-header");
                RegisterSelectAllHeader(header);
                return header;
            };
            selectCol.makeCell = () =>
            {
                var cell = CloneCellTemplate(TplCellSelect);
                DisableFocusRecursive(cell);
                RegisterRowContextMenu(cell);
                return cell;
            };
            selectCol.bindCell = (element, index) =>
            {
                element.userData = _displayedPrefs[index];
                ApplyRowStyle(element, _displayedPrefs[index], index);
            };
            selectCol.unbindCell = (element, _) => element.userData = null;
            columns.Add(selectCol);

            // ── Favorite ─────────────────────────────────────────────────────
            var favoriteCol = new Column
            {
                name = ColFavorite, title = BtnTextFavorite,
                width = 28, minWidth = 28, maxWidth = 28,
                sortable = true, resizable = false,
            };
            _favoriteColumn = favoriteCol;
            favoriteCol.makeCell = () =>
            {
                var cell = CloneCellTemplate(TplCellFavorite);
                var btn = cell.Q<Button>(NameFavBtn);
                DisableFocusRecursive(cell);
                RegisterRowContextMenu(cell);
                btn.clicked += () =>
                {
                    if (btn.userData is not PlayerPrefStore pref) return;
                    ToggleFavorite(pref);
                };
                return cell;
            };
            favoriteCol.bindCell = (element, index) =>
            {
                var pref = _displayedPrefs[index];
                var btn = element.Q<Button>(NameFavBtn);
                bool isFavorite = IsFavorite(pref);
                btn.userData = pref;
                btn.text = isFavorite ? BtnTextFavorite : BtnTextNotFavorite;
                btn.tooltip = isFavorite ? TooltipFavorite : TooltipNotFavorite;
                btn.SetEnabled(!pref.isMarkedForDelete);
                element.userData = pref;
                ApplyRowStyle(element, pref, index);
            };
            favoriteCol.unbindCell = (element, _) =>
            {
                var btn = element.Q<Button>(NameFavBtn);
                if (btn != null) btn.userData = null;
            };
            columns.Add(favoriteCol);

            // ── Key ──────────────────────────────────────────────────────────
            var keyCol = new Column
            {
                name = ColKey, title = ColTitleKey,
                width = 200, minWidth = 60, sortable = true, resizable = true,
            };
            _keyColumn = keyCol;
            keyCol.makeCell = () =>
            {
                var cell = CloneCellTemplate(TplCellKey);
                var tf   = cell.Q<TextField>(NameKeyField);
                RegisterRowContextMenu(cell);
                tf.RegisterValueChangedCallback(evt =>
                {
                    if (tf.userData is not PlayerPrefStore pref) return;
                    if (!pref.isNew)
                    {
                        if (!string.Equals(evt.newValue, pref.name, StringComparison.Ordinal))
                            tf.SetValueWithoutNotify(pref.name);
                        return;
                    }

                    ReplaceTrackedKey(pref.name, evt.newValue);
                    pref.name = evt.newValue;
                    ValidateDuplicates();
                    SortPrefs();
                    ApplyFilter();
                    UpdateStatus();
                });
                return cell;
            };
            keyCol.bindCell = (element, index) =>
            {
                var pref    = _displayedPrefs[index];
                var tf      = element.Q<TextField>(NameKeyField);
                var dupIcon = element.Q<Label>(NameDupIcon);

                tf.userData = pref;
                element.userData = pref;
                tf.SetValueWithoutNotify(pref.name);
                tf.isReadOnly = !pref.isNew && !_isPhonePortraitLayout;
                tf.SetEnabled(!pref.isMarkedForDelete);
                tf.EnableInClassList(ClassFieldReadonly, !pref.isNew);

                bool isDup = !pref.isMarkedForDelete && _duplicateKeys.Contains(pref.name);
                dupIcon.EnableInClassList(ClassHidden, !isDup);
                dupIcon.tooltip = isDup
                    ? string.Format(TooltipDuplicateKey, pref.name)
                    : "";

                ApplyRowStyle(element, pref, index);
            };
            keyCol.unbindCell = (element, _) =>
            {
                var tf = element.Q<TextField>(NameKeyField);
                if (tf != null) tf.userData = null;
            };
            columns.Add(keyCol);

            // ── Type ─────────────────────────────────────────────────────────
            var typeCol = new Column
            {
                name = ColType, title = ColTitleType,
                width = 80, minWidth = 60, sortable = true, resizable = true,
            };
            _typeColumn = typeCol;
            typeCol.makeCell = () =>
            {
                var cell = CloneCellTemplate(TplCellType);
                var drop = cell.Q<DropdownField>(NameTypeField);
                RegisterRowContextMenu(cell);
                drop.choices = new List<string>(PrefValue.AllTypeDisplayNames);
                drop.RegisterValueChangedCallback(evt =>
                {
                    if (drop.userData is not PlayerPrefStore pref) return;
                    string targetId = PrefValue.DisplayToTypeId(evt.newValue);
                    if (pref.value.TypeId == targetId) return;
                    pref.value = pref.value.ConvertTo(targetId);
                    SortPrefs();
                    ApplyFilter();
                    _listView.RefreshItems();
                    UpdateStatus();
                });
                return cell;
            };
            typeCol.bindCell = (element, index) =>
            {
                var pref = _displayedPrefs[index];
                var drop = element.Q<DropdownField>(NameTypeField);
                drop.userData = pref;
                element.userData = pref;
                drop.SetValueWithoutNotify(pref.value.TypeDisplayName);
                drop.SetEnabled(!pref.isMarkedForDelete);
                ApplyRowStyle(element, pref, index);
            };
            typeCol.unbindCell = (element, _) =>
            {
                var drop = element.Q<DropdownField>(NameTypeField);
                if (drop != null) drop.userData = null;
            };
            columns.Add(typeCol);

            // ── Value ─────────────────────────────────────────────────────────
            var valueCol = new Column
            {
                name = ColValue, title = ColTitleValue,
                width = 200, minWidth = 60, sortable = true,
                resizable = true, stretchable = true,
            };
            _valueColumn = valueCol;
            valueCol.makeCell = () =>
            {
                var cell    = CloneCellTemplate(TplCellValue);
                var tf      = cell.Q<TextField>(NameValueField);
                var errIcon = cell.Q<Label>(NameErrIcon);
                RegisterRowContextMenu(cell);
                tf.RegisterValueChangedCallback(evt =>
                {
                    if (tf.userData is not PlayerPrefStore pref) return;
                    bool valid = pref.value.TrySetFromString(evt.newValue);
                    tf.EnableInClassList(ClassFieldInvalid, !valid);
                    errIcon.EnableInClassList(ClassHidden, valid);
                    errIcon.tooltip = valid
                        ? ""
                        : string.Format(TooltipInvalidValue, pref.value.TypeDisplayName);
                    RefreshRow(pref);
                    UpdateStatus();
                });
                return cell;
            };
            valueCol.bindCell = (element, index) =>
            {
                var pref    = _displayedPrefs[index];
                var tf      = element.Q<TextField>(NameValueField);
                var errIcon = element.Q<Label>(NameErrIcon);
                tf.userData = pref;
                element.userData = pref;
                tf.SetValueWithoutNotify(pref.StringValue);
                tf.SetEnabled(!pref.isMarkedForDelete);
                tf.RemoveFromClassList(ClassFieldInvalid);
                errIcon.EnableInClassList(ClassHidden, true); // reset on rebind
                ApplyRowStyle(element, pref, index);
            };
            valueCol.unbindCell = (element, _) =>
            {
                var tf = element.Q<TextField>(NameValueField);
                if (tf != null) tf.userData = null;
            };
            columns.Add(valueCol);

            // ── Edit Value ───────────────────────────────────────────────────
            var editCol = new Column
            {
                name = ColEdit, title = "",
                width = 28, minWidth = 28, maxWidth = 28,
                sortable = false, resizable = false,
            };
            _editColumn = editCol;
            editCol.makeCell = () =>
            {
                var cell = CloneCellTemplate(TplCellEdit);
                var btn = cell.Q<Button>(NameEditBtn);
                DisableFocusRecursive(cell);
                RegisterRowContextMenu(cell);
                btn.clicked += () =>
                {
                    if (btn.userData is not PlayerPrefStore pref) return;
                    OpenValueEditor(pref);
                };
                return cell;
            };
            editCol.bindCell = (element, index) =>
            {
                var pref = _displayedPrefs[index];
                var btn = element.Q<Button>(NameEditBtn);
                btn.userData = pref;
                btn.text = BtnTextEdit;
                btn.tooltip = TooltipEditValue;
                btn.SetEnabled(!pref.isMarkedForDelete);
                element.userData = pref;
                ApplyRowStyle(element, pref, index);
            };
            editCol.unbindCell = (element, _) =>
            {
                var btn = element.Q<Button>(NameEditBtn);
                if (btn != null) btn.userData = null;
            };
            columns.Add(editCol);

            // ── Row Action ────────────────────────────────────────────────────
            var rowActionCol = new Column
            {
                name = ColRowAction, title = "",
                width = 28, minWidth = 28, maxWidth = 28,
                sortable = false, resizable = false,
            };
            _rowActionColumn = rowActionCol;
            rowActionCol.makeCell = () =>
            {
                var cell = CloneCellTemplate(TplCellRowAction);
                var restoreBtn = cell.Q<Button>(NameRestoreBtn);
                var delBtn = cell.Q<Button>(NameDelBtn);
                DisableFocusRecursive(cell);
                RegisterRowContextMenu(cell);
                restoreBtn.clicked += () =>
                {
                    if (restoreBtn.userData is not PlayerPrefStore pref) return;
                    RestoreValue(pref);
                };
                delBtn.clicked += () =>
                {
                    if (delBtn.userData is not PlayerPrefStore pref) return;
                    ToggleDelete(pref);
                };
                return cell;
            };
            rowActionCol.bindCell = (element, index) =>
            {
                var pref = _displayedPrefs[index];
                var restoreBtn = element.Q<Button>(NameRestoreBtn);
                var delBtn = element.Q<Button>(NameDelBtn);
                bool showRestore = pref.isMarkedForDelete || CanRestoreValue(pref);

                restoreBtn.userData = pref;
                restoreBtn.text = BtnTextRestore;
                restoreBtn.tooltip = pref.isMarkedForDelete ? TooltipRestore : TooltipRestoreValue;
                restoreBtn.EnableInClassList(ClassHidden, !showRestore);
                restoreBtn.SetEnabled(showRestore);

                delBtn.userData = pref;
                delBtn.text = BtnTextDelete;
                delBtn.tooltip = TooltipDelete;
                delBtn.EnableInClassList(ClassHidden, showRestore);
                delBtn.SetEnabled(!pref.isMarkedForDelete);

                element.userData = pref;
                ApplyRowStyle(element, pref, index);
            };
            rowActionCol.unbindCell = (element, _) =>
            {
                element.userData = null;
                var restoreBtn = element.Q<Button>(NameRestoreBtn);
                var delBtn = element.Q<Button>(NameDelBtn);
                if (restoreBtn != null) restoreBtn.userData = null;
                if (delBtn != null) delBtn.userData = null;
            };
            columns.Add(rowActionCol);

            // ── MultiColumnListView ───────────────────────────────────────────
            _listView = new MultiColumnListView(columns)
            {
                name                          = NameList,
                itemsSource                   = _displayedPrefs,
                fixedItemHeight               = GetEffectiveRowHeight(),
                selectionType                 = SelectionType.Multiple,
                sortingMode                   = ColumnSortingMode.Custom,
                virtualizationMethod          = CollectionVirtualizationMethod.FixedHeight,
                showAlternatingRowBackgrounds = AlternatingRowBackground.None,
            };
            _listView.AddToClassList(NameList); // .ppe-list { flex-grow: 1 } in USS
            _listView.columnSortingChanged += OnColumnSortingChanged;
            _listView.selectionChanged += OnListSelectionChanged;
            LockColumnConfiguration(columns);
            _listView.schedule.Execute(LockColumnReordering).StartingIn(100);

            (_root.Q(NameListContainer) ?? _root).Add(_listView);

            SetupFilterSync();
        }

        private static void LockColumnConfiguration(Columns columns)
        {
            foreach (var column in columns)
            {
                column.optional = false;
            }
        }

        private void LockColumnReordering()
        {
            TrySetBoolProperty(_listView, "reorderable", false);
            TrySetBoolProperty(_listView, "canReorder", false);
            TrySetBoolProperty(_listView.columns, "reorderable", false);
            TrySetBoolProperty(_listView.columns, "canReorder", false);

            var header = _listView?.Q(className: ClassMultiColumnHeader);
            if (header == null)
                return;

            TrySetBoolProperty(header, "reorderable", false);
            TrySetBoolProperty(header, "canReorder", false);
            foreach (var element in header.Query<VisualElement>().ToList())
            {
                TrySetBoolProperty(element, "reorderable", false);
                TrySetBoolProperty(element, "canReorder", false);
            }

#if UNITY_EDITOR
            header.RegisterCallback<DragUpdatedEvent>(evt =>
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                evt.StopPropagation();
            }, TrickleDown.TrickleDown);
#endif
        }

        private static void TrySetBoolProperty(object target, string name, bool value)
        {
            var prop = target.GetType().GetProperty(
                name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop?.PropertyType == typeof(bool) && prop.CanWrite)
                prop.SetValue(target, value);
        }

        private void OnListSelectionChanged(IEnumerable<object> selectedItems)
        {
            if (_syncingListSelection)
                return;

            _selectedPrefs.Clear();
            foreach (var pref in selectedItems.OfType<PlayerPrefStore>())
                _selectedPrefs.Add(pref);

            if (_pendingSelectionAnchorIndex >= 0 &&
                _pendingSelectionAnchorIndex < _displayedPrefs.Count &&
                _selectedPrefs.Contains(_displayedPrefs[_pendingSelectionAnchorIndex]))
            {
                _selectionAnchorIndex = _pendingSelectionAnchorIndex;
            }
            else if (_selectionAnchorIndex < 0 || _selectionAnchorIndex >= _displayedPrefs.Count)
            {
                _selectionAnchorIndex = GetFirstVisibleSelectedIndex();
            }
            _pendingSelectionAnchorIndex = -1;

            RefreshSelectionStyles();
            UpdateSelectedControls();
        }

        private void RefreshSelectionStyles()
        {
            _listView?.RefreshItems();
        }

        private void RegisterRowContextMenu(VisualElement cell)
        {
            cell.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0) return;
                if (cell.userData is not PlayerPrefStore pref) return;

                int index = _displayedPrefs.IndexOf(pref);
                if (index < 0) return;

                if (evt.ctrlKey && evt.shiftKey)
                {
                    AddSelectionRangeToAnchor(index);
                    evt.StopImmediatePropagation();
                    return;
                }

                if (!evt.shiftKey)
                {
                    _selectionAnchorIndex = index;
                    _pendingSelectionAnchorIndex = index;
                }
            }, TrickleDown.TrickleDown);

#if UNITY_EDITOR
            cell.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0) return;
                if (cell.userData is not PlayerPrefStore pref) return;
                _dragStartPref = pref;
                _dragStartPosition = evt.position;
                _draggingRows = false;
            }, TrickleDown.TrickleDown);

            cell.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (_draggingRows || _dragStartPref == null || (evt.pressedButtons & 1) == 0)
                    return;

                if (((Vector2)evt.position - _dragStartPosition).sqrMagnitude < 25f)
                    return;

                var prefs = _selectedPrefs.Contains(_dragStartPref)
                    ? _selectedPrefs.Where(p => p != null).ToList()
                    : new List<PlayerPrefStore> { _dragStartPref };
                if (prefs.Count == 0)
                    return;

                if (!TryStartRowsDrag(prefs))
                {
                    _dragStartPref = null;
                    _draggingRows = false;
                    evt.StopPropagation();
                    return;
                }

                _draggingRows = true;
                evt.StopPropagation();
            }, TrickleDown.TrickleDown);

            cell.RegisterCallback<PointerUpEvent>(_ =>
            {
                _dragStartPref = null;
                _draggingRows = false;
            });

            cell.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 1) return;
                if (cell.userData is not PlayerPrefStore pref) return;
                ShowRowContextMenu(pref);
                evt.StopPropagation();
            }, TrickleDown.TrickleDown);
#endif
        }

#if UNITY_EDITOR
        private static bool TryStartRowsDrag(List<PlayerPrefStore> prefs)
        {
            if (prefs == null || prefs.Count == 0)
                return false;

            try
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.SetGenericData(DragPrefsGenericDataKey, prefs);
                DragAndDrop.StartDrag(GetRowsDragTitle(prefs));
                return true;
            }
            catch (NullReferenceException)
            {
                ClearRowsDragData();
                return false;
            }
        }

        private static void ClearRowsDragData()
        {
            try
            {
                DragAndDrop.SetGenericData(DragPrefsGenericDataKey, null);
            }
            catch (NullReferenceException)
            {
                // Unity can leave DragAndDrop uninitialized for runtime UI panels in play mode.
            }
        }

        private static string GetRowsDragTitle(List<PlayerPrefStore> prefs)
        {
            if (prefs.Count != 1)
                return $"{prefs.Count} PlayerPrefs";

            string name = prefs[0]?.name;
            return string.IsNullOrWhiteSpace(name) ? "PlayerPref" : name;
        }
#endif

        private int GetFirstVisibleSelectedIndex()
        {
            for (int i = 0; i < _displayedPrefs.Count; i++)
                if (_selectedPrefs.Contains(_displayedPrefs[i]))
                    return i;

            return -1;
        }

        private void AddSelectionRangeToAnchor(int clickedIndex)
        {
            if (_listView == null || clickedIndex < 0 || clickedIndex >= _displayedPrefs.Count)
                return;

            int anchorIndex = _selectionAnchorIndex;
            if (anchorIndex < 0 || anchorIndex >= _displayedPrefs.Count)
                anchorIndex = GetFirstVisibleSelectedIndex();
            if (anchorIndex < 0)
                anchorIndex = clickedIndex;

            int from = Mathf.Min(anchorIndex, clickedIndex);
            int to = Mathf.Max(anchorIndex, clickedIndex);

            for (int i = from; i <= to; i++)
                _selectedPrefs.Add(_displayedPrefs[i]);

            _selectionAnchorIndex = anchorIndex;
            SyncListSelectionToSelectedPrefs();
            _listView.ScrollToItem(clickedIndex);
            UpdateSelectedControls();
        }

        private void ShowRowContextMenu(PlayerPrefStore pref)
        {
#if UNITY_EDITOR
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(pref.isMarkedForDelete ? TooltipRestore : TooltipDelete),
                false, () => ToggleDelete(pref));

            if (CanRestoreValue(pref))
                menu.AddItem(new GUIContent(TooltipRestoreValue), false, () => RestoreValue(pref));
            else
                menu.AddDisabledItem(new GUIContent(TooltipRestoreValue));

            menu.AddSeparator("");
            foreach (string group in GetAllGroups())
            {
                string targetGroup = group;
                bool isCurrent = string.Equals(GetGroup(pref), targetGroup, StringComparison.OrdinalIgnoreCase);
                menu.AddItem(new GUIContent("Move To/" + targetGroup), isCurrent,
                    () => MovePrefsToGroup(new[] { pref }, targetGroup));
            }

            menu.ShowAsContext();
#else
            if (pref != null)
                ToggleDelete(pref);
#endif
        }

        private void SelectAllDisplayedRows()
        {
            if (_listView == null || _displayedPrefs.Count == 0)
                return;

            bool allSelected = _displayedPrefs.All(p => _selectedPrefs.Contains(p));
            if (allSelected)
            {
                _selectedPrefs.Clear();
                _syncingListSelection = true;
                _listView.ClearSelection();
                _syncingListSelection = false;
                UpdateSelectedControls();
                return;
            }

            _selectedPrefs.Clear();
            foreach (var pref in _displayedPrefs)
                _selectedPrefs.Add(pref);

            _syncingListSelection = true;
            _listView.SetSelection(Enumerable.Range(0, _displayedPrefs.Count));
            _syncingListSelection = false;
            UpdateSelectedControls();
        }

        private void OpenValueEditor(PlayerPrefStore pref)
        {
            if (pref == null || pref.isMarkedForDelete)
                return;

#if UNITY_EDITOR
            ValueEditorWindow.ShowWindow(
                pref.name,
                pref.StringValue,
                pref.value.TypeDisplayName,
                value =>
                {
                    if (!pref.value.TrySetFromString(value))
                    {
                        EditorUtility.DisplayDialog(
                            DialogTitleEditValue,
                            string.Format(TooltipInvalidValue, pref.value.TypeDisplayName),
                            DialogBtnOk);
                        return false;
                    }

                    RefreshRow(pref);
                    UpdateStatus();
                    return true;
                });
#else
            ShowValueEditorDialog(
                pref.name,
                pref.StringValue,
                pref.value.TypeDisplayName,
                value =>
                {
                    if (!pref.value.TrySetFromString(value))
                        return false;

                    RefreshRow(pref);
                    UpdateStatus();
                    return true;
                });
#endif
        }

        private static bool CanRestoreValue(PlayerPrefStore pref) =>
            pref != null && !pref.isNew && !pref.isMarkedForDelete && pref.Changed;

        private static bool CanRestoreSelected(PlayerPrefStore pref) =>
            pref != null && !pref.isNew && (pref.isMarkedForDelete || pref.Changed);

        // =====================================================================
        // Filter–column width synchronisation
        // =====================================================================

        private void RegisterSelectAllHeader(VisualElement header)
        {
            if (header == null ||
                header.ClassListContains(ClassSelectAllHeaderRegistered))
            {
                return;
            }

            header.AddToClassList(ClassSelectAllHeaderRegistered);
            header.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.button != 0)
                    return;

                SelectAllDisplayedRows();
                evt.StopPropagation();
            }, TrickleDown.TrickleDown);
        }

        /// <summary>
        /// Polls every 50 ms until the MultiColumnListView's internal header
        /// element is available, then registers <see cref="GeometryChangedEvent"/>
        /// directly on each resizable column-header cell.
        /// <para>
        /// IMPORTANT: <c>GeometryChangedEvent</c> does NOT bubble in UIToolkit,
        /// so the listener must be placed on the individual column cell elements,
        /// not on the parent header container. The header container listener is
        /// kept only as a fallback for whole-list-view resize (window resize).
        /// </para>
        /// </summary>
        private void SetupFilterSync()
        {
            _headerSyncRegistered = false;
            _listView.schedule
                .Execute(TryRegisterHeaderSync)
                .Every(50)
                .Until(() => _headerSyncRegistered);
        }

        private void TryRegisterHeaderSync()
        {
            var header = _listView?.Q(className: ClassMultiColumnHeader);
            if (header == null) return;

            // Stop polling as soon as the header element exists.
            _headerSyncRegistered = true;

            // Unity sets each column-header cell's name to the column's name,
            // so Q(ColKey) reliably finds the Key column's header cell.
            var keyColHeader  = header.Q(ColKey);
            var typeColHeader = header.Q(ColType);
            var selectColHeader = header.Q(ColSelect);

            if (keyColHeader == null) return;

            RegisterSelectAllHeader(selectColHeader);

            // Initial sync (layout may already be resolved at this point).
            SyncFilterWidths(keyColHeader, typeColHeader);

            // GeometryChangedEvent does NOT bubble — register on every cell
            // that we want to track so column-drag resizes are caught.
            keyColHeader.RegisterCallback<GeometryChangedEvent>(_ =>
                SyncFilterWidths(keyColHeader, typeColHeader));

            typeColHeader?.RegisterCallback<GeometryChangedEvent>(_ =>
                SyncFilterWidths(keyColHeader, typeColHeader));

            // Header container event fires when the whole list resizes
            // (e.g. window resize). Key / type widths are fixed in that case,
            // but re-running the sync is harmless and keeps things correct.
            header.RegisterCallback<GeometryChangedEvent>(_ =>
                SyncFilterWidths(keyColHeader, typeColHeader));
        }

        private void SyncFilterWidths(VisualElement keyColHeader, VisualElement typeColHeader)
        {
            if (_isPhonePortraitLayout)
            {
                ClearFilterCellWidth(_filterColKeyCell);
                ClearFilterCellWidth(_filterColTypeCell);
                return;
            }

            // ColValue is flex-grow (stretchable) — no fixed-width sync needed.
            // ColActions is fixed at 28 px in USS and is not resizable.
            SetFilterCellWidth(_filterColKeyCell,  keyColHeader);
            SetFilterCellWidth(_filterColTypeCell, typeColHeader);
        }

        private static void ClearFilterCellWidth(VisualElement filterCell)
        {
            if (filterCell == null) return;
            filterCell.style.width = StyleKeyword.Auto;
        }

        private static void SetFilterCellWidth(VisualElement filterCell, VisualElement colHeader)
        {
            if (filterCell == null || colHeader == null) return;
            float w = colHeader.layout.width;
            if (w > 0)
                filterCell.style.width = w;
        }

        // =====================================================================
        // Row styling
        // =====================================================================

        private void ApplyRowStyle(VisualElement cell, PlayerPrefStore pref, int index)
        {
            bool isDup     = !pref.isMarkedForDelete && _duplicateKeys.Contains(pref.name);
            bool isDeleted = pref.isMarkedForDelete;
            bool isNew     = !isDeleted && !isDup && pref.isNew;
            bool isEdited  = !isDeleted && !isDup && !pref.isNew && pref.Changed;
            bool isSelected = pref != null && _selectedPrefs.Contains(pref);
            bool hasState  = isDup || isDeleted || isNew || isEdited;

            cell.EnableInClassList(ClassRowDeleted,   isDeleted);
            cell.EnableInClassList(ClassRowDuplicate, isDup);
            cell.EnableInClassList(ClassRowNew,       isNew);
            cell.EnableInClassList(ClassRowEdited,    isEdited);
            cell.EnableInClassList(ClassRowSelected,  isSelected);

            // Odd rows get a subtle dark stripe only when no other state is active.
            cell.EnableInClassList(ClassRowOdd, index % 2 != 0 && !hasState);
        }

        private void RefreshRow(PlayerPrefStore pref)
        {
            int index = _displayedPrefs.IndexOf(pref);
            if (index >= 0) _listView.RefreshItem(index);
        }

        // =====================================================================
        // Column sorting
        // =====================================================================

        private void OnColumnSortingChanged()
        {
            SortPrefs();
            ApplyFilter();
        }

        private void SortPrefs()
        {
            var descs = _listView.sortedColumns?.ToList()
                        ?? new List<SortColumnDescription>();

            if (descs.Count == 0)
            {
                _prefs.Sort((a, b) =>
                    string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                var primary = descs[0];
                int Compare(PlayerPrefStore a, PlayerPrefStore b)
                {
                    int cmp = primary.columnName switch
                    {
                        ColFavorite => IsFavorite(a).CompareTo(IsFavorite(b)),
                        ColKey   => string.Compare(a.name, b.name,
                                        StringComparison.OrdinalIgnoreCase),
                        ColType  => string.Compare(a.value.TypeDisplayName,
                                        b.value.TypeDisplayName,
                                        StringComparison.OrdinalIgnoreCase),
                        ColValue => string.Compare(a.StringValue, b.StringValue,
                                        StringComparison.OrdinalIgnoreCase),
                        _        => string.Compare(a.name, b.name,
                                        StringComparison.OrdinalIgnoreCase),
                    };
                    return primary.direction == SortDirection.Descending ? -cmp : cmp;
                }
                _prefs.Sort(Compare);
            }
        }

        // =====================================================================
        // VisualElement actions
        // =====================================================================

        private void AddNewPref()
        {
            var pref = new PlayerPrefStore(DefaultNewKey, PrefValue.Create(DefaultNewTypeId, ""))
            {
                isNew = true,
            };
            _prefs.Add(pref);
            SetGroup(pref, _currentGroup);
            ValidateDuplicates();
            SortPrefs();
            ApplyFilter();

            if (!_displayedPrefs.Contains(pref))
            {
                ClearFiltersWithoutNotify();
                ApplyFilter();
            }

            FocusPrefKeyField(pref);
        }

        private void BeginAddCustomGroup()
        {
#if UNITY_EDITOR
            NewGroupWindow.ShowWindow(AddCustomGroup);
#else
            ShowTextInputDialog(
                DialogTitleNewGroup,
                "Name",
                GetNextRuntimeGroupName(),
                false,
                DialogBtnOk,
                DialogBtnCancel,
                AddCustomGroup);
#endif
        }

        private bool AddCustomGroup(string group)
        {
            group = (group ?? "").Trim();
            if (string.IsNullOrEmpty(group))
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog(DialogTitleInvalidGroup, MsgGroupNameRequired, DialogBtnOk);
#else
                SetStatusMessage(MsgGroupNameRequired);
#endif
                return false;
            }

            if (IsKnownGroup(group))
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog(DialogTitleInvalidGroup, MsgGroupNameReserved, DialogBtnOk);
#else
                SetStatusMessage(MsgGroupNameReserved);
#endif
                return false;
            }

            _customGroups.Add(group);
            SavePersistentKeySets();
            RebuildGroupControls();
            SetCurrentGroup(group);
            return true;
        }

        private string GetNextRuntimeGroupName()
        {
            int index = 1;
            string group;
            do
            {
                group = $"Group {index++}";
            }
            while (IsKnownGroup(group));

            return group;
        }

        private void ShowConfirmDialog(
            string title,
            string message,
            string ok,
            string cancel,
            Action onConfirmed,
            bool danger)
        {
            ShowDialog(
                title,
                message,
                null,
                new[]
                {
                    new RuntimeDialogButton(ok, () =>
                    {
                        CloseDialog();
                        onConfirmed?.Invoke();
                    }, danger),
                    new RuntimeDialogButton(cancel, CloseDialog, false),
                },
                null);
        }

        private void ShowMessageDialog(string title, string message)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog(title, message, DialogBtnOk);
#else
            ShowDialog(
                title,
                message,
                null,
                new[] { new RuntimeDialogButton(DialogBtnOk, CloseDialog, false) },
                null);
#endif
        }

        private void ShowTextInputDialog(
            string title,
            string label,
            string value,
            bool multiline,
            string ok,
            string cancel,
            Func<string, bool> onSubmit)
        {
            var field = new TextField(label)
            {
                value = value ?? "",
                multiline = multiline,
            };
            field.AddToClassList(ClassDialogField);

            var body = new VisualElement();
            body.AddToClassList(ClassDialogBody);
            body.Add(field);

            ShowDialog(
                title,
                "",
                body,
                new[]
                {
                    new RuntimeDialogButton(ok, () =>
                    {
                        if (onSubmit?.Invoke(field.value ?? "") == false)
                            return;
                        CloseDialog();
                    }, false),
                    new RuntimeDialogButton(cancel, CloseDialog, false),
                },
                field);
        }

        private void ShowValueEditorDialog(
            string key,
            string value,
            string typeName,
            Func<string, bool> onSubmit)
        {
            var body = new VisualElement();
            body.AddToClassList(ClassDialogBody);

            var field = new TextField($"{key} ({typeName})")
            {
                value = value ?? "",
                multiline = true,
            };
            field.AddToClassList(ClassDialogField);
            field.AddToClassList("ppe-value-editor-field");

            var error = new Label();
            error.AddToClassList(ClassDialogError);
            error.AddToClassList(ClassHidden);

            body.Add(field);
            body.Add(error);

            ShowDialog(
                DialogTitleEditValue,
                "",
                body,
                new[]
                {
                    new RuntimeDialogButton(DialogBtnOk, () =>
                    {
                        if (onSubmit?.Invoke(field.value ?? "") == false)
                        {
                            error.text = string.Format(TooltipInvalidValue, typeName);
                            error.RemoveFromClassList(ClassHidden);
                            return;
                        }

                        CloseDialog();
                    }, false),
                    new RuntimeDialogButton(DialogBtnCancel, CloseDialog, false),
                },
                field);
        }

        private void ShowChoiceDialog(
            string title,
            string message,
            RuntimeDialogButton[] buttons)
        {
            ShowDialog(title, message, null, buttons, null);
        }

        private void ShowExportGroupsDialog(List<string> groups)
        {
            groups ??= new List<string>();

            var body = new VisualElement();
            body.AddToClassList(ClassDialogBody);

            var scroll = new ScrollView();
            scroll.AddToClassList(ClassDialogScroll);
            var toggles = new List<Toggle>();
            foreach (string group in groups)
            {
                var toggle = new Toggle(group) { value = true };
                toggles.Add(toggle);
                scroll.Add(toggle);
            }

            var error = new Label();
            error.AddToClassList(ClassDialogError);
            error.AddToClassList(ClassHidden);

            body.Add(scroll);
            body.Add(error);

            ShowDialog(
                DialogTitleExportGroups,
                "",
                body,
                new[]
                {
                    new RuntimeDialogButton(DialogBtnOk, () =>
                    {
                        var selected = toggles
                            .Where(t => t.value)
                            .Select(t => t.label)
                            .ToList();
                        if (selected.Count == 0)
                        {
                            error.text = MsgExportGroupsEmpty;
                            error.RemoveFromClassList(ClassHidden);
                            return;
                        }

                        CloseDialog();
                        ExportGroupsToJson(selected);
                    }, false),
                    new RuntimeDialogButton(DialogBtnCancel, CloseDialog, false),
                },
                toggles.FirstOrDefault());
        }

        private void ShowMoveToGroupDialog(Action<string> onSelected)
        {
            var groups = GetAllGroups();
            if (!_isPhonePortraitLayout)
            {
                var buttons = groups
                    .Select(group =>
                    {
                        string target = group;
                        return new RuntimeDialogButton(target, () =>
                        {
                            CloseDialog();
                            onSelected?.Invoke(target);
                        }, false);
                    })
                    .Concat(new[] { new RuntimeDialogButton(DialogBtnCancel, CloseDialog, false) })
                    .ToArray();

                ShowChoiceDialog("Move To Group", "", buttons);
                return;
            }

            var body = new VisualElement();
            body.AddToClassList(ClassDialogBody);

            var scroll = new ScrollView();
            scroll.AddToClassList(ClassDialogScroll);
            scroll.AddToClassList(ClassMoveGroupList);

            Button firstGroupButton = null;
            foreach (string group in groups)
            {
                string target = group;
                var button = new Button(() =>
                {
                    CloseDialog();
                    onSelected?.Invoke(target);
                })
                {
                    text = target,
                };

                button.AddToClassList(ClassDialogButton);
                button.AddToClassList(ClassMoveGroupButton);
                scroll.Add(button);
                firstGroupButton ??= button;
            }

            body.Add(scroll);

            ShowDialog(
                "Move To Group",
                "",
                body,
                new[] { new RuntimeDialogButton(DialogBtnCancel, CloseDialog, false) },
                firstGroupButton);
        }

        private void ShowDialog(
            string title,
            string message,
            VisualElement body,
            IEnumerable<RuntimeDialogButton> buttons,
            VisualElement focusTarget)
        {
            CloseDialog();

            _dialogOverlay = new VisualElement { focusable = true };
            _dialogOverlay.AddToClassList(ClassDialogOverlay);

            var dialog = new VisualElement();
            dialog.AddToClassList(ClassDialog);

            var titleLabel = new Label(title ?? "");
            titleLabel.AddToClassList(ClassDialogTitle);
            dialog.Add(titleLabel);

            if (!string.IsNullOrEmpty(message))
            {
                var messageLabel = new Label(message);
                messageLabel.AddToClassList(ClassDialogMessage);
                dialog.Add(messageLabel);
            }

            if (body != null)
                dialog.Add(body);

            var actions = new VisualElement();
            actions.AddToClassList(ClassDialogActions);
            foreach (var buttonInfo in buttons ?? Enumerable.Empty<RuntimeDialogButton>())
            {
                var button = new Button(buttonInfo.OnClick) { text = buttonInfo.Text };
                button.AddToClassList(ClassDialogButton);
                button.EnableInClassList(ClassDialogDangerButton, buttonInfo.Danger);
                actions.Add(button);
            }

            dialog.Add(actions);
            _dialogOverlay.Add(dialog);
            _root.Add(_dialogOverlay);

            _dialogOverlay.RegisterCallback<KeyDownEvent>(evt =>
            {
                if (evt.keyCode == KeyCode.Escape)
                {
                    CloseDialog();
                    evt.StopPropagation();
                }
            });

            _dialogOverlay.schedule.Execute(() =>
            {
                (focusTarget ?? _dialogOverlay).Focus();
            }).StartingIn(0);
        }

        private void CloseDialog()
        {
            if (_dialogOverlay == null)
                return;

            _dialogOverlay.RemoveFromHierarchy();
            _dialogOverlay = null;
        }

        private sealed class RuntimeDialogButton
        {
            public RuntimeDialogButton(string text, Action onClick, bool danger)
            {
                Text = text;
                OnClick = onClick;
                Danger = danger;
            }

            public string Text { get; }
            public Action OnClick { get; }
            public bool Danger { get; }
        }

        private void ConfirmAction(
            string key,
            string title,
            string message,
            string ok,
            string cancel,
            Action onConfirmed)
        {
#if UNITY_EDITOR
            if (EditorUtility.DisplayDialog(title, message, ok, cancel))
                onConfirmed?.Invoke();
#else
            _pendingConfirmationKey = key;
            _pendingConfirmationUntil = Time.unscaledTime;
            ShowConfirmDialog(
                title,
                message,
                ok,
                cancel,
                onConfirmed,
                ok == DialogBtnDelete || ok == DialogBtnOverwrite);
#endif
        }

        private void DeleteGroup(string group)
        {
            if (IsDefaultGroup(group) || !_customGroups.Contains(group))
                return;

            ConfirmAction(
                "delete-group:" + group,
                DialogTitleDeleteGroup,
                string.Format(MsgDeleteGroupFmt, group),
                DialogBtnOk,
                DialogBtnCancel,
                () =>
                {
                    foreach (var pref in _prefs.Where(p =>
                                 string.Equals(GetGroup(p), group, StringComparison.OrdinalIgnoreCase)))
                        SetGroup(pref, GroupMain);

                    _customGroups.Remove(group);
                    if (string.Equals(_currentGroup, group, StringComparison.OrdinalIgnoreCase))
                        _currentGroup = GroupMain;
                    SavePersistentKeySets();
                    RebuildGroupControls();
                    ValidateDuplicates();
                    ApplyFilter();
                });
        }

        private void DeleteAllPrefsImmediately()
        {
            ConfirmAction(
                "delete-all",
                DialogTitleDeleteAll,
                MsgDeleteAll,
                DialogBtnDelete,
                DialogBtnCancel,
                () =>
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    _prefs.Clear();
                    _displayedPrefs.Clear();
                    _selectedPrefs.Clear();
                    _favoriteKeys.Clear();
                    _customGroups.Clear();
                    _keyGroups.Clear();
                    _currentGroup = GroupMain;
                    SavePersistentKeySets();
                    RebuildGroupControls();
                    ValidateDuplicates();
                    ApplyFilter();
                });
        }

        private void ToggleFavorite(PlayerPrefStore pref)
        {
            if (pref == null || string.IsNullOrEmpty(pref.name)) return;

            if (!_favoriteKeys.Remove(pref.name))
                _favoriteKeys.Add(pref.name);

            SavePersistentKeySets();
            SortPrefs();
            ApplyFilter();
        }

        private void RestoreValue(PlayerPrefStore pref)
        {
            if (pref == null || pref.isNew)
                return;

            if (pref.isMarkedForDelete)
            {
                pref.isMarkedForDelete = false;
                ValidateDuplicates();
                _listView.RefreshItems();
                UpdateStatus();
                return;
            }

            if (!CanRestoreValue(pref))
                return;

            pref.Reset();
            SortPrefs();
            ApplyFilter();
            UpdateStatus();
        }

        private void RestoreSelectedItems()
        {
            var toRestore = _selectedPrefs
                .Where(CanRestoreSelected)
                .ToList();

            if (toRestore.Count == 0) return;

            foreach (var pref in toRestore)
            {
                if (pref.isMarkedForDelete)
                    pref.isMarkedForDelete = false;
                else
                    pref.Reset();
            }

            ValidateDuplicates();
            SortPrefs();
            ApplyFilter();
        }

        private void ShowMoveSelectedMenu()
        {
            if (_selectedPrefs.Count == 0 || _moveSelectedButton == null)
                return;

#if UNITY_EDITOR
            var menu = new GenericMenu();
            foreach (string group in GetAllGroups())
            {
                string targetGroup = group;
                menu.AddItem(new GUIContent(targetGroup), false,
                    () => MoveSelectedItemsToGroup(targetGroup));
            }
            menu.DropDown(_moveSelectedButton.worldBound);
#else
            ShowMoveToGroupDialog(MoveSelectedItemsToGroup);
#endif
        }

        private void MoveSelectedItemsToGroup(string targetGroup)
        {
            MovePrefsToGroup(_selectedPrefs.Where(p => p != null).ToList(), targetGroup);
        }

        private void MovePrefsToGroup(IEnumerable<PlayerPrefStore> prefs, string targetGroup)
        {
            if (!IsKnownGroup(targetGroup))
                targetGroup = GroupMain;

            var selected = prefs
                .Where(p => p != null)
                .ToList();

            if (selected.Count == 0) return;

            foreach (var pref in selected)
                SetGroup(pref, targetGroup);

            _selectedPrefs.Clear();
            _listView?.ClearSelection();
            SavePersistentKeySets();
            ValidateDuplicates();
            SortPrefs();
            ApplyFilter();
        }

        private void ToggleDelete(PlayerPrefStore pref)
        {
            if (pref.isNew)
            {
                _prefs.Remove(pref);
                _selectedPrefs.Remove(pref);
                ValidateDuplicates();
                ApplyFilter();
            }
            else
            {
                pref.isMarkedForDelete = !pref.isMarkedForDelete;
                if (pref.isMarkedForDelete)
                    _selectedPrefs.Remove(pref);
                ValidateDuplicates();
                _listView.RefreshItems();
                UpdateStatus();
            }
        }

        private void SaveAll()
        {
            if (ValidateDuplicates())
            {
                _listView.RefreshItems();
                return;
            }

            for (int i = _prefs.Count - 1; i >= 0; i--)
            {
                var pref = _prefs[i];
                if (pref.isMarkedForDelete)
                {
                    PlayerPrefs.DeleteKey(pref.name);
                    _favoriteKeys.Remove(pref.name);
                    _keyGroups.Remove(pref.name);
                    _selectedPrefs.Remove(pref);
                    _prefs.RemoveAt(i);
                    continue;
                }
                pref.value.WriteToPlayerPrefs(pref.name);
                pref.Save();
            }

            PlayerPrefs.Save();
            SavePersistentKeySets();
            ApplyFilter();
        }

        private void RefreshPlayerPrefs()
        {
            ClearSelectedPrefs();
            _prefs.Clear();
            _prefs.AddRange(PlayerPrefsRuntimeStoreConverter.ReadAll(_prefsFetcher));
            SortPrefs();
            ValidateDuplicates();
            ApplyFilter();
        }

        private void SaveSnapshot()
        {
            if (ValidateDuplicates())
            {
                _listView?.RefreshItems();
                SetStatusMessage(MsgSnapshotDuplicateBlocked);
                return;
            }

            string displayName = NormalizeSnapshotDisplayName(GetSnapshotNameFromField());
            PlayerPrefsSnapshotInfo existing;

            try
            {
                existing = FindSnapshotByDisplayName(displayName);
            }
            catch (Exception ex)
            {
                ShowMessageDialog(DialogTitleSnapshotSaveError, ex.Message);
                return;
            }

            if (existing != null)
            {
                ConfirmAction(
                    "overwrite-snapshot:" + existing.FilePath,
                    DialogTitleSnapshotOverwrite,
                    string.Format(MsgSnapshotOverwriteConfirmFmt, existing.DisplayName),
                    DialogBtnOverwrite,
                    DialogBtnCancel,
                    () => SaveSnapshotConfirmed(displayName, existing.FilePath));
                return;
            }

            SaveSnapshotConfirmed(displayName, null);
        }

        private void SaveSnapshotConfirmed(string displayName, string overwritePath)
        {
            if (ValidateDuplicates())
            {
                _listView?.RefreshItems();
                SetStatusMessage(MsgSnapshotDuplicateBlocked);
                return;
            }

            var exportable = _prefs
                .Where(IsSnapshotExportablePref)
                .ToList();

            try
            {
                var snapshot = WriteSnapshotFile(displayName, exportable, overwritePath);
                RefreshSnapshots();
                _snapshotNameField?.SetValueWithoutNotify(GetDefaultSnapshotName());
                SetStatusMessage(string.Format(
                    MsgSnapshotSavedFmt,
                    snapshot.RowCount,
                    snapshot.DisplayName));
            }
            catch (Exception ex)
            {
                ShowMessageDialog(DialogTitleSnapshotSaveError, ex.Message);
            }
        }

        private void RefreshSnapshots()
        {
            _snapshots.Clear();

            try
            {
                _snapshots.AddRange(ScanSnapshotFiles());
                SortSnapshots();
            }
            catch (Exception ex)
            {
                SetStatusMessage($"{DialogTitleSnapshotLoadError}: {ex.Message}");
            }

            _snapshotsListView?.Rebuild();
            UpdateTabLabels();

            if (_showingSnapshots)
                UpdateSnapshotsStatus();
        }

        private void LoadSnapshot(PlayerPrefsSnapshotInfo snapshot)
        {
            if (snapshot == null)
                return;

            ConfirmAction(
                "load-snapshot:" + snapshot.FilePath,
                DialogTitleSnapshotLoad,
                string.Format(MsgSnapshotLoadConfirmFmt, snapshot.DisplayName),
                DialogBtnLoad,
                DialogBtnCancel,
                () => LoadSnapshotConfirmed(snapshot));
        }

        private void LoadSnapshotConfirmed(PlayerPrefsSnapshotInfo snapshot)
        {
            List<PlayerPrefStore> loaded;
            try
            {
                loaded = ReadSnapshotFile(snapshot.FilePath);
            }
            catch (Exception ex)
            {
                ShowMessageDialog(DialogTitleSnapshotLoadError, ex.Message);
                return;
            }

            PlayerPrefs.DeleteAll();
            foreach (var pref in loaded.Where(IsSnapshotExportablePref))
            {
                pref.value.WriteToPlayerPrefs(pref.name);
                pref.Save();
            }
            PlayerPrefs.Save();

            ClearSelectedPrefs();
            _prefs.Clear();
            _prefs.AddRange(loaded.Where(IsSnapshotExportablePref));
            PruneMetadataForCurrentPrefs();
            SavePersistentKeySets();
            SortPrefs();
            ValidateDuplicates();
            _currentGroup = GroupMain;
            _showingSnapshots = false;
            SyncTabToggles();
            ApplyFilter();
            SetStatusMessage(string.Format(MsgSnapshotLoadedFmt, _prefs.Count, snapshot.DisplayName));
        }

        private void DeleteSnapshot(PlayerPrefsSnapshotInfo snapshot)
        {
            if (snapshot == null)
                return;

            ConfirmAction(
                "delete-snapshot:" + snapshot.FilePath,
                DialogTitleSnapshotDelete,
                string.Format(MsgSnapshotDeleteConfirmFmt, snapshot.DisplayName),
                DialogBtnDelete,
                DialogBtnCancel,
                () => DeleteSnapshotConfirmed(snapshot));
        }

        private void DeleteSnapshotConfirmed(PlayerPrefsSnapshotInfo snapshot)
        {
            try
            {
                if (File.Exists(snapshot.FilePath))
                    File.Delete(snapshot.FilePath);
                RefreshSnapshots();
                SetStatusMessage(string.Format(MsgSnapshotDeletedFmt, snapshot.DisplayName));
            }
            catch (Exception ex)
            {
                ShowMessageDialog(DialogTitleSnapshotDeleteError, ex.Message);
            }
        }

        private void ExportToJson()
        {
#if UNITY_EDITOR
            ExportGroupsWindow.ShowWindow(GetAllGroups(), groups =>
            {
                if (groups == null || groups.Count == 0)
                {
                    EditorUtility.DisplayDialog(DialogTitleExportGroups, MsgExportGroupsEmpty, DialogBtnOk);
                    return;
                }

                ExportGroupsToJson(groups);
            });
#else
            ShowExportGroupsDialog(GetAllGroups());
#endif
        }

        private void ExportGroupsToJson(List<string> groups)
        {
            string defaultName = string.Format(ExportFileNameFmt, Application.productName);
#if UNITY_EDITOR
            string path = EditorUtility.SaveFilePanel(DialogTitleExport, "", defaultName, FileExtJson);
            if (string.IsNullOrEmpty(path))
                return;
#else
            string path = Path.Combine(Application.persistentDataPath, defaultName + "." + FileExtJson);
#endif

            var exportableGroups = groups
                .Where(IsKnownGroup)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group,
                    group => _prefs
                        .Where(p => !p.isMarkedForDelete &&
                                    string.Equals(GetGroup(p), group, StringComparison.OrdinalIgnoreCase))
                        .ToList(),
                    StringComparer.OrdinalIgnoreCase);
            int exportCount = exportableGroups.Sum(pair => pair.Value.Count);
            try
            {
                File.WriteAllText(path,
                    _serializer.SerializeGroups(exportableGroups),
                    System.Text.Encoding.UTF8);
#if UNITY_EDITOR
                ShowMessageDialog(DialogTitleExportDone, string.Format(MsgExportSuccess, exportCount, path));
#else
                ShowMessageDialog(DialogTitleExportDone, string.Format(MsgExportSuccess, exportCount, path));
#endif
            }
            catch (Exception ex)
            {
                ShowMessageDialog(DialogTitleExportError, ex.Message);
            }
        }

        private void ImportFromJson()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel(DialogTitleImport, "", FileExtJson);
            if (string.IsNullOrEmpty(path))
                return;
#else
            string path = Path.Combine(Application.persistentDataPath,
                string.Format(ExportFileNameFmt, Application.productName) + "." + FileExtJson);
#endif
            if (!File.Exists(path))
            {
                ShowMessageDialog(DialogTitleImportError, $"{path} not found.");
                return;
            }

            string data;
            try
            {
                data = File.ReadAllText(path, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ShowMessageDialog(DialogTitleImportError, ex.Message);
                return;
            }

            List<PlayerPrefStore> imported;
            try
            {
                imported = _serializer.Deserialize(data);
            }
            catch (Exception ex)
            {
                ShowMessageDialog(DialogTitleImportError, string.Format(MsgImportParseError, ex.Message));
                return;
            }

            if (imported.Count == 0)
            {
                ShowMessageDialog(DialogTitleImportResult, MsgImportEmpty);
                return;
            }

#if UNITY_EDITOR
            int choice = EditorUtility.DisplayDialogComplex(
                DialogTitleImportChoice,
                string.Format(MsgImportChoice, imported.Count, path),
                DialogBtnMerge,
                DialogBtnCancel,
                DialogBtnReplaceAll);

            if (choice == 1)
                return;

            ApplyImportedPrefs(imported, choice == 2);
#else
            ShowChoiceDialog(
                DialogTitleImportChoice,
                string.Format(MsgImportChoice, imported.Count, path),
                new[]
                {
                    new RuntimeDialogButton(DialogBtnMerge, () =>
                    {
                        CloseDialog();
                        ApplyImportedPrefs(imported, false);
                    }, false),
                    new RuntimeDialogButton(DialogBtnReplaceAll, () =>
                    {
                        CloseDialog();
                        ApplyImportedPrefs(imported, true);
                    }, true),
                    new RuntimeDialogButton(DialogBtnCancel, CloseDialog, false),
                });
#endif
        }

        private void ApplyImportedPrefs(List<PlayerPrefStore> imported, bool replaceAll)
        {
            if (replaceAll)
            {
                ClearSelectedPrefs();
                _prefs.Clear();
                foreach (var imp in imported)
                {
                    imp.isNew = true;
                    _prefs.Add(imp);
                }
            }
            else // Merge
            {
                foreach (var imp in imported)
                {
                    var existing = _prefs.FirstOrDefault(p =>
                        string.Equals(p.name, imp.name, StringComparison.OrdinalIgnoreCase));
                    if (existing != null)
                        existing.value = imp.value;
                    else
                    {
                        imp.isNew = true;
                        _prefs.Add(imp);
                    }
                }
            }

            ValidateDuplicates();
            SortPrefs();
            ApplyFilter();
        }

        private static bool IsSnapshotExportablePref(PlayerPrefStore pref) =>
            pref != null &&
            !pref.isMarkedForDelete &&
            !string.IsNullOrEmpty(pref.name) &&
            !PlayerPrefsRuntimeStoreConverter.IsEditorMetadataKey(pref.name);

        private static string SnapshotDirectoryPath =>
            Path.Combine(Application.persistentDataPath, SnapshotDirectoryName);

        private string GetSnapshotNameFromField()
        {
            string name = _snapshotNameField?.value?.Trim();
            return string.IsNullOrEmpty(name)
                ? GetDefaultSnapshotName()
                : name;
        }

        private static string GetDefaultSnapshotName() =>
            string.Format(
                SnapshotDefaultNameFmt,
                DateTime.Now.ToString(SnapshotDefaultDateFormat, CultureInfo.InvariantCulture));

        private static PlayerPrefsSnapshotInfo WriteSnapshotFile(
            string displayName,
            List<PlayerPrefStore> prefs,
            string overwritePath = null)
        {
            Directory.CreateDirectory(SnapshotDirectoryPath);

            displayName = NormalizeSnapshotDisplayName(displayName);
            var dto = new PlayerPrefsSnapshotFileDto
            {
                Name = displayName,
                CreatedUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                Entries = prefs
                    .Where(IsSnapshotExportablePref)
                    .Select(ToSnapshotEntry)
                    .ToList(),
            };

            string path = string.IsNullOrEmpty(overwritePath)
                ? GetUniqueSnapshotFilePath(displayName)
                : overwritePath;
            string json = JsonConvert.SerializeObject(dto, Formatting.Indented);
            File.WriteAllText(path, json, System.Text.Encoding.UTF8);
            return ToSnapshotInfo(path, dto);
        }

        private static PlayerPrefsSnapshotInfo FindSnapshotByDisplayName(string displayName)
        {
            displayName = NormalizeSnapshotDisplayName(displayName);
            return ScanSnapshotFiles()
                .FirstOrDefault(snapshot =>
                    string.Equals(
                        NormalizeSnapshotDisplayName(snapshot.DisplayName),
                        displayName,
                        StringComparison.OrdinalIgnoreCase));
        }

        private static List<PlayerPrefsSnapshotInfo> ScanSnapshotFiles()
        {
            var result = new List<PlayerPrefsSnapshotInfo>();
            if (!Directory.Exists(SnapshotDirectoryPath))
                return result;

            foreach (string path in Directory.GetFiles(
                         SnapshotDirectoryPath,
                         "*" + SnapshotFileExtension,
                         SearchOption.TopDirectoryOnly))
            {
                try
                {
                    result.Add(ToSnapshotInfo(path, ReadSnapshotDto(path)));
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Skipping PlayerPrefs snapshot \"{path}\": {ex.Message}");
                }
            }

            SortSnapshotsByCreatedDescending(result);
            return result;
        }

        private static List<PlayerPrefStore> ReadSnapshotFile(string path)
        {
            var dto = ReadSnapshotDto(path);
            return (dto.Entries ?? new List<PlayerPrefsSnapshotEntryDto>())
                .Where(IsValidSnapshotEntry)
                .Select(entry => PlayerPrefStore.FromTypeString(
                    entry.Key,
                    string.IsNullOrEmpty(entry.Type) ? DefaultNewTypeId : entry.Type,
                    entry.Value ?? ""))
                .ToList();
        }

        private static PlayerPrefsSnapshotFileDto ReadSnapshotDto(string path)
        {
            string json = File.ReadAllText(path, System.Text.Encoding.UTF8);
            var dto = JsonConvert.DeserializeObject<PlayerPrefsSnapshotFileDto>(json);
            if (dto == null)
                throw new InvalidDataException("Snapshot file is empty or invalid.");
            dto.Entries ??= new List<PlayerPrefsSnapshotEntryDto>();
            return dto;
        }

        private static PlayerPrefsSnapshotEntryDto ToSnapshotEntry(PlayerPrefStore pref) =>
            new PlayerPrefsSnapshotEntryDto
            {
                Key = pref.name,
                Type = pref.value.TypeId,
                Value = pref.StringValue,
            };

        private static bool IsValidSnapshotEntry(PlayerPrefsSnapshotEntryDto entry) =>
            entry != null && !string.IsNullOrEmpty(entry.Key);

        private static PlayerPrefsSnapshotInfo ToSnapshotInfo(
            string path,
            PlayerPrefsSnapshotFileDto dto)
        {
            var file = new FileInfo(path);
            DateTime createdUtc = file.Exists ? file.CreationTimeUtc : DateTime.UtcNow;
            if (!string.IsNullOrEmpty(dto.CreatedUtc) &&
                DateTime.TryParse(
                    dto.CreatedUtc,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out var parsed))
            {
                createdUtc = parsed.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(parsed, DateTimeKind.Utc)
                    : parsed.ToUniversalTime();
            }

            string displayName = NormalizeSnapshotDisplayName(dto.Name);
            if (string.Equals(displayName, SnapshotEmptyNameFallback, StringComparison.Ordinal))
                displayName = GetSnapshotFallbackName(path);

            return new PlayerPrefsSnapshotInfo
            {
                DisplayName = displayName,
                FilePath = path,
                RowCount = dto.Entries?.Count(IsValidSnapshotEntry) ?? 0,
                SizeBytes = file.Exists ? file.Length : 0,
                CreatedUtc = createdUtc,
            };
        }

        private static string NormalizeSnapshotDisplayName(string name)
        {
            name = (name ?? "").Trim();
            return string.IsNullOrEmpty(name)
                ? SnapshotEmptyNameFallback
                : name;
        }

        private static string GetSnapshotFallbackName(string path)
        {
            string fileName = Path.GetFileName(path);
            if (fileName.EndsWith(SnapshotFileExtension, StringComparison.OrdinalIgnoreCase))
                return fileName.Substring(0, fileName.Length - SnapshotFileExtension.Length);
            return Path.GetFileNameWithoutExtension(path);
        }

        private static string GetUniqueSnapshotFilePath(string displayName)
        {
            string fileStem = SanitizeFileName(displayName);
            if (string.IsNullOrEmpty(fileStem))
                fileStem = SnapshotEmptyNameFallback;

            string path = Path.Combine(SnapshotDirectoryPath, fileStem + SnapshotFileExtension);
            if (!File.Exists(path))
                return path;

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            for (int i = 1; i < 1000; i++)
            {
                string suffix = i == 1
                    ? "_" + timestamp
                    : "_" + timestamp + "_" + i.ToString(CultureInfo.InvariantCulture);
                path = Path.Combine(SnapshotDirectoryPath, fileStem + suffix + SnapshotFileExtension);
                if (!File.Exists(path))
                    return path;
            }

            return Path.Combine(
                SnapshotDirectoryPath,
                fileStem + "_" + Guid.NewGuid().ToString("N") + SnapshotFileExtension);
        }

        private static string SanitizeFileName(string name)
        {
            name = NormalizeSnapshotDisplayName(name);
            var invalid = new HashSet<char>(Path.GetInvalidFileNameChars());
            var chars = name
                .Select(ch => invalid.Contains(ch) ? '_' : ch)
                .ToArray();
            string safe = new string(chars).Trim('.', ' ');
            return safe.Length <= 80 ? safe : safe.Substring(0, 80).Trim('.', ' ');
        }

        private static string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
                return bytes.ToString(CultureInfo.InvariantCulture) + " B";

            double kb = bytes / 1024d;
            if (kb < 1024d)
                return kb.ToString("0.#", CultureInfo.InvariantCulture) + " KB";

            double mb = kb / 1024d;
            return mb.ToString("0.##", CultureInfo.InvariantCulture) + " MB";
        }

        // =====================================================================
        // Lifecycle
        // =====================================================================

        private void OnDisable()
        {
            PlayerPrefsEditorMetadata.SetFloat(EditorPrefsRowHeight, _rowHeight);
            SavePersistentKeySets();
        }

        // =====================================================================
        // Row-height resize handle
        // =====================================================================

        /// <summary>
        /// Wires the draggable handle element between the filter row and the list
        /// so that dragging it vertically resizes all rows globally.
        /// Row height is persisted across sessions via metadata storage.
        /// </summary>
        private void SetupRowResizeHandle()
        {
            _rowHeight = PlayerPrefsEditorMetadata.GetFloat(EditorPrefsRowHeight, RowHeightDefault);
            _listView.fixedItemHeight = GetEffectiveRowHeight();

            var handle = _root.Q<VisualElement>(NameRowResizeHandle);
            if (handle == null) return;
            // Cursor is set via USS (.ppe-row-resize-handle { cursor: resize-vertical; })

            float startY      = 0f;
            float startHeight = 0f;
            bool  dragging    = false;

            handle.RegisterCallback<PointerDownEvent>(evt =>
            {
                startY      = evt.position.y;
                startHeight = _rowHeight;
                dragging    = true;
                handle.CapturePointer(evt.pointerId);
                evt.StopPropagation();
            });

            handle.RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!dragging) return;
                float newH = Mathf.Clamp(startHeight + (evt.position.y - startY),
                    RowHeightMin, RowHeightMax);
                if (Mathf.Approximately(newH, _rowHeight)) return;
                _rowHeight                = newH;
                _listView.fixedItemHeight = GetEffectiveRowHeight();
                evt.StopPropagation();
            });

            handle.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (!dragging) return;
                dragging = false;
                handle.ReleasePointer(evt.pointerId);
                PlayerPrefsEditorMetadata.SetFloat(EditorPrefsRowHeight, _rowHeight);
                evt.StopPropagation();
            });

            handle.RegisterCallback<PointerCancelEvent>(evt =>
            {
                dragging = false;
                handle.ReleasePointer(evt.pointerId);
            });
        }

        // =====================================================================
        // Keyboard shortcuts
        // =====================================================================

        private void RegisterKeyboardShortcuts()
        {
            _root.RegisterCallback<KeyDownEvent>(evt =>
            {
                bool selectAll = IsSelectAllShortcut(evt);
                if (!selectAll && evt.keyCode != KeyCode.Delete) return;

                if (IsTextInputFocused())
                    return;

                if (selectAll)
                {
                    SelectAllDisplayedPrefs();
                    evt.StopImmediatePropagation();
                    return;
                }

                if (_selectedPrefs.Count == 0) return;

                DeleteSelectedItems();
                evt.StopImmediatePropagation();
            });
        }

        private static bool IsSelectAllShortcut(KeyDownEvent evt) =>
            evt.keyCode == KeyCode.A && (evt.ctrlKey || evt.commandKey);

        private bool IsTextInputFocused()
        {
            var focused = _root.focusController?.focusedElement;
            var focusedVe = focused as VisualElement;
            return focused is TextField ||
                   focused is DropdownField ||
                   focusedVe?.GetFirstAncestorOfType<TextField>() != null ||
                   focusedVe?.GetFirstAncestorOfType<DropdownField>() != null;
        }

        private void SelectAllDisplayedPrefs()
        {
            if (_showingSnapshots || _listView == null || _displayedPrefs.Count == 0)
                return;

            _selectedPrefs.Clear();
            foreach (var pref in _displayedPrefs)
                if (pref != null)
                    _selectedPrefs.Add(pref);

            _selectionAnchorIndex = 0;
            _pendingSelectionAnchorIndex = -1;
            _syncingListSelection = true;
            _listView.ClearSelection();
            for (int i = 0; i < _displayedPrefs.Count; i++)
                _listView.AddToSelection(i);
            _syncingListSelection = false;
            UpdateSelectedControls();
        }

        private void ClearSelectedPrefs()
        {
            if (_selectedPrefs.Count == 0) return;
            _selectedPrefs.Clear();
            _listView?.ClearSelection();
            UpdateSelectedControls();
        }

        private void DeleteSelectedItems()
        {
            var toDelete = _selectedPrefs
                .Where(p => _prefs.Contains(p))
                .ToList();

            if (toDelete.Count == 0) return;

            foreach (var pref in toDelete)
            {
                if (pref.isNew)
                    _prefs.Remove(pref);
                else
                    pref.isMarkedForDelete = true;
            }

            _selectedPrefs.Clear();
            _listView?.ClearSelection();
            ValidateDuplicates();
            ApplyFilter();
        }

        private sealed class PlayerPrefsSnapshotInfo
        {
            public string DisplayName;
            public string FilePath;
            public int RowCount;
            public long SizeBytes;
            public DateTime CreatedUtc;

            public DateTime CreatedLocal => CreatedUtc.ToLocalTime();
        }

        private sealed class PlayerPrefsSnapshotFileDto
        {
            [JsonProperty("name")]
            public string Name;

            [JsonProperty("createdUtc")]
            public string CreatedUtc;

            [JsonProperty("entries")]
            public List<PlayerPrefsSnapshotEntryDto> Entries;
        }

        private sealed class PlayerPrefsSnapshotEntryDto
        {
            [JsonProperty("key")]
            public string Key;

            [JsonProperty("type")]
            public string Type;

            [JsonProperty("value")]
            public string Value;
        }

#if UNITY_EDITOR
        private sealed class ExportGroupsWindow : EditorWindow
        {
            private Action<List<string>> _onExport;
            private readonly List<Toggle> _toggles = new List<Toggle>();

            public static void ShowWindow(List<string> groups, Action<List<string>> onExport)
            {
                var window = CreateInstance<ExportGroupsWindow>();
                window.titleContent = new GUIContent(DialogTitleExportGroups);
                window._onExport = onExport;
                int visibleRows = Mathf.Min(10, Mathf.Max(1, groups.Count));
                var size = new Vector2(280, 64 + visibleRows * 22);
                window.minSize = size;
                window.maxSize = size;
                window.position = new Rect(200, 200, size.x, size.y);
                window.Build(groups);
                window.ShowUtility();
            }

            private void Build(List<string> groups)
            {
                rootVisualElement.style.paddingLeft = 8;
                rootVisualElement.style.paddingRight = 8;
                rootVisualElement.style.paddingTop = 8;
                rootVisualElement.style.paddingBottom = 8;
                rootVisualElement.style.flexDirection = FlexDirection.Column;

                var scroll = new ScrollView();
                scroll.style.flexGrow = 1;
                scroll.verticalScrollerVisibility = groups.Count > 10
                    ? ScrollerVisibility.AlwaysVisible
                    : ScrollerVisibility.Hidden;
                foreach (string group in groups)
                {
                    var toggle = new Toggle(group) { value = true };
                    _toggles.Add(toggle);
                    scroll.Add(toggle);
                }

                var buttons = new VisualElement();
                buttons.style.flexDirection = FlexDirection.Row;
                buttons.style.justifyContent = Justify.FlexStart;
                buttons.style.marginTop = 8;

                var cancel = new Button(Close) { text = DialogBtnCancel };
                var export = new Button(() =>
                {
                    var selected = _toggles
                        .Where(t => t.value)
                        .Select(t => t.label)
                        .ToList();
                    Close();
                    _onExport?.Invoke(selected);
                })
                {
                    text = DialogBtnOk,
                };

                buttons.Add(export);
                buttons.Add(cancel);
                rootVisualElement.Add(scroll);
                rootVisualElement.Add(buttons);
            }
        }

        private sealed class NewGroupWindow : EditorWindow
        {
            private Func<string, bool> _onOk;
            private TextField _nameField;

            public static void ShowWindow(Func<string, bool> onOk)
            {
                var window = CreateInstance<NewGroupWindow>();
                window.titleContent = new GUIContent(DialogTitleNewGroup);
                window._onOk = onOk;
                var size = new Vector2(280, 82);
                window.minSize = size;
                window.maxSize = size;
                window.position = new Rect(240, 240, size.x, size.y);
                window.Build();
                window.ShowUtility();
            }

            private void Build()
            {
                rootVisualElement.style.paddingLeft = 8;
                rootVisualElement.style.paddingRight = 8;
                rootVisualElement.style.paddingTop = 8;
                rootVisualElement.style.paddingBottom = 8;

                _nameField = new TextField { label = "Name" };
                _nameField.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
                    {
                        Submit();
                        evt.StopPropagation();
                    }
                    else if (evt.keyCode == KeyCode.Escape)
                    {
                        Close();
                        evt.StopPropagation();
                    }
                });

                var buttons = new VisualElement();
                buttons.style.flexDirection = FlexDirection.Row;
                buttons.style.justifyContent = Justify.FlexStart;
                buttons.style.marginTop = 8;

                buttons.Add(new Button(Submit) { text = DialogBtnOk });
                buttons.Add(new Button(Close) { text = DialogBtnCancel });

                rootVisualElement.Add(_nameField);
                rootVisualElement.Add(buttons);
                _nameField.schedule.Execute(() => _nameField.Focus());
            }

            private void Submit()
            {
                if (_onOk?.Invoke(_nameField.value) != false)
                    Close();
            }
        }

        private sealed class ValueEditorWindow : EditorWindow
        {
            private Func<string, bool> _onOk;
            private TextField _valueField;

            public static void ShowWindow(
                string key,
                string value,
                string typeName,
                Func<string, bool> onOk)
            {
                var window = CreateInstance<ValueEditorWindow>();
                window.titleContent = new GUIContent(DialogTitleEditValue);
                window._onOk = onOk;
                window.minSize = new Vector2(360, 220);
                window.position = new Rect(200, 200, 640, 420);
                window.Build(key, value, typeName);
                window.ShowUtility();
            }

            private void Build(string key, string value, string typeName)
            {
                var ss = Resources.Load<StyleSheet>(StyleSheetResourcePath);
                if (ss != null)
                    rootVisualElement.styleSheets.Add(ss);

                rootVisualElement.style.paddingLeft = 8;
                rootVisualElement.style.paddingRight = 8;
                rootVisualElement.style.paddingTop = 8;
                rootVisualElement.style.paddingBottom = 8;

                var title = new Label($"{key} ({typeName})");
                title.style.marginBottom = 6;
                title.style.unityFontStyleAndWeight = FontStyle.Bold;

                _valueField = new TextField
                {
                    multiline = true,
                    value = value,
                };
                _valueField.AddToClassList("ppe-value-editor-field");
                _valueField.style.flexGrow = 1;
                _valueField.style.flexShrink = 1;
                _valueField.style.alignSelf = Align.Stretch;
                _valueField.style.whiteSpace = WhiteSpace.Normal;

                var buttons = new VisualElement();
                buttons.style.flexDirection = FlexDirection.Row;
                buttons.style.justifyContent = Justify.FlexStart;
                buttons.style.marginTop = 8;

                var cancel = new Button(Close) { text = DialogBtnCancel };
                var ok = new Button(() =>
                {
                    if (_onOk?.Invoke(_valueField.value ?? "") == false)
                        return;
                    Close();
                })
                {
                    text = DialogBtnOk,
                };

                buttons.Add(ok);
                buttons.Add(cancel);
                rootVisualElement.Add(title);
                rootVisualElement.Add(_valueField);
                rootVisualElement.Add(buttons);
            }
        }
#endif

    }
}
