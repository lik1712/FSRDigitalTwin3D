using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Sensors {

// Rotational Joint with 1 DoF. Rotates around a single axis.
public class RevoluteJointSensorSource : JointSensorSource
{
    [SerializeField] private ArticulationBody jointBody;

    public override EJointType JointType => EJointType.REVOLUTE;
    public ReadOnlyReactiveProperty<float> Z => rot;
    public ArticulationBody JointBody => jointBody ?? throw new NullReferenceException();

    private ReadOnlyReactiveProperty<float> rot;
    
    private void Awake() {
        jointBody = jointBody != null ? jointBody : GetComponent<ArticulationBody>();

        // TODO Currently only senses target positions from controller
        sensorData = gameObject.UpdateAsObservable()
            // .DistinctUntilChanged()
            .Select(_ => new float[] { jointBody.xDrive.target });
        
        rot = new ReadOnlyReactiveProperty<float>(sensorData.Select(x => x[0]), jointBody.xDrive.target);
    }
}

} // END namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Sensors