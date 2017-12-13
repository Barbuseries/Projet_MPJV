using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: This is meant to be an equivalent of the Transform of a
// GameObject.
[AddComponentMenu("CustomPhysics/Custom Transform")]
public class CustomTransform : MonoBehaviour {
	// Overwrite gameObject.transform.XXX
	// NOTE: This uses {get; set;} to mimic the behaviour of Unity
	// (e,g: not being able to modify position.X directly)
	public Vector3 position {get; set;}
	public Vector3 scale {get; set;}
	public Quaternion rotation {get; set;}

	void Awake() {
		position = gameObject.transform.position;
		rotation = gameObject.transform.rotation;
		scale = gameObject.transform.localScale;
	}
	
	void LateUpdate() {
		gameObject.transform.position = position;
		gameObject.transform.rotation = rotation;
		gameObject.transform.localScale = scale;
	}

	private Matrix4x4 _GetXRotationMatrix(float angle) {
		angle *= Mathf.Deg2Rad;

		float ca = Mathf.Cos(angle);
		float sa = Mathf.Sin(angle);
		
		Vector4 column0 = new Vector4(1, 0, 0, 0);
		Vector4 column1 = new Vector4(0, ca, sa, 0);
		Vector4 column2 = new Vector4(0, -sa, ca, 0);
		Vector4 column3 = new Vector4(0, 0, 0, 1);

		Matrix4x4 result = new Matrix4x4(column0, column1, column2, column3);
		return result;
	}

	private Matrix4x4 _GetYRotationMatrix(float angle) {
		angle *= Mathf.Deg2Rad;

		float ca = Mathf.Cos(angle);
		float sa = Mathf.Sin(angle);
		
		Vector4 column0 = new Vector4(ca, 0, sa, 0);
		Vector4 column1 = new Vector4(0, 1, 0, 0);
		Vector4 column2 = new Vector4(-sa, 0, ca, 0);
		Vector4 column3 = new Vector4(0, 0, 0, 1);

		Matrix4x4 result = new Matrix4x4(column0, column1, column2, column3);
		return result;
	}

	private Matrix4x4 _GetZRotationMatrix(float angle) {
		angle *= Mathf.Deg2Rad;

		float ca = Mathf.Cos(angle);
		float sa = Mathf.Sin(angle);
		
		Vector4 column0 = new Vector4(ca, -sa, 0, 0);
		Vector4 column1 = new Vector4(sa, ca, 0, 0);
		Vector4 column2 = new Vector4(0, 0, 1, 0);
		Vector4 column3 = new Vector4(0, 0, 0, 1);

		Matrix4x4 result = new Matrix4x4(column0, column1, column2, column3);
		return result;
	}

	private Matrix4x4 _GetRotationMatrix(Vector3 axis, float angle) {
		angle *= Mathf.Deg2Rad;
		axis.Normalize();

		// https://en.wikipedia.org/wiki/Rotation_matrix
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

	private Matrix4x4 _GetScalingMatrix(float x, float y, float z) {
		Vector4 column0 = new Vector4(x, 0, 0, 0);
		Vector4 column1 = new Vector4(0, y, 0, 0);
		Vector4 column2 = new Vector4(0, 0, z, 0);
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
		rotation *= rotationQuaternion;
	}

	private void _Scale(Matrix4x4 scalingMatrix) {
		scale = scalingMatrix.MultiplyVector(scale);
	}


	
// Public methods
	public void Rotate(Vector3 axis, float angle) {
		angle *= Mathf.Deg2Rad;
		axis.Normalize();

		float ca = Mathf.Cos(angle / 2);
		float sa = Mathf.Sin(angle / 2);
		
		axis *= sa;

		// NOTE: This is equivalent to Quaternion.AxisAngle(axis, angle)
		Quaternion rotationQuaternion = new Quaternion(axis.x, axis.y, axis.z, ca);
		rotation *= rotationQuaternion;
	}

	public void Rotate(float x, float y, float z) {
		Matrix4x4 fullRotation = (_GetXRotationMatrix(x) *
								  _GetYRotationMatrix(y) *
								  _GetZRotationMatrix(z));

		_Rotate(fullRotation);
	}

	public void Translate(Vector3 translation, Space referential = Space.World) {
		if (referential == Space.World) {
			position += translation;
		}
		else {
			position += rotation * translation;
		}
	}

	public void Scale(Vector3 axis, float k) {
		Matrix4x4 scalingMatrix = _GetScalingMatrix(axis, k);

		_Scale(scalingMatrix);
	}

	public void Scale(float x, float y, float z) {
		Matrix4x4 fullScaling = _GetScalingMatrix(x, y, z);

		_Scale(fullScaling);
	}

	public void Reflect(Vector3 axis) {
		Scale(axis, -1);
	}

	public void Project(Vector3 normal) {
		Scale(normal, 0);
	}

	// Return a transformation matrix corresponding to the current
	// rotation and scale.
	// (IMPORTANT: This does not take into account the position, hence
	// the 'Relative' part)
	public Matrix4x4 RelativeTransformMatrix() {
		Matrix4x4 result = (GetRotationMatrix() *
							_GetScalingMatrix(scale.x, scale.y, scale.z));

		return result;
	}

	// Inverse translation and rotation.
	// IMPORTANT: Does not work if scale is involved!
	public Vector3 InvertTransform(Vector3 v) {
		Vector3 axis;
		float angle;
		rotation.ToAngleAxis(out angle, out axis);

		Vector3 result = _GetRotationMatrix(axis, -angle).MultiplyVector(v - position);
		
		return result;
	}

	public Vector3 Transform(Vector3 v) {
		Vector3 result = (rotation * v) + position;
		return result;
	}

	public Matrix4x4 GetRotationMatrix() {
		Vector3 axis;
		float angle;
		rotation.ToAngleAxis(out angle, out axis);

		Matrix4x4 result = _GetRotationMatrix(axis, angle);
			
		return result;
	}
}
