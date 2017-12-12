using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomRigidBody))]
public abstract class CustomJoint : MonoBehaviour {
    public CustomRigidBody connectedBody;
    
    protected CustomTransform selfTransform;
    protected CustomTransform connectedTransform;

	protected CustomRigidBody selfBody;

    private void Start()
    {
        selfTransform = gameObject.GetComponent<CustomTransform>();
        connectedTransform = connectedBody.GetComponent<CustomTransform>();

		selfBody = gameObject.GetComponent<CustomRigidBody>();

		InitJoint();
    }

	abstract protected void InitJoint();
}
