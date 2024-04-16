using System.Collections;
using System.Collections.Generic;
using FSR.DigitalTwin.Unity.Workspace.Digital;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Actors {

public abstract class BaseActor : DigitalTwinComponent
{
    [SerializeField] private VirtualWorkspace ws;

    public VirtualWorkspace VirtualWorkspace => ws;

    private void Awake() {
        ws = ws ? ws : FindFirstObjectByType<VirtualWorkspace>();
    }

    // TODO an actor in Virtual Workspace
}

};