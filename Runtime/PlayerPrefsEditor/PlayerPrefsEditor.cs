#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace YummyDev.PlayerPrefsTool
{
    public sealed class PlayerPrefsEditor : EditorWindow
    {
        private PlayerPrefsEditorView _view;

        [MenuItem("Tools/Yummy Player Prefs/Editor", false, 1)]
        private static void Init() =>
            GetWindow<PlayerPrefsEditor>("PlayerPrefs Editor");

        public void CreateGUI()
        {
            _view?.Dispose();
            rootVisualElement.Clear();

            _view = new PlayerPrefsEditorView(
                rootVisualElement,
                Resources.Load<VisualTreeAsset>(PlayerPrefsEditorView.UxmlResourcePath),
                Resources.Load<VisualTreeAsset>(PlayerPrefsEditorView.CellTemplatesResourcePath),
                Resources.Load<StyleSheet>(PlayerPrefsEditorView.StyleSheetResourcePath));
        }

        private void OnDisable()
        {
            _view?.Dispose();
            _view = null;
        }
    }
}
#endif
