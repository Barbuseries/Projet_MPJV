using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom Sphere Collider")]
[RequireComponent(typeof(CustomRigidBody))]
public class CustomSphereCollider : CustomCollider  {

	public float radius = 0.5f; // TODO: Maybe find a way to set this
								// to the scale (directly when it is
								// added in the editor, not at start
								// or awake)
}
