using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toto : MonoBehaviour {
	[SerializeField] private CustomTransform test;
	[SerializeField] private GameObject reference;

	void Start() {
		Vector3 axis = new Vector3(1, 0, 0);
		
		
		// float angle = 90;
		// test.Rotate(angle, angle, 0);

		// test.Project(axis);
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			test.GetComponent<CustomRigidBody>().AddForce(new Vector3(0.0f, 0.0f, 100.0f),
														  new Vector3(5.0f, 5.0f, 0));
		}
		// test.GetComponent<CustomRigidBody>().
		// Vector3 axis = new Vector3(1, 0, 0);
		// float angle = 90 * Time.deltaTime;
		// test.Rotate(axis, angle);
		
		// reference.transform.Rotate(axis, angle);
		
		// test.Translate(new Vector3(0, 0, 1) * Time.deltaTime);
		
		// Vector3 translation = Vector3.up * Time.deltaTime;

		// reference.transform.Translate(translation);
		// test.Translate(translation);
	}
}
