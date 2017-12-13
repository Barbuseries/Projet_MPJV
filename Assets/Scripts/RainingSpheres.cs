using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomTransform))]
public class RainingSpheres : MonoBehaviour {
	public CustomTransform obj;
	public CustomTransform obj2;
	
	public float rate = 1f;

	private CustomGameWorld _gameWorld;

	private Vector3 _originalPosition;
	
    // Use this for initialization
    void Start () {
		_gameWorld = FindObjectOfType<CustomGameWorld>();
		_originalPosition = new Vector3(0, 10, 0);
		
		StartCoroutine(SpawnWave());
    }

	IEnumerator SpawnWave() {
		for (;;) {
			bool leftButton = Input.GetMouseButton(0);
			bool rightButton = Input.GetMouseButton(1);
			
			if (leftButton || rightButton) {
				CustomTransform newObject = leftButton ? Instantiate(obj) : Instantiate(obj2);
				CustomCollider newCollider = newObject.gameObject.AddComponent<CustomSphereCollider>();
				Vector3 spawnPos = Utils.ScreenToWorld(Input.mousePosition);
				
				newObject.transform.SetParent(_gameWorld.transform);
				newObject.position = spawnPos;
				newObject.GetComponent<CustomRigidBody>().velocity = Vector3.zero;

				float lifeTime = 4f;
				Destroy(newObject.gameObject, lifeTime);

				yield return new WaitForSeconds(1.0f / rate);
			}
			else {
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}
	}
}
