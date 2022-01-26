using Normal.Realtime;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToyCar : ColoredObject
{
	[Serializable]
	public class Axle
	{
		public WheelCollider[] Wheels;
		public bool CanAccelerate;
		public bool CanBrake;
		public bool CanSteer;
	}

	public Axle[] Axles;

	[Header("Acceleration")]
	public float MaxForwardTorque;
	public float TorqueForwardAcceleration;
	public float MaxReverseTorque;
	public float TorqueReverseAcceleration;
	public float TorqueDeceleration;

	[Header("Braking")]
	public float MaxBrakeTorque;
	public float BrakeTorqueAcceleration;
	public float BrakeDeceleration;

	[Header("Steering")]
	public float MaxSteeringAngle;
	public float SteeringAngleAcceleration;
	public float SteeringAngleDeceleration;

	public ToyCarCamera CameraPrefab;
	public Rigidbody Rigidbody;
	public Transform CenterOfMass;
	public RealtimeView Root;

	float _currentMotorTorque;
	float _currentBrakeTorque;
	float _currentSteeringAngle;

	void Start()
	{
		Rigidbody.centerOfMass = CenterOfMass.localPosition;

		if (Root.isOwnedLocallyInHierarchy) {
			Root.RequestOwnership();

			var camera = Instantiate(CameraPrefab);
			camera.Initialise(this);

			var realtimeComponents = GetComponentsInChildren<IRealtimeComponent>();
			foreach (var component in realtimeComponents) {
				component.RequestOwnership();
			}
		}
	}

	protected override void Update()
	{
		base.Update();

		if (Keyboard.current[Key.W].IsPressed()) {
			_currentMotorTorque += TorqueForwardAcceleration * Time.deltaTime;
			_currentMotorTorque = Mathf.Clamp(_currentMotorTorque, MaxReverseTorque, MaxForwardTorque);
		} else if (Keyboard.current[Key.S].IsPressed()) {
			_currentMotorTorque += TorqueReverseAcceleration * Time.deltaTime;
			_currentMotorTorque = Mathf.Clamp(_currentMotorTorque, MaxReverseTorque, MaxForwardTorque);
		} else {
			_currentMotorTorque = Mathf.MoveTowards(_currentMotorTorque, 0f, Time.deltaTime * TorqueDeceleration);
		}

		if (Keyboard.current[Key.A].IsPressed()) {
			_currentSteeringAngle -= SteeringAngleAcceleration * Time.deltaTime;
			_currentSteeringAngle = Mathf.Clamp(_currentSteeringAngle, -MaxSteeringAngle, MaxSteeringAngle);
		} else if (Keyboard.current[Key.D].IsPressed()) {
			_currentSteeringAngle += SteeringAngleAcceleration * Time.deltaTime;
			_currentSteeringAngle = Mathf.Clamp(_currentSteeringAngle, -MaxSteeringAngle, MaxSteeringAngle);
		} else {
			_currentSteeringAngle = Mathf.MoveTowards(_currentSteeringAngle, 0f, Time.deltaTime * SteeringAngleDeceleration);
		}

		if (Keyboard.current[Key.Space].IsPressed()) {
			_currentBrakeTorque += BrakeTorqueAcceleration * Time.deltaTime;
			_currentBrakeTorque = Mathf.Clamp(_currentBrakeTorque, 0f, MaxBrakeTorque);
		} else {
			_currentBrakeTorque = Mathf.MoveTowards(_currentBrakeTorque, 0f, Time.deltaTime * BrakeDeceleration);
		}
	}

	void FixedUpdate()
	{
		foreach (var axle in Axles) {
			foreach (var wheel in axle.Wheels) {
				if (axle.CanAccelerate) {
					wheel.motorTorque = _currentMotorTorque;
				}

				if (axle.CanBrake) {
					wheel.brakeTorque = _currentBrakeTorque;
				}

				if (axle.CanSteer) {
					wheel.steerAngle = _currentSteeringAngle;
					wheel.transform.GetChild(0).localEulerAngles = new Vector3(0f, _currentSteeringAngle, 0f);
				}
			}
		}
	}
}