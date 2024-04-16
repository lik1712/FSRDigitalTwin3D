using System.Collections;
using System.Collections.Generic;
using FSR.DigitalTwinLayer.GRPC.Lib;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace {
public abstract class Workspace : MonoBehaviour
{
    public enum EKind {
        DIGITAL, VIRTUAL, PHYSICAL
    }

    public abstract EKind Kind { get; }
}

}


