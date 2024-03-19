using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Digital {
public class DigitalWorkspace : Workspace
{
    private static DigitalWorkspace instance;
    public static DigitalWorkspace Instance {
        get {
            return instance ?? FindAnyObjectByType<DigitalWorkspace>();
        }
    }

    [SerializeField]
    private DigitalWorkspaceBridge digitalWorkspaceBridge;

    public DigitalWorkspaceBridge ApiBridge { get => digitalWorkspaceBridge; }

    void Awake() {
        instance = this;
    }
}

}

