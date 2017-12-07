﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom RigidBody")]
[RequireComponent(typeof(CustomBehavior))]
public class CustomRigidBody : MonoBehaviour {
	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 angularVelocity;

	[SerializeField] // NOTE/@HACK: [...] => This is used to have access to
					 // a field in the editor.
	private float _mass = 1.0f;

	public float mass {
		get
		{
			return _mass;
		}
		set
		{
			if (value <= 0) {
				Debug.LogError("Set mass: mass must be > 0.");
				return;
			}

			_mass = value;
			_inverseMass = 1.0f / value;

			shape.mass = _mass;
		}
	}
	
	public bool useGravity = true;
	public float gravityFactor = 1.0f;

	public Shape shape {get; private set;}

	private float _inverseMass = 1.0f;
	private Vector3 _forceAccumulator;

	private Vector3 _torque;
	
	private CustomBehavior _behavior;

	private static Vector3 _gravity = new Vector3(0, -9.8f, 0);

	// TODO: Replace by (and implement) drag
	private static float _linearDamping = 0.90f;
	private static float _angularDamping = 0.90f;
	
	void Start () {
		// Used to move and rotate the parent entity
		_behavior = GetComponent<CustomBehavior>();

		// TODO?: We do not currently store the entity's scale in CustomBehavior. 
		// shape = new Sphere(gameObject.transform.localScale.x, mass);
		shape = new Cube(gameObject.transform.localScale.x * 2,
						 gameObject.transform.localScale.y * 2,
						 gameObject.transform.localScale.z * 2,
						 mass);
	}
	
	void FixedUpdate () {
		// Body is immobile
		if (_inverseMass == 0) return;

		float linearDamping = Mathf.Pow(_linearDamping, Time.deltaTime);
		float angularDamping = Mathf.Pow(_angularDamping, Time.deltaTime);

		// Linear
		Vector3 acceleration = _forceAccumulator * _inverseMass;
		if (useGravity) acceleration += _gravity * gravityFactor;
		
		velocity += acceleration * Time.deltaTime;
		velocity *= linearDamping;
		
		Vector3 deltaPos = velocity * Time.deltaTime;
		_behavior.Translate(deltaPos, Space.World);
 

		// Angular
		Vector3 angularAcceleration = shape.inertia.inverse.MultiplyPoint3x4(_torque);
				
		angularVelocity += angularAcceleration * Time.deltaTime;
		angularVelocity *= angularDamping;
		
		Vector3 deltaAngle = angularVelocity * Time.deltaTime;
		_behavior.Rotate(deltaAngle.x, deltaAngle.y, deltaAngle.z);
		
		_forceAccumulator = Vector3.zero;
		_torque = Vector3.zero;
	}

	public void AddForce(Vector3 force) {
		_forceAccumulator += force;
	}

	public void AddForce(Vector3 force, Vector3 relativePosition) {
		_forceAccumulator += force;

		// TODO: Define a center of mass;
		Vector3 centerOfMass = Vector3.zero;
		
		_torque += Vector3.Cross((relativePosition - centerOfMass), force);
	}

	public void SetImmovable(bool state = true) {
		_inverseMass = (state) ? 0 : (1.0f / _mass);
	}

	public float GetMass() {
		if (_inverseMass == 0) return float.MaxValue;
		return mass;
	}
}
