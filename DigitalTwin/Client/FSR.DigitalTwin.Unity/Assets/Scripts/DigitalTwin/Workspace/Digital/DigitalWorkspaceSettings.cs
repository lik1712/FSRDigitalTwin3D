using UniRx;

namespace FSR.DigitalTwin.Unity.Workspace.Digital {

    public class DigitalWorkspaceSettings {
        
        private ReactiveProperty<bool> _isListening = new(true);
        private ReactiveProperty<bool> _noClipEnabled = new(true);

        public ReadOnlyReactiveProperty<bool> IsListening => _isListening.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> NoClipEnabled => _noClipEnabled.ToReadOnlyReactiveProperty();

        public void SetListening(bool listening) {
            _isListening.Value = listening;
        }
        public void SetNoClipMode(bool enabled) {
            _noClipEnabled.Value = enabled;
        }

    }

}