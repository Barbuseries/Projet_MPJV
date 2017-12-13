using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom Box Collider")]
[RequireComponent(typeof(CustomRigidBody))]
public class CustomBoxCollider : CustomCollider {
	public float width = 1f;
	public float height = 1f;
	public float depth = 1f;

	public Vector3 halfDim {
		get {
			return 0.5f * new Vector3(width, height, depth);
		}
	}

	public Vector3 min {
		get {
			return selfTransform.position - halfDim;
		}
	}

	public Vector3 max {
		get {
			return selfTransform.position + halfDim;
		}
	}
}
