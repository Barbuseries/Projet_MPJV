﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomCollider : MonoBehaviour{
	protected Shape _colliderShape;
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

		_gameWorld = GetComponentInParent<CustomGameWorld>();
	}
	
	public void setId(int id){
		_id = id;
	}
	
	public int getId(){
		return _id;
	}

	void FixedUpdate() {
		//update collider only if component is moving
		if (selfBody.velocity.magnitude != 0) {
			var closeColliderList = _gameWorld.getColliderList();
		
			foreach (CustomCollider currentCollider in closeColliderList) {
				//should not test itself
				if (this._id != currentCollider.getId ()) {
					Collide(this, currentCollider);
				}
			}
		}
	}

	void OnDestroy() {
		_gameWorld.RemoveCollider(this);
	}

	// TODO: A lot of code is reused (get rigid body, get transform)
	// Maybe make a resolve collision take those parameters (instead
	// of just colliders, we do not care about them anymore anyway).
	// TODO: Add shape inertia as parameter to handle collisions with
	// rotation.
	protected static void ResolveCollision(CustomCollider c1, CustomCollider c2, Vector3 contactNormal, float penetration) {
		CreateCollisionForce(c1, c2, contactNormal);
		HandlePenetration(c1, c2, contactNormal, penetration);
	}

	// In case tested is inside me, both of us back proportional to
	// our mass and the penetration.
	protected static void HandlePenetration(CustomCollider c1, CustomCollider c2, Vector3 contactNormal, float penetration) {
		if (penetration <= 0) return;

		CustomRigidBody r1 = c1.GetComponent<CustomRigidBody>();
		CustomRigidBody r2 = c2.GetComponent<CustomRigidBody>();
			
		float invM1 = 1 / r1.mass;
		float invM2 = 1 / r2.mass;

		CustomTransform t1 = r1.GetComponent<CustomTransform>();
		CustomTransform t2 = r2.GetComponent<CustomTransform>();

		Vector3 moveIPerMass = contactNormal * (-penetration / (invM1 + invM2));

		t1.position -= moveIPerMass * invM1;
		t2.position += moveIPerMass * invM2;
	}

	public static void CreateCollisionForce(CustomCollider c1, CustomCollider c2, Vector3 contactNormal){
		CustomRigidBody r1 = c1.GetComponent<CustomRigidBody>();
		CustomRigidBody r2 = c2.GetComponent<CustomRigidBody>();
			
		float m1 = r1.mass;
		float m2 = r2.mass;
		Vector3 v1 = r1.velocity;
		Vector3 v2 = r2.velocity;

		Vector3 vRel = v1 - v2;

		// FIXME: What do we decide to do? (as c1 collides with c2, we
		// currently use c1's restauration factor).
		Vector3 K = contactNormal * ((c1.restauration + 1) * Vector3.Dot(vRel, contactNormal)) / (1 / m1 + 1 / m2);
		
		r1.velocity = v1 - K/m1;
		r2.velocity = v2 + K/m2;
	}

	public static void CollideSphereSphere(CustomSphereCollider s1, CustomSphereCollider s2) {
		Vector3 s1Pos = s1.GetComponent<CustomTransform>().position;
		Vector3 s2Pos = s2.GetComponent<CustomTransform>().position;
		
		float radiusSum = s1.radius + s2.radius;
		float d = Vector3.Distance (s1Pos, s2Pos);

		//object collide
		if (d < radiusSum) {
			ResolveCollision(s1, s2, (s1Pos - s2Pos).normalized, (radiusSum - d));
		}
	}

	// FIXME: This does not work correctly in Billboard scene.
	public static void CollideSphereBox(CustomSphereCollider s1, CustomBoxCollider b1) {
		float radius = s1.radius;
		
		Vector3 s1Pos = s1.GetComponent<CustomTransform>().position;
		Vector3 b1Pos = b1.GetComponent<CustomTransform>().position;
		
		// Get the center of the sphere relative to the center of the box
		Vector3 relCenter = s1Pos - b1Pos;
		
		// Closest point
		Vector3 b1Point = Vector3.zero;
		Vector3 halfDim = b1.halfDim;
		float dist = 0.0f;

		//check sphere pos with the box on the X axis
		dist = relCenter.x;
		if (dist > halfDim.x) dist = halfDim.x;
		if (dist < -halfDim.x) dist = -halfDim.x;
		b1Point.x = dist;

		//same for Y
		dist = relCenter.y;
		if (dist > halfDim.y) dist = halfDim.y;
		if (dist < -halfDim.y) dist = -halfDim.y;
		b1Point.y = dist;

		//same for Z
		dist = relCenter.z;
		if (dist > halfDim.z) dist = halfDim.z;
		if (dist < -halfDim.z) dist = -halfDim.z;
		b1Point.z = dist;

		// Now we have the closest point on the box, to the sphere
		// So we check if it's less than the radius
		dist = (b1Point - relCenter).sqrMagnitude;
		if (dist > radius * radius) return;
		
		b1Point += b1Pos;
		float penetration = radius - Mathf.Sqrt(dist);
		ResolveCollision(b1, s1, (b1Point - s1Pos).normalized, penetration);
	}

	public static void CollideBoxBox(CustomBoxCollider b1, CustomBoxCollider b2) {
		Vector3 b1Pos = b1.GetComponent<CustomTransform>().position;
		Vector3 b2Pos = b2.GetComponent<CustomTransform>().position;
		
		Vector3 minB1 = b1.min;
		Vector3 maxB1 = b1.max;
		Vector3 minB2 = b2.min;
		Vector3 maxB2 = b2.max;

		bool xCol = (minB1.x <= maxB2.x && maxB1.x >= minB2.x);
		bool yCol = (minB1.y <= maxB2.y && maxB1.y >= minB2.y);
		bool zCol = (minB1.z <= maxB2.z && maxB1.z >= minB2.z);

		if (xCol && yCol && zCol) {
			Vector3 delta = new Vector3();
			
			delta.x = Mathf.Min (Mathf.Abs (maxB2.x - minB1.x), Mathf.Abs (maxB1.x - minB2.x));
			delta.y = Mathf.Min (Mathf.Abs (maxB2.y - minB1.y), Mathf.Abs (maxB1.y - minB2.y));
			delta.z = Mathf.Min (Mathf.Abs (maxB2.z - minB1.z), Mathf.Abs (maxB1.z - minB2.z));

			float minDelta;
			Vector3 axis = Vector3.zero;

			if ((delta.x < delta.y) && (delta.x < delta.z)) {
				minDelta = delta.x;
				axis.x = b1Pos.x - b2Pos.x;
			}
			else if ((delta.y < delta.x) && (delta.y < delta.z)) {
				minDelta = delta.y;
				axis.y = b1Pos.y - b2Pos.y;
			}
			else {
				minDelta = delta.z;
				axis.z = b1Pos.z - b2Pos.z;
			}

			axis.Normalize();
			ResolveCollision(b1, b2, axis, minDelta);
		}
	}

	// Ideally (code-size wise), use a lookup table based on c1's
	// and c2's types?
	public static void Collide(CustomCollider c1, CustomCollider c2) {
		if (c1 is CustomSphereCollider) {
			var s1 = c1 as CustomSphereCollider;
			
			if (c2 is CustomSphereCollider) {
				CollideSphereSphere(s1, c2 as CustomSphereCollider);
			}
			else if (c2 is CustomBoxCollider) {
				CollideSphereBox(s1, c2 as CustomBoxCollider);
			}
			else {
				Debug.LogError ("Unknow collider (Sphere - ???)");
			}
		}
		else if (c1 is CustomBoxCollider){
			var b1 = c1 as CustomBoxCollider;
			
			if (c2 is CustomSphereCollider) {
				CollideSphereBox(c2 as CustomSphereCollider, b1);
			}
			else if (c2 is CustomBoxCollider) {
				CollideBoxBox(b1, c2 as CustomBoxCollider);
			}
			else {
				Debug.LogError ("Unknow collider (Box - ???)");
			}
		}
		else {
			Debug.LogError ("Unknow collider");
			return;
		}
	}
}
