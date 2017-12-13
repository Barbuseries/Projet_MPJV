using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomTransform))]
public class RainingSpheres : MonoBehaviour {
	public CustomTransform obj;
	
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
			if (Input.GetMouseButton(0)) {
				CustomTransform newObject = Instantiate(obj);
				CustomCollider newCollider = newObject.gameObject.AddComponent<CustomSphereCollider>();
				Vector3 spawnPos = Utils.ScreenToWorld(Input.mousePosition);
				
				newObject.transform.SetParent(_gameWorld.transform);
				newObject.position = spawnPos;

				float lifeTime = 6f;
				Destroy(newObject.gameObject, lifeTime);

				yield return new WaitForSeconds(1.0f / rate);
			}
			else {
				yield return new WaitForSeconds(Time.deltaTime);
			}
		}
	}
}
