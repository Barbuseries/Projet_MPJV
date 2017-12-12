using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom RigidBody")]
[RequireComponent(typeof(CustomTransform))]
public class CustomRigidBody : MonoBehaviour {
	[HideInInspector]
	public Vector3 velocity;
	[HideInInspector]
	public Vector3 angularVelocity;

	// FIXME: _inverseMass is not set if
	// we modify the mass from the editor while running.
	[SerializeField] // NOTE/@HACK: [...] => This is used to have access to
					 // a field in the editor.
	private float _mass = 1.0f;

	public float mass {
		get
		{
			return GetMass();
		}
		set
		{
			_SetMass(value);
		}
	}
	
	public bool useGravity = true;
	public float gravityFactor = 1.0f;

	public Shape shape {get; private set;}

	private float _inverseMass = 1.0f;
	private Vector3 _forceAccumulator;

	private Vector3 _torque;
	
	private CustomTransform _customTransform;

	private static Vector3 _gravity = new Vector3(0, -9.8f, 0);

	// TODO: Replace by (and implement) drag
	private static float _linearDamping = 0.90f;
	private static float _angularDamping = 0.90f;

	void Awake() {
		_SetMass(_mass);
	}
	
	void Start () {
		// Used to move and rotate the parent entity
		_customTransform = GetComponent<CustomTransform>();

		// If a custom primitive is given, try to find the closest
		// rigidbody.
		var primitive = gameObject.GetComponent<CustomPrimitive>();
		if (primitive == null) return;
		
		// Yep
		if (primitive is CustomSphere) {
			shape = new Ellipsoid(_customTransform.scale.x,
								  _customTransform.scale.y,
								  _customTransform.scale.z,
								  mass);
		}
		else { // Defaults to a cube, because why not?
			shape = new Cube(_customTransform.scale.x * 2,
							 _customTransform.scale.y * 2,
							 _customTransform.scale.z * 2,
							 mass);
		}
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
		_customTransform.Translate(deltaPos, Space.World);
 

		// Angular
		Vector3 angularAcceleration = shape.inertia.inverse.MultiplyPoint3x4(_torque);
				
		angularVelocity += angularAcceleration * Time.deltaTime;
		angularVelocity *= angularDamping;
		
		Vector3 deltaAngle = angularVelocity * Time.deltaTime;
		_customTransform.Rotate(deltaAngle.x, deltaAngle.y, deltaAngle.z);
		
		_forceAccumulator = Vector3.zero;
		_torque = Vector3.zero;
	}

	private void _SetMass(float mass) {
		if (mass <= 0) {
			Debug.LogError("Set mass: mass must be > 0.");
			return;
		}

		_mass = mass;
		_inverseMass = 1.0f / mass;

		if (shape != null) {
			shape.mass = _mass;
		}
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
		return _mass;
	}
}
