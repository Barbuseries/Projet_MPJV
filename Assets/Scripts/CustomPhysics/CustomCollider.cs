using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomCollider : MonoBehaviour{
	
	protected Shape _colliderShape;
	protected CustomCollider[] _closeColliderList;// get every collider for now
	protected CustomGameWorld _gameWorld;
	protected int _id;
	public bool collideEnable =false;

	public void setId(int id){
		_id = id;
	}
	public int getId(){
		return _id;
	}

	public void updateCloseColliderList(){
		CustomGameWorld gameWorld = GetComponentInParent<CustomGameWorld>();
		_closeColliderList =gameWorld.getColliderList ();
		//test
		if (_id == 0) {
			CustomRigidBody r1 = GetComponent<CustomRigidBody> ();
			Vector3 K = new Vector3 (0, -0.5f, -0.5f);
			r1.AddForce (K);
			Debug.Log ("test");
		}
	}

	public abstract void isCollidingWithSphere(CustomSphereCollider s);
	public abstract void isCollidingWithBox();
}