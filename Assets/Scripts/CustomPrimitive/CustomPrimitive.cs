using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: This acts a little like a vertex shader:
// - It takes the vertices attached to the gameObject
// - Store their relative position
// - Use them to compute their actual position
//   ((relativetransfromMatrix * relativePos) + objectPos)
//
// It also draw lines between the vertices (to customize in each
// primitive).

// IMPORTANT: For the vertices to be found, they must be in a child,
//  named 'Vertices', of the gameObject.
public abstract class CustomPrimitive : MonoBehaviour {
	public float vertexSize = 0.2f;
	public float lineWidth = 0.05f;
	
	private CustomTransform customTransform;
	
	private GameObject[] _vertices;
	private Vector3[] _verticesPosition;
	protected int _vertexCount;

	// Draw lines between vertices.
	private LineRenderer line;
	private int[] _VAO;
	
	void Start() {
		customTransform = gameObject.GetComponent<CustomTransform>();
		
		Transform vertices = gameObject.transform.Find("Vertices");

		if (vertices != null) {
			_vertexCount = vertices.childCount;
			
			_vertices = new GameObject[_vertexCount];
			_verticesPosition = new Vector3[_vertexCount];

			var i = 0;
			foreach (Transform v in vertices) {
				_vertices[i] = v.gameObject;
				_verticesPosition[i] = v.localPosition;
				++i;
			}
		}
		else { // So I do not have to check for null in some places.
			_vertices = new GameObject[0];
		}

		Debug.Assert(_vertexCount == GetVertexShapeCount(),
					 "Invalid vertex count: " + _vertexCount + " instead of " + GetVertexShapeCount() + ".");
		
		_VAO = GetVAO();

		if (_VAO != null) {
			line = gameObject.AddComponent<LineRenderer>();

			line.SetWidth(lineWidth, lineWidth);
			line.SetVertexCount(_VAO.Length);
		}
	}

	void Update() {
		Debug.Assert((_vertices.Length == _vertexCount),
					 "The number of vertices of this primitive has changed!");

		Matrix4x4 vertexTransformMatrix = customTransform.RelativeTransformMatrix();
		for (int i = 0; i < _vertexCount; ++i) {
			_vertices[i].transform.position = vertexTransformMatrix.MultiplyVector(_verticesPosition[i]) + customTransform.position;

			// NOTE: objectScale = parentScale * localScale
			_vertices[i].transform.localScale = vertexSize * Utils.invertOrZero(customTransform.scale);
		}

		if (_vertices.Length >= 2) {
			for (var i = 0; i < _VAO.Length; ++i) {
				var index = _VAO[i]; 
				line.SetPosition(i, _vertices[index].transform.position);
			}
		}
	}

	// Return the vertex order to draw the shape with a single line.
	// Return null if there is no vertex.
	abstract protected int[] GetVAO();

	// Return the number of vertices needed to draw the shape.
	abstract protected int GetVertexShapeCount();
}
