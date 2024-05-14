using System.Collections;
using System.Collections.Generic;
using FSR.DigitalTwin.Unity.Workspace.Digital;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FSR.UI {
    public class ControlPanel : MonoBehaviour
    {

        [SerializeField] private Button connectButton;
        [SerializeField] private Button disconnectButton;

        [SerializeField] private TMP_InputField serverIpAddrField;
        [SerializeField] private TMP_InputField serverPortField;

        [SerializeField] private TMP_Text statusLabel;

        [SerializeField] private Toggle isListeningToggle;
        [SerializeField] private Toggle noClippingToggle;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void OnConnectButtonPress() {
            DigitalWorkspace.Instance.ApiBridge.SetServerConnectionAddress(serverIpAddrField.text, int.Parse(serverPortField.text));
            DigitalWorkspace.Instance.ApiBridge.Connect();
            if (DigitalWorkspace.Instance.ApiBridge.IsConnected) {
                statusLabel.text = "Connected!";
                statusLabel.color = Color.green;
                connectButton.gameObject.SetActive(false);
                disconnectButton.gameObject.SetActive(true);
            }
        }

        public void OnDisconnectButtonPress() {
            if (DigitalWorkspace.Instance.ApiBridge.IsConnected) {
                DigitalWorkspace.Instance.ApiBridge.Disconnect();
                statusLabel.text = "Disconnected!";
                statusLabel.color = Color.red;
                connectButton.gameObject.SetActive(true);
                disconnectButton.gameObject.SetActive(false);
            }
        }

        public void OnIsListeningTick(bool value) {
            DigitalWorkspace.Instance.Settings.SetListening(value);
        }

        public void OnNoClipTick(bool value) {
            DigitalWorkspace.Instance.Settings.SetNoClipMode(value);
        }
    }
}


