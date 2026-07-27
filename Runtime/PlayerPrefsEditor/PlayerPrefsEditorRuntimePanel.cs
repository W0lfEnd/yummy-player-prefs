using UnityEngine;
using UnityEngine.UIElements;

namespace YummyDev.PlayerPrefsTool
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UIDocument))]
    public sealed class PlayerPrefsEditorRuntimePanel : MonoBehaviour
    {
        public enum RuntimeLayoutMode
        {
            Auto,
            Desktop,
            Mobile,
        }

        private const string RuntimeThemeResourcePath = "PlayerPrefsEditor/PlayerPrefsEditorRuntimeTheme";
        private const string SafeAreaRootName = "player-prefs-editor-safe-area";
        private const float DesktopPanelScale = 1f;
        private const float RuntimeScaleMatch = 0.5f;
        private const float NarrowMobileLayoutMaxWidth = 520f;
        private const float EditorSimulatorPortraitMinAspect = 1.08f;
        private static readonly Vector2Int NarrowMobileReferenceResolution = new Vector2Int(390, 844);
        private static readonly Vector2Int MobileReferenceResolution = new Vector2Int(540, 960);
        private static readonly Vector2Int DesktopReferenceResolution = new Vector2Int(1600, 900);

        [SerializeField] private bool showOnStart = true;
        [SerializeField] private bool respectSafeArea = true;
        [SerializeField] private RuntimeLayoutMode layoutMode = RuntimeLayoutMode.Auto;
        [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote;
        [SerializeField] private VisualTreeAsset uxmlAsset;
        [SerializeField] private VisualTreeAsset cellTemplatesAsset;
        [SerializeField] private StyleSheet styleSheet;

        private UIDocument _document;
        private VisualElement _safeAreaRoot;
        private PlayerPrefsEditorView _view;
        private Rect _lastAppliedSafeArea;
        private int _lastAppliedScreenWidth = -1;
        private int _lastAppliedScreenHeight = -1;
        private float _lastAppliedRootWidth = -1f;
        private float _lastAppliedRootHeight = -1f;

        public bool IsVisible =>
            _document != null &&
            _document.rootVisualElement.style.display.value != DisplayStyle.None;

        public RuntimeLayoutMode LayoutMode
        {
            get => layoutMode;
            set
            {
                if (layoutMode == value)
                    return;

                layoutMode = value;
                ApplyRuntimePresentation(force: true);
            }
        }

        public static PlayerPrefsEditorRuntimePanel Create(PanelSettings panelSettings)
        {
            EnsureRuntimeThemeStyleSheet(panelSettings);

            var go = new GameObject("PlayerPrefs Editor Runtime Panel");
            var document = go.AddComponent<UIDocument>();
            document.panelSettings = panelSettings;
            return go.AddComponent<PlayerPrefsEditorRuntimePanel>();
        }

        public static PanelSettings CreateDefaultPanelSettings(int sortingOrder = 1000)
        {
            var panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
            panelSettings.name = "PlayerPrefs Editor Runtime Panel Settings";
            ConfigureDefaultPanelSettings(panelSettings, sortingOrder);
            return panelSettings;
        }

        public static void ConfigureDefaultPanelSettings(
            PanelSettings panelSettings,
            int sortingOrder = 1000)
        {
            if (panelSettings == null)
                return;

            if (ShouldUseMobilePanelScale())
            {
                panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                panelSettings.referenceResolution = MobileReferenceResolution;
                panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                panelSettings.match = RuntimeScaleMatch;
            }
            else
            {
                panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
                panelSettings.scale = DesktopPanelScale;
                panelSettings.referenceResolution = DesktopReferenceResolution;
                panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                panelSettings.match = RuntimeScaleMatch;
            }

            panelSettings.sortingOrder = sortingOrder;
            EnsureRuntimeThemeStyleSheet(panelSettings);
        }

        private static bool ShouldUseMobilePanelScale()
        {
            if (Application.isMobilePlatform)
                return true;

#if UNITY_EDITOR
            return IsCurrentEditorSimulatorPortrait();
#else
            return false;
#endif
        }

#if UNITY_EDITOR
        private static bool IsCurrentEditorSimulatorPortrait()
        {
            Rect safeArea = Screen.safeArea;
            return (IsPortraitAspect(Screen.width, Screen.height, EditorSimulatorPortraitMinAspect) &&
                    IsNarrowWidth(Screen.width)) ||
                   (IsPortraitAspect(safeArea.width, safeArea.height, EditorSimulatorPortraitMinAspect) &&
                    IsNarrowWidth(safeArea.width));
        }
#endif

        private static bool IsPortraitAspect(float width, float height, float minAspect)
        {
            return width > 0f && height >= width * minAspect;
        }

        private void Awake()
        {
            _document = GetComponent<UIDocument>();
            EnsureRuntimeThemeStyleSheet(_document.panelSettings);
            Build();

            if (showOnStart)
                Show();
            else
                Hide();
        }

        private void Update()
        {
            if (toggleKey != KeyCode.None && Input.GetKeyDown(toggleKey))
                Toggle();

            if (IsVisible)
                ApplyRuntimePresentation();
        }

        private void OnDestroy()
        {
            _view?.Dispose();
            _view = null;
        }

        public void Show()
        {
            Build();
            _document.rootVisualElement.style.display = DisplayStyle.Flex;
            ApplyRuntimePresentation(force: true);
        }

        public void Hide()
        {
            if (_document != null)
                _document.rootVisualElement.style.display = DisplayStyle.None;
        }

        public void Toggle()
        {
            if (IsVisible)
                Hide();
            else
                Show();
        }

        public void Rebuild()
        {
            _view?.Dispose();
            _view = null;
            _document.rootVisualElement.Clear();
            _safeAreaRoot = null;
            ResetSafeAreaCache();
            Build();
        }

        private void Build()
        {
            if (_view != null)
                return;

            if (_document == null)
                _document = GetComponent<UIDocument>();

            ApplyRuntimePresentation(force: true);
            _safeAreaRoot = EnsureSafeAreaRoot();
            ApplySafeAreaIfNeeded(force: true);

            _view = new PlayerPrefsEditorView(
                _safeAreaRoot,
                uxmlAsset,
                cellTemplatesAsset,
                styleSheet,
                phonePortraitLayoutOverride: ShouldUseMobileLayout());
            ApplyLayoutModeIfNeeded(force: true);
        }

        private void ApplyRuntimePresentation(bool force = false)
        {
            if (_document == null)
                return;

            ApplyPanelScaleIfNeeded(force);
            ApplySafeAreaIfNeeded(force);
            ApplyLayoutModeIfNeeded(force);
        }

        private VisualElement EnsureSafeAreaRoot()
        {
            var root = _document.rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.flexGrow = 1;
            root.style.backgroundColor = new Color(0.118f, 0.118f, 0.118f, 1f);

            if (_safeAreaRoot != null && _safeAreaRoot.parent == root)
                return _safeAreaRoot;

            _safeAreaRoot = root.Q<VisualElement>(SafeAreaRootName);
            if (_safeAreaRoot == null)
            {
                _safeAreaRoot = new VisualElement { name = SafeAreaRootName };
                root.Add(_safeAreaRoot);
            }

            _safeAreaRoot.style.flexDirection = FlexDirection.Column;
            _safeAreaRoot.style.flexGrow = 1;
            _safeAreaRoot.style.flexShrink = 1;
            _safeAreaRoot.style.minWidth = 0;
            _safeAreaRoot.style.minHeight = 0;
            return _safeAreaRoot;
        }

        private void ApplySafeAreaIfNeeded(bool force = false)
        {
            if (_document == null || _document.rootVisualElement == null)
                return;

            var root = _document.rootVisualElement;
            float rootWidth = GetElementSize(root.resolvedStyle.width, root.layout.width);
            float rootHeight = GetElementSize(root.resolvedStyle.height, root.layout.height);
            if (rootWidth <= 0f || rootHeight <= 0f)
            {
                root.schedule.Execute(() => ApplySafeAreaIfNeeded(true)).StartingIn(0);
                return;
            }

            int screenWidth = Mathf.Max(1, Screen.width);
            int screenHeight = Mathf.Max(1, Screen.height);
            Rect safeArea = respectSafeArea
                ? Screen.safeArea
                : new Rect(0f, 0f, screenWidth, screenHeight);

            if (!force &&
                screenWidth == _lastAppliedScreenWidth &&
                screenHeight == _lastAppliedScreenHeight &&
                safeArea == _lastAppliedSafeArea &&
                Mathf.Approximately(rootWidth, _lastAppliedRootWidth) &&
                Mathf.Approximately(rootHeight, _lastAppliedRootHeight))
            {
                return;
            }

            _lastAppliedScreenWidth = screenWidth;
            _lastAppliedScreenHeight = screenHeight;
            _lastAppliedSafeArea = safeArea;
            _lastAppliedRootWidth = rootWidth;
            _lastAppliedRootHeight = rootHeight;

            if (safeArea.width <= 0f || safeArea.height <= 0f)
                safeArea = new Rect(0f, 0f, screenWidth, screenHeight);

            float left = Mathf.Clamp(safeArea.xMin / screenWidth * rootWidth, 0f, rootWidth);
            float right = Mathf.Clamp((screenWidth - safeArea.xMax) / screenWidth * rootWidth, 0f, rootWidth);
            float bottom = Mathf.Clamp(safeArea.yMin / screenHeight * rootHeight, 0f, rootHeight);
            float top = Mathf.Clamp((screenHeight - safeArea.yMax) / screenHeight * rootHeight, 0f, rootHeight);

            root.style.paddingLeft = left;
            root.style.paddingRight = right;
            root.style.paddingTop = top;
            root.style.paddingBottom = bottom;

            _view?.RefreshAdaptiveLayout();
        }

        private void ApplyPanelScaleIfNeeded(bool force = false)
        {
            PanelSettings panelSettings = _document != null ? _document.panelSettings : null;
            if (panelSettings == null)
                return;

            bool mobileScale = ShouldUseMobileLayout();
            Vector2Int mobileReferenceResolution = ShouldUseNarrowMobileLayout()
                ? NarrowMobileReferenceResolution
                : MobileReferenceResolution;
            bool alreadyApplied = mobileScale
                ? panelSettings.scaleMode == PanelScaleMode.ScaleWithScreenSize &&
                  panelSettings.referenceResolution == mobileReferenceResolution &&
                  Mathf.Approximately(panelSettings.match, RuntimeScaleMatch)
                : panelSettings.scaleMode == PanelScaleMode.ConstantPixelSize &&
                  Mathf.Approximately(panelSettings.scale, DesktopPanelScale);

            if (!force && alreadyApplied)
                return;

            if (mobileScale)
            {
                panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
                panelSettings.referenceResolution = mobileReferenceResolution;
                panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                panelSettings.match = RuntimeScaleMatch;
            }
            else
            {
                panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
                panelSettings.scale = DesktopPanelScale;
                panelSettings.referenceResolution = DesktopReferenceResolution;
                panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
                panelSettings.match = RuntimeScaleMatch;
            }

            panelSettings.sortingOrder = Mathf.RoundToInt(panelSettings.sortingOrder);
            EnsureRuntimeThemeStyleSheet(panelSettings);
        }

        private void ApplyLayoutModeIfNeeded(bool force = false)
        {
            if (_view == null)
                return;

            bool mobileLayout = ShouldUseMobileLayout();
            _view.SetPhonePortraitLayoutOverride(mobileLayout);
            if (force)
                _view.RefreshAdaptiveLayout();
        }

        private bool ShouldUseMobileLayout()
        {
            switch (layoutMode)
            {
                case RuntimeLayoutMode.Mobile:
                    return true;
                case RuntimeLayoutMode.Desktop:
                    return false;
                default:
                    return ShouldUseAutomaticMobileLayout();
            }
        }

        private bool ShouldUseAutomaticMobileLayout()
        {
            if (Application.isMobilePlatform)
                return IsCurrentRuntimePortrait() || ShouldUseNarrowMobileLayout();

#if UNITY_EDITOR
            return IsCurrentRuntimePortrait() && ShouldUseNarrowMobileLayout();
#else
            return false;
#endif
        }

        private bool ShouldUseNarrowMobileLayout()
        {
            if (layoutMode == RuntimeLayoutMode.Desktop)
                return false;

            if (_document != null && _document.rootVisualElement != null)
            {
                var root = _document.rootVisualElement;
                float rootWidth = GetElementSize(root.resolvedStyle.width, root.layout.width);
                if (IsNarrowWidth(rootWidth))
                    return true;
            }

            Rect safeArea = Screen.safeArea;
            return IsNarrowWidth(Screen.width) || IsNarrowWidth(safeArea.width);
        }

        private bool IsCurrentRuntimePortrait()
        {
            if (_document != null && _document.rootVisualElement != null)
            {
                var root = _document.rootVisualElement;
                float width = GetElementSize(root.resolvedStyle.width, root.layout.width);
                float height = GetElementSize(root.resolvedStyle.height, root.layout.height);
                if (IsPortraitAspect(width, height, 1f))
                    return true;
            }

            Rect safeArea = Screen.safeArea;
            return IsPortraitAspect(Screen.width, Screen.height, 1f) ||
                   IsPortraitAspect(safeArea.width, safeArea.height, 1f);
        }

        private static bool IsNarrowWidth(float width)
        {
            return width > 0f && width <= NarrowMobileLayoutMaxWidth;
        }

        private static float GetElementSize(float resolvedSize, float layoutSize)
        {
            if (resolvedSize > 0f && !float.IsNaN(resolvedSize))
                return resolvedSize;

            return layoutSize > 0f && !float.IsNaN(layoutSize) ? layoutSize : 0f;
        }

        private void ResetSafeAreaCache()
        {
            _lastAppliedSafeArea = default;
            _lastAppliedScreenWidth = -1;
            _lastAppliedScreenHeight = -1;
            _lastAppliedRootWidth = -1f;
            _lastAppliedRootHeight = -1f;
        }

        private static void EnsureRuntimeThemeStyleSheet(PanelSettings panelSettings)
        {
            if (panelSettings == null || panelSettings.themeStyleSheet != null)
                return;

            ThemeStyleSheet themeStyleSheet = Resources.Load<ThemeStyleSheet>(RuntimeThemeResourcePath);
            if (themeStyleSheet != null)
                panelSettings.themeStyleSheet = themeStyleSheet;
        }
    }
}
