using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Unity.Workspace.Digital;
using FSR.DigitalTwin.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Unity.Workspace.Virtual.Sensors;
using Unity.Robotics.UrdfImporter;
using UnityEngine;

namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Actors {

    /// <summary>
    /// A cobot is a simple kinematic (robot arm) consisting of (for now) rotational joints.
    /// Using a set of joints, the kinematic can be controlled and transformed internally and externally
    /// </summary>
    public class Cobot : DigitalTwinAsset
    {
        [SerializeField] private CobotKinematic cobotKinematic;
        [SerializeField] private bool disableCollision = false;

        public void SetJointRotation(int index, float z) {
            if (cobotKinematic.Joints[index].JointType == EJointType.REVOLUTE) {
                RevoluteJointSensorSource jointSensor = cobotKinematic.Joints[index] as RevoluteJointSensorSource;
                jointSensor.JointBody.SetDriveTarget(ArticulationDriveAxis.X, z);
            }
        }

        void Awake () {
            if (disableCollision) {
                foreach(UrdfCollisions cols in FindObjectsByType<UrdfCollisions>(FindObjectsSortMode.None)) cols.gameObject.SetActive(false);
            }
        }
    }

} // END namespace FSR.DigitalTwin.Unity.Workspace.Virtual.Actors 
