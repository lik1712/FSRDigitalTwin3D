using UnityEngine;

namespace FSR.Workspace.Digital {
public class DigitalWorkspace : Workspace
{
    private static DigitalWorkspace instance;
    public static DigitalWorkspace Instance {
        get {
            return instance != null ? instance : FindAnyObjectByType<DigitalWorkspace>();
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

