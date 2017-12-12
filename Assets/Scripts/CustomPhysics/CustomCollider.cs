using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomCollider : MonoBehaviour{
	
	protected Shape _colliderShape;
	protected CustomCollider[] _closeColliderList;// get every collider for now
	protected CustomGameWorld _gameWorld;

	protected CustomRigidBody selfBody;
	protected CustomTransform selfTransform;

	protected int _id;
	public bool collideEnable = true;
	public float restauration = 0.5f; // TODO: Find a reasonable
									  // default value (if any...).

	public void Start(){
		selfBody = GetComponent<CustomRigidBody>();
		selfTransform = GetComponent<CustomTransform>();
	}
	
	public void setId(int id){
		_id = id;
	}
	
	public int getId(){
		return _id;
	}

	public void updateCloseColliderList(){
		CustomGameWorld gameWorld = GetComponentInParent<CustomGameWorld>();
		_closeColliderList = gameWorld.getColliderList ();
	}

	public abstract void isCollidingWithSphere(CustomSphereCollider s);
	public abstract void isCollidingWithBox();

	// TODO: Add shape inertia as parameter to handle collisions with
	// rotation.
	protected void ResolveCollision(CustomCollider tested, float penetration) {
		createCollisionForce(tested);
		handlePenetration(tested, penetration);
	}

	// In case tested is inside me, both of us back proportional to
	// our mass and the penetration.
	protected void handlePenetration(CustomCollider tested, float penetration) {
		if (penetration <= 0) return;

		CustomRigidBody r1 = selfBody;
		CustomRigidBody r2 = tested.GetComponent<CustomRigidBody>();
			
		float invM1 = 1 / r1.mass;
		float invM2 = 1 / r2.mass;

		// FIXME: This fails to work if both object have the same
		// velocity (restauration = 0).
		Vector3 contactNormal = selfTransform.position - r2.GetComponent<CustomTransform>().position;
		contactNormal.Normalize();

		Vector3 moveIPerMass = contactNormal * (-penetration / (invM1 + invM2));

		selfTransform.position -= moveIPerMass * invM1;
		r2.GetComponent<CustomTransform>().position += moveIPerMass * invM2;
	}

	public void createCollisionForce(CustomCollider tested){
		CustomRigidBody r1 = selfBody;
		CustomRigidBody r2 = tested.GetComponent<CustomRigidBody>();
		
		float m1 = r1.mass;
		float m2 = r2.mass;
		Vector3 v1 = r1.velocity;
		Vector3 v2 = r2.velocity;

		Vector3 contactNormal = selfTransform.position - r2.GetComponent<CustomTransform>().position;
		contactNormal.Normalize();
		
		Vector3 vRel = v1 - v2;

		Vector3 K = contactNormal * ((restauration + 1) * Vector3.Dot(vRel, contactNormal)) / (1 / m1 + 1 / m2);
		
		r1.velocity = v1 - K/m1;
		r2.velocity = v2 + K/m2;
	}
}
