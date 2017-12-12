using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom Spring Joint")]
[RequireComponent(typeof(CustomRigidBody))]
public class CustomSpringJoint : CustomJoint {
    public float spring = 1;
    //public float minDistance;
    //public float maxDistance;
    // public float tolerance;

    private float restDistance;

    override protected void InitJoint() {
        restDistance = (connectedTransform.position - selfTransform.position).magnitude;
    }

    void FixedUpdate () {
        Vector3 axis = connectedTransform.position - selfTransform.position;
        Vector3 springForce = spring * axis * (restDistance - axis.magnitude);

        connectedBody.AddForce(springForce);
		// NOTE: As per Unity, the default behaviour is to make both
		// rigidbodies move.
		// (Except if one is immobile)
		selfBody.AddForce(-springForce);
    }
}
