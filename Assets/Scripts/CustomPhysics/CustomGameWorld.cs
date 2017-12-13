using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGameWorld : MonoBehaviour {

	private List<CustomCollider> _colliderList; // should be something more avanced than a simple list


	public void Start(){
		FetchAllColliders();
	}

	public void Update(){
		FetchAllColliders();
	}

	public List<CustomCollider> getColliderList(){
		return _colliderList;
	}

	private void FetchAllColliders() {
		_colliderList = new List<CustomCollider>(gameObject.GetComponentsInChildren<CustomCollider>());
		
		int id = 0;
		foreach (CustomCollider currentCollider in _colliderList) {
			//set an id for the collider
			//for now only usefull to not make the object collide with itself
			currentCollider.setId (id);
			id += 1;
		}
	}

	public void RemoveCollider(CustomCollider item) {
		_colliderList.Remove(item);
	}
}
