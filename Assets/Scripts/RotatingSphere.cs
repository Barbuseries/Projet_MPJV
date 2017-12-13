using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSphere : MonoBehaviour {
	public float speed = 1.0f;
	public Vector3 axis = new Vector3(0, 0, 1);

	private CustomRigidBody _sphere;

	void Start() {
		_sphere = GetComponent<CustomRigidBody>();
	}
	
	void Update () {
		_sphere.angularVelocity = speed * axis.normalized;
	}
}
