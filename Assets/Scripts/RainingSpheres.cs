using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomTransform))]
public class RainingSpheres : MonoBehaviour {
	public CustomTransform obj;
	
	public float rate = 1f;

	private CustomTransform _customTransform;
	private CustomGameWorld _gameWorld;

	private Vector3 _originalPosition;
	
    // Use this for initialization
    void Start () {
        _customTransform = GetComponent<CustomTransform>();
		_gameWorld = FindObjectOfType<CustomGameWorld>();
		_originalPosition = new Vector3(0, 10, 0);
		
		StartCoroutine(SpawnWave());
    }

	IEnumerator SpawnWave() {
		for (;;) {
			CustomTransform newObject = Instantiate(obj);
			CustomCollider newCollider = newObject.gameObject.AddComponent<CustomSphereCollider>();
				
			newObject.transform.SetParent(_gameWorld.transform);
			newObject.position = _originalPosition + new Vector3(Random.Range(-2, 2), 0, 0);
			Debug.Log(newObject.position);
			Debug.Log(1.0f / rate);

			yield return new WaitForSeconds(1.0f / rate);
		}
	}
}
