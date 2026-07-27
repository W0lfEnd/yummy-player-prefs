using UnityEngine;

namespace YummyDev.PlayerPrefsTool
{
    public class DeferredPlayerPrefsSaver : MonoBehaviour
    {
        [SerializeField] private float _saveIntervalSec = 1f;

        private bool _isDirty;
        private float _timeSinceLastSave;

        public void MarkDirty()
        {
            _isDirty = true;
        }

        private void LateUpdate()
        {
            if (!_isDirty)
                return;

            _timeSinceLastSave += Time.deltaTime;
            if (_timeSinceLastSave < _saveIntervalSec)
                return;

            Save();
        }

        private void Save()
        {
            _isDirty = false;
            _timeSinceLastSave = 0f;
            PlayerPrefs.Save();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && _isDirty)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            if (_isDirty)
            {
                Save();
            }
        }
    }
}
