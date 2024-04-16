using FSR.Aas.GRPC.Lib.V3.Services;
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

    public override EKind Kind => EKind.DIGITAL;

    public OutputModifier StandardOutput = new() { Content = OutputContent.Normal, Cursor = "", Level = OutputLevel.Deep, Extent = OutputExtent.WithoutBlobValue, Limit = 32 };

    void Awake() {
        instance = this;
    }
}

}

