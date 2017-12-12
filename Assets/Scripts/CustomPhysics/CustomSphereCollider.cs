using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom Sphere Collider")]
[RequireComponent(typeof(CustomRigidBody))]
public class CustomSphereCollider : CustomCollider  {

	public float radius;
	private Vector3 position;

	public void FixedUpdate(){
		if (_closeColliderList == null) //if the start of gameWorld did not end
			updateCloseColliderList ();
		else {
			//update collider only if component is moving
			if (selfBody.velocity.magnitude != 0) {
				foreach (CustomCollider currentCollider in _closeColliderList) {
					//should not test itself
					if (this._id != currentCollider.getId ()) {
						if (currentCollider is CustomSphereCollider) {
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
		Vector3 currentPos = selfTransform.position;
		Vector3 testedPos = tested.GetComponent<CustomTransform>().position;
		float radiusSum = tested.radius + radius;
		float d = Vector3.Distance (currentPos, testedPos);

		//object collide
		if (d < radiusSum) {
			ResolveCollision(tested, (radiusSum - d));
		}
	}

	//sphere to box collide
	public override void isCollidingWithBox(){
	}
}
