using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBoxCollider : CustomCollider {

	public float width;
	public float height;
	public float depth;


	public void Start(){
	}

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

		float radius = tested.radius;
		Vector3 spherePos = tested.GetComponent<CustomTransform> ().position;
		Vector3 boxPos = this.GetComponent<CustomTransform> ().position;

		// Get the center of the sphere relative to the center of the box
		Vector3 sphereCenterRelBox = spherePos - boxPos;
		Vector3 boxPoint =new Vector3();

		//check sphere pos with the box on the X axis
		if (sphereCenterRelBox.x < -this.width/2.0f)
			boxPoint.x = -this.width/2.0f;
		else if (sphereCenterRelBox.x > this.width/2.0f)
			boxPoint.x = this.width/2.0f;
		else
			boxPoint.x = sphereCenterRelBox.x;

		//same for Y
		if (sphereCenterRelBox.y < -this.height / 2.0f)
			boxPoint.y = -this.width / 2.0f;
		else if (sphereCenterRelBox.y > this.height / 2.0f)
			boxPoint.y = this.height/2.0f;
		else
			boxPoint.y = sphereCenterRelBox.y;

		//same for Z
		if (sphereCenterRelBox.z < -this.depth/2.0f)
			boxPoint.x = -this.depth/2.0f;
		else if (sphereCenterRelBox.z > this.depth/2.0f)
			boxPoint.z = this.depth/2.0f;
		else
			boxPoint.z = sphereCenterRelBox.z;
		// Now we have the closest point on the box, to the sphere
		// So we check if it's less than the radius

		float distBetweenSphereAndBox = (sphereCenterRelBox - boxPoint).magnitude;
		if (distBetweenSphereAndBox < radius)
			ResolveCollision (tested, radius -distBetweenSphereAndBox);
	}

	//box to box collide
	public override void isCollidingWithBox(CustomBoxCollider boxB){

		Vector3 aPos = this.GetComponent<CustomTransform> ().position;
		Vector3 bPos = boxB.GetComponent<CustomTransform> ().position;//doesn't seems to help
		Vector3 minA = new Vector3(aPos.x - width, aPos.y - height, aPos.z - depth);
		Vector3 maxA = new Vector3(aPos.x + width, aPos.y + height, aPos.z + depth);
		Vector3 minB = new Vector3(1 + bPos.x - boxB.width, bPos.y - boxB.height, 1 + bPos.z - boxB.depth);
		Vector3 maxB = new Vector3(1 + bPos.x + boxB.width, bPos.y + boxB.height, 1 + bPos.z + boxB.depth);

		bool xCol = (minA.x <= maxB.x && maxA.x >= minB.x);
		bool yCol = (minA.y <= maxB.y && maxA.y >= minB.y);
		bool zCol = (minA.z <= maxB.z && maxA.z >= minB.z);


		if (xCol && yCol && zCol) {
			Vector3 dist = new Vector3 ();
			dist.x = Mathf.Min (Mathf.Abs (maxB.x - minA.x), Mathf.Abs (maxA.x - minB.x));
			dist.y = Mathf.Min (Mathf.Abs (maxB.y - minA.y), Mathf.Abs (maxA.y - minB.y));
			dist.z = Mathf.Min (Mathf.Abs (maxB.z - minA.z), Mathf.Abs (maxA.z - minB.z));
			ResolveCollision (boxB,  dist.magnitude);
		}
			
	}



}
