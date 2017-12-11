using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSphereCollider : CustomCollider  {

	public float radius;
	private bool _firstLoop;
	private Vector3 position;

	public void Start(){
		// enable collision if there is a rigidBody
		if (collideEnable && GetComponent<CustomRigidBody>() != null)
			collideEnable =true;
	}

	public void Update(){
		if (_closeColliderList == null) //if the start of gameWorld did not end
			updateCloseColliderList ();
		else {
			//update collider only if component is moving
			if (GetComponent<CustomRigidBody> ().velocity.magnitude != 0) {
				foreach (CustomCollider currentCollider in _closeColliderList) {
					//should not test itself
					if (this._id != currentCollider.getId ()) {
						if (currentCollider.GetType () == typeof(CustomSphereCollider)) {
							isCollidingWithSphere ((CustomSphereCollider)currentCollider);
						} else {
							Debug.LogError ("Unknow collider");
							return;
						}
					}
				}
			}
		}

	}
		

	//sphere to sphere collide
	public override void isCollidingWithSphere(CustomSphereCollider tested){
		Vector3 currentPos = this.transform.position;
		Vector3 testedPos = tested.transform.position;
		float radiusSum = tested.radius + radius;
		float d = Vector3.Distance (currentPos, testedPos);
		//object collide
		if (d < radiusSum) {
			createCollisionForce (tested);
		}
	}

	//sphere to box collide
	public override void isCollidingWithBox(){
	}


	public void createCollisionForce(CustomCollider tested){
		//if both object have a rigidBody
		if (collideEnable && tested.collideEnable) {
			CustomRigidBody r1 = GetComponent<CustomRigidBody> ();
			CustomRigidBody r2 = tested.GetComponent<CustomRigidBody> ();
			float e = 0f;
			float m1 = r1.mass;
			float m2 = r2.mass;
			Vector3 v1 = r1.velocity;
			Vector3 v2 = r2.velocity;
			Vector3 vRel = new Vector3 (Mathf.Abs (r1.velocity.x - r2.velocity.x),
						                Mathf.Abs (r1.velocity.y - r2.velocity.y),
						                Mathf.Abs (r1.velocity.z - r2.velocity.z));
			float Kx = ((e + 1) * vRel.x) / (1 / m1 + 1 / m2);
			float Ky = ((e + 1) * vRel.y) / (1 / m1 + 1 / m2);
			float Kz = ((e + 1) * vRel.z) / (1 / m1 + 1 / m2);
			Vector3 K = new Vector3 (Kx, Ky, Kz);

			r1.velocity = v1 + K/m1;
			r2.velocity = v2 - K/m2;
		}
	}
}
