using System.Collections;
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
	private static float _damping = 0.90f;
	
	void Start () {
		_behavior = GetComponent<CustomBehavior>();

		shape = new Sphere(gameObject.transform.localScale.x, mass);

		Debug.Log(shape.inertia);
	}
	
	void FixedUpdate () {
		if (_inverseMass == 0) return;

		// Linear
		Vector3 acceleration = _forceAccumulator / GetMass();

		if (useGravity) acceleration += _gravity * gravityFactor;
		
		velocity += 0.5f * acceleration * Time.deltaTime;
		Vector3 deltaPos = velocity * Time.deltaTime;
		
		_behavior.Translate(deltaPos, Space.World);
 
		float damp = Mathf.Pow(_damping, Time.deltaTime); 
		velocity *= damp;


		// Angular
		Vector3 angularAcceleration = shape.inertia.inverse.MultiplyPoint3x4(_torque);
				
		angularVelocity += 0.5f * angularAcceleration * Time.deltaTime;
		Vector3 deltaAngle = angularVelocity * Time.deltaTime;
		_behavior.Rotate(deltaAngle.x, deltaAngle.y, deltaAngle.z);

		angularVelocity *= damp;
		
		
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
