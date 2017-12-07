using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom Behavior")]
public class CustomBehavior : MonoBehaviour {
	// FIXME: Need to store position, rotation, scale (and rename
	//        script as CustomTransform or something).
	
	// NOTE(rotation): We currently do not store the object's rotation
	//                 (nor its position).
	//
	//                 We do not change the object's rotation directly
	//                 (via transform.rotation), because that would
	//                 make our own rotation methods useless.
	//
	//                 Our temporary fix is to use the rotation of the
	//                 object's vertices (that we manually modify) to
	//                 indicate the rotation of the object as a whole.
	//
	//                 A better alternative would be to actually store
	//                 its position and its rotation (different from
	//                 transform.position/rotation). But by doing so,
	//                 we would need to add a Start, Update, ...
	//                 method to save (from transform) and copy
	//                 back (to transform) some changes (save all, but
	//                 only overwrite the position (as this is the
	//                 only thing that vertices do not only have as
	//                 'local')).
	
	[SerializeField] private GameObject[] vertices;

	// Draw lines between vertices.
	private LineRenderer line;

	void Start() {
		line = gameObject.AddComponent<LineRenderer>();

		line.SetWidth(0.05f, 0.05f);
		line.SetVertexCount(vertices.Length * ((vertices.Length * 2 - 1) - 1));
	}

	void Update() {
		if (vertices.Length < 2) return;


		var count = 0;
		for (var i = 0; i < vertices.Length; ++i) {
			for (var j = 0; j < vertices.Length; ++j) {
				if (i == j) continue;

				line.SetPosition(count, vertices[i].transform.position);
				++count;
				
				line.SetPosition(count, vertices[j].transform.position);
				++count;
			}
		}
	}

	private Matrix4x4 _GetRotationMatrix(Vector3 axis, float angle) {
		angle *= Mathf.Deg2Rad;
		axis.Normalize();

		// // https://en.wikipedia.org/wiki/Rotation_matrix
		float ca = Mathf.Cos(angle);
		float sa = Mathf.Sin(angle);
		float oneMCa = 1 - ca;

		float x = axis.x;
		float y = axis.y;
		float z = axis.z;

		float xSa = x * sa;
		float ySa = y * sa;
		float zSa = z * sa;

		float xxCa = x * x * oneMCa;
		float yyCa = y * y * oneMCa;
		float zzCa = z * z * oneMCa;

		float xyCa = x * y * oneMCa;
		float xzCa = x * z * oneMCa;
		float yzCa = y * z * oneMCa;
		
		Vector4 column0 = new Vector4(ca + xxCa, xyCa + zSa, xzCa - ySa, 0);
		Vector4 column1 = new Vector4(xyCa - zSa, ca + yyCa, yzCa + xSa, 0);
		Vector4 column2 = new Vector4(xzCa + ySa, yzCa - xSa, ca + zzCa, 0);
		Vector4 column3 = new Vector4(0, 0, 0, 1);

		Matrix4x4 result = new Matrix4x4(column0, column1, column2, column3);

		return result;
	}

	private Matrix4x4 _GetScalingMatrix(Vector3 axis, float k) {
		float x = axis.x;
		float y = axis.y;
		float z = axis.z;

		float kM1 = k - 1;

		float xxKM1 = x * x * kM1;
		float yyKM1 = y * y * kM1;
		float zzKM1 = z * z * kM1;

		float xyKM1 = x * y * kM1;
		float xzKM1 = x * z * kM1;
		float yzKM1 = y * z * kM1;
		
		Vector4 column0 = new Vector4(1 + xxKM1, xyKM1, xzKM1, 0);
		Vector4 column1 = new Vector4(xyKM1, 1 + yyKM1, yzKM1, 0);
		Vector4 column2 = new Vector4(xzKM1, yzKM1, 1 + zzKM1, 0);
		Vector4 column3 = new Vector4(0, 0, 0, 1);
		
		Matrix4x4 result = new Matrix4x4(column0, column1, column2, column3);

		return result;
	}

	private Vector3 _GetRotationAlongAxes(Matrix4x4 rotationMatrix) {
		float m00 = rotationMatrix.m00;
		float m10 = rotationMatrix.m10;
		float m20 = rotationMatrix.m20;
		float m21 = rotationMatrix.m21;
		float m22 = rotationMatrix.m22;
		
		Vector3 result = new Vector3(Mathf.Atan2(m21, m22),
									 Mathf.Asin(-m20),
									 Mathf.Atan2(m10, m00)) * Mathf.Rad2Deg;

		return result;
	}

	private void _Rotate(Matrix4x4 rotationMatrix) {
		Vector3 angleRotation = _GetRotationAlongAxes(rotationMatrix);

		Quaternion rotationQuaternion = Quaternion.Euler(angleRotation.x, angleRotation.y, angleRotation.z);

		foreach (GameObject vertex in vertices) {
			Vector3 relativePos = vertex.transform.position - transform.position;
			
			Vector4 relativePos4D = new Vector4(relativePos.x, relativePos.y, relativePos.z, 1);
			Vector4 result = rotationMatrix * relativePos4D;
		
			vertex.transform.position = new Vector3(result.x, result.y, result.z) + transform.position;

			// Converted to quaternion to avoid Gimbal Lock on x-axis.
			// NOTE: This is mainly done to be able to see the vertex' referential move.
			//       This is also used to determine the object rotation.
			//       (See NOTE about rotation)
			vertex.transform.rotation *= rotationQuaternion;
		}
	}

	// FIXME: The scale need to be stored somewhere (and position must
	//        not be modified).
	//        Otherwhise, there is not way to set the scale back to
	//        (1, 1, 1).
	private void _Scale(Matrix4x4 scalingMatrix) {
		Debug.Log(scalingMatrix);
		
		foreach (GameObject vertex in vertices) {
			Vector3 relativePos = vertex.transform.position - transform.position;
			
			Vector4 relativePos4D = new Vector4(relativePos.x, relativePos.y, relativePos.z, 1);
			Vector4 result = scalingMatrix * relativePos4D;
		
			vertex.transform.position = new Vector3(result.x, result.y, result.z) + transform.position;
		}
	}
	
	public void Rotate(Vector3 axis, float angle) {
		if (vertices.Length == 0) return;
		
		// FIXME: This can just use a quaternion.
		Matrix4x4 rotationMatrix = _GetRotationMatrix(axis, angle);
		
		_Rotate(rotationMatrix);
	}

	public void Rotate(float x, float y, float z) {
		Matrix4x4 fullRotation = (_GetRotationMatrix(new Vector3(1, 0, 0), x) *
								  _GetRotationMatrix(new Vector3(0, 1, 0), y) *
								  _GetRotationMatrix(new Vector3(0, 0, 1), z));

		_Rotate(fullRotation);
	}

	public void Translate(Vector3 translation, Space referential = Space.World) {
		if (vertices.Length == 0) return;

		if (referential == Space.World) {
			transform.position += translation;
		}
		else {
			Quaternion rotation = vertices[0].transform.rotation;
			
			transform.position += rotation * translation;
		}
	}

	public void Scale(Vector3 axis, float k) {
		if (vertices.Length == 0) return;
		
		Matrix4x4 scalingMatrix = _GetScalingMatrix(axis, k);

		_Scale(scalingMatrix);
	}

	public void Scale(float x, float y, float z) {
		Matrix4x4 fullScaling = (_GetScalingMatrix(new Vector3(1, 0, 0), x) *
								 _GetScalingMatrix(new Vector3(0, 1, 0), y) *
								 _GetScalingMatrix(new Vector3(0, 0, 1), z));

		_Scale(fullScaling);
	}

	public void Reflect(Vector3 axis) {
		Scale(axis, -1);
	}

	public void Project(Vector3 normal) {
		Scale(normal, 0);
	}
}
