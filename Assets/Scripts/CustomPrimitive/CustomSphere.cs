using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: This directly uses Unity's Sphere, because I can not be
// bothered to create a variable length vertex array.
[AddComponentMenu("CustomRender/Custom Sphere")]
public class CustomSphere : CustomPrimitive {
	override protected int GetVertexShapeCount() { return 0; }
	
	override protected int[] GetVAO() {
		return null;
	}
}
