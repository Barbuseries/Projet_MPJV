using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CustomRender/Custom Cube")]
public class CustomCube : CustomPrimitive {
	override protected int GetVertexShapeCount() { return 8; }
	
	override protected int[] GetVAO() {
		return new int[]
		{
			0, 1, 2, 3, 0,
			4, 5, 1,
			5, 6, 2,
			6, 7, 3,
			7, 4, 5
		};
	}
}
