using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueStickControl : MonoBehaviour {
	public CustomRigidBody cueStick;
	public float maxDistance = 10f;

	private CustomTransform _cueStickTransform;

	private CustomSpringJoint _spring;
	private CustomTransform _connectedTransform;
	
    // Use this for initialization
    void Start () {
        _cueStickTransform = cueStick.GetComponent<CustomTransform>();
		_spring = cueStick.GetComponent<CustomSpringJoint>();
		_connectedTransform = _spring.connectedBody.GetComponent<CustomTransform>();
		
		_spring.enabled = false;
		cueStick.useGravity = false;
    }

	void FixedUpdate() {
		bool leftButton = Input.GetMouseButton(0);
		
		if (leftButton) {
			_spring.enabled = false;

			Vector3 dragPos = Utils.ScreenToWorld(Input.mousePosition);
			Vector3 deltaPos = (dragPos - _connectedTransform.position);
			
			if (deltaPos.magnitude > maxDistance) {
				dragPos = _connectedTransform.position + deltaPos.normalized * maxDistance;
			}
			
			_cueStickTransform.position = dragPos;
			cueStick.velocity = Vector3.zero;
		}
		else {
			_spring.enabled = true;
		}
	}
}
