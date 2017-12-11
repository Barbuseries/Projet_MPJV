using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom Joint")]
[RequireComponent(typeof(CustomRigidBody))]
public class CustomSpringJoint : MonoBehaviour {

    public GameObject connectedBody;
    public GameObject self;
   
    public float spring;
    //public float damper;
    //public float minDistance;
    //public float maxDistance;
    // public float tolerance;

    private CustomTransform selfTransform;
    private CustomTransform connectTransform;
    private CustomRigidBody connectRigid;

    private Vector3 reposPos;

    private void Start()
    {
        selfTransform = self.GetComponent<CustomTransform>();
        connectTransform = connectedBody.GetComponent<CustomTransform>();
        connectRigid = connectedBody.GetComponent<CustomRigidBody>();

        reposPos = connectTransform.position;
    }

    void FixedUpdate () {

        Vector3 axis = reposPos - connectTransform.position;
        Vector3 springForce = spring * axis;
        connectRigid.AddForce(springForce);

    }
}
