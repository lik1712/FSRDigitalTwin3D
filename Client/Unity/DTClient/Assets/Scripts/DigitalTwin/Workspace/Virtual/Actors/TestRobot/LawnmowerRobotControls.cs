using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSR.Workspace.Virtual.Sensor;
using System;
using UniRx;

namespace FSR.Workspace.Virtual.Actor {

/**
 * Provides naive controls for a lawnmower robot
 *
 * IS NOT INTENDED TO BE IMPROVED NOR USED IN LATER VERSIONS
 */
public class LawnmowerRobotControls : MonoBehaviour {
    
    [SerializeField] private CollisionSensorSource collisionSensor;
    [SerializeField] private float maxVelocity = 1.0f;
    [SerializeField] private float force = 5.0f;
    private Rigidbody rigidBody;
    private float timer = 0.0f;

    enum State {
        MOWING, BACKWARDS, TURNING
    }
    private State state;

    private void Start() {
        rigidBody = GetComponent<Rigidbody>();
        state = State.MOWING;

        collisionSensor.SensorData
            .Where((_) => state == State.MOWING )
            .Subscribe((_) => { state = State.BACKWARDS; })
            .AddTo(this);
    }

    private void Update() {
        if (state == State.MOWING) {
            MowingState();
        }
        else if (state == State.BACKWARDS) {
            BackwardsState();
        }
        else {
            TurningState();
        }
    }

    private void BackwardsState()
    {
        if (rigidBody.velocity.magnitude < 0.5 * maxVelocity) {
            rigidBody.AddForce(force * transform.TransformVector(Vector3.back));
        }

        timer += Time.deltaTime;
        if (timer >= 4.0f) {
            timer = 0;
            state = State.TURNING; 
        }
    }

    private void TurningState() {
        if (rigidBody.angularVelocity.magnitude < 0.5f) {
            rigidBody.AddTorque(0, 0.5f * force, 0);
        }

        timer += Time.deltaTime;
        if (timer >= 1.0f) {
            timer = 0;
            state = State.MOWING; 
        }
    }
    
    private void MowingState() {
        if (rigidBody.velocity.magnitude < maxVelocity) {
            rigidBody.AddForce(force * transform.TransformVector(Vector3.forward));
        }
    }
    
}

};