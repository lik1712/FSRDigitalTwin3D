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
public abstract class SensorSource<T> : MonoBehaviour
{
    [SerializeField] private VirtualWorkspace ws;

    protected IObservable<T> sensorData = Observable.Empty<T>();

    private void Awake() {
        ws = ws ? ws : FindFirstObjectByType<VirtualWorkspace>();
    }

    public IObservable<T> SensorData {
        get { return sensorData; }
    }

}

}

