using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Actors {

public abstract class BaseActor : MonoBehaviour
{
    [SerializeField] private VirtualWorkspace ws;

    private void Awake() {
        ws = ws ? ws : FindFirstObjectByType<VirtualWorkspace>();
    }

    // TODO an actor in Virtual Workspace
}

};