using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Sensors {

/**
 * A sensor source within the Virtual Workspace.
 *
 * Generates Data for a Sensor within the Digital Workspace
 */
public abstract class SensorSource : MonoBehaviour
{
    [SerializeField] private VirtualWorkspace ws;

    protected IObservable<Hashtable> sensorData = Observable.Empty<Hashtable>();

    private void Awake() {
        ws = ws ? ws : FindFirstObjectByType<VirtualWorkspace>();
    }

    public IObservable<Hashtable> SensorData {
        get { return sensorData; }
    }

}

}

