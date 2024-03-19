using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Sensors {

public class CollisionSensorSource : SensorSource
{

    [SerializeField] private ObservableCollisionTrigger trigger;

    private void Awake() {
        sensorData = trigger.OnTriggerEnterAsObservable()
            .Select((_) => new Hashtable());
    }

    
}

}


