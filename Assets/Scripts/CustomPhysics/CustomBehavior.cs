using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomPhysics/Custom Behavior")]
public class CustomBehavior : MonoBehaviour {
	[SerializeField] private GameObject[] vertices;

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
	
	public void Rotate(Vector3 axis, float angle) {
		if (vertices.Length == 0) return;
		
		Matrix4x4 rotationMatrix = _GetRotationMatrix(axis, angle);
		Vector3 angleRotation = _GetRotationAlongAxes(rotationMatrix);

		for (var i = 0; i < vertices.Length; ++i) {
			GameObject vertex = vertices[i];
			Vector3 relativePos = vertex.transform.position - transform.position;
			
			Vector4 relativePos4D = new Vector4(relativePos.x, relativePos.y, relativePos.z, 1);
			Vector4 result = rotationMatrix * relativePos4D;
		
			vertex.transform.position = new Vector3(result.x, result.y, result.z) + transform.position;

			// Converted to quaternion to avoid Gimbal Lock on x-axis.
			vertex.transform.rotation *= Quaternion.Euler(angleRotation.x, angleRotation.y, angleRotation.z);
		}
	}

	public void Translate(Vector3 translation, Space referential) {
		if (vertices.Length == 0) return;

		if (referential == Space.World) {
			transform.position += translation;
		}
		else {
			Quaternion rotation = vertices[0].transform.rotation;
			
			transform.position += rotation * translation;
		}
	}
}
