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
						if (currentCollider.GetType () == typeof(CustomSphereCollider)/*currentCollider is CustomSphereCollider*/) {//"is" doesn't seems to work with me
							isCollidingWithSphere ((CustomSphereCollider)currentCollider);
						}
						else if(currentCollider.GetType () == typeof(CustomBoxCollider)/*currentCollider is CustomBoxCollider*/){
							isCollidingWithBox((CustomBoxCollider)currentCollider);
						}
						else {
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
	public override void isCollidingWithBox(CustomBoxCollider box){
		
		Vector3 spherePos = this.GetComponent<CustomTransform>().position;
		Vector3 boxPos = box.GetComponent<CustomTransform>().position;
		// Get the center of the sphere relative to the center of the box
		Vector3 sphereCenterRelBox = spherePos - boxPos;
		Vector3 boxPoint =new Vector3();

		//check sphere pos with the box on the X axis
		if (sphereCenterRelBox.x < -box.width/2.0f)
			boxPoint.x = -box.width/2.0f;
		else if (sphereCenterRelBox.x > box.width/2.0f)
			boxPoint.x = box.width/2.0f;
		else
			boxPoint.x = sphereCenterRelBox.x;

		//same for Y
		if (sphereCenterRelBox.y < -box.height / 2.0f)
			boxPoint.y = -box.width / 2.0f;
		else if (sphereCenterRelBox.y > box.height / 2.0f)
			boxPoint.y = box.height/2.0f;
		else
			boxPoint.y = sphereCenterRelBox.y;

		//same for Z
		if (sphereCenterRelBox.z < -box.depth/2.0f)
			boxPoint.x = -box.depth/2.0f;
		else if (sphereCenterRelBox.z > box.depth/2.0f)
			boxPoint.z = box.depth/2.0f;
		else
			boxPoint.z = sphereCenterRelBox.z;

		// Now we have the closest point on the box, to the sphere
		// So we check if it's less than the radius

		float distBetweenSphereAndBox = (sphereCenterRelBox - boxPoint).magnitude;

		if (distBetweenSphereAndBox < radius) {
			Debug.Log (distBetweenSphereAndBox);
			ResolveCollision (box, radius -distBetweenSphereAndBox);
		}
	}
}
