using UnityEngine;

public abstract class Shape {
	protected Matrix4x4 _inertia;
	
	public Matrix4x4 inertia {get {return _inertia;}}

	protected float _mass;
	public float mass {
		get {return _mass;}
		set {
			_mass = value;
			ComputeInertia();
		}
	}

	public Shape() {
		_inertia = Matrix4x4.identity;
	}

	public abstract void ComputeInertia();
}

public class Sphere : Shape {
	private float _radius;

	public float radius {
		get {return _radius;}
		set {
			_radius = value;
			ComputeInertia();
		}
	}

	public Sphere(float radius, float mass = 1.0f) {
		_radius = radius;
		_mass = mass;

		ComputeInertia();
	}
	
	public override void ComputeInertia() {
		float value = 2.0f/5.0f * mass * radius * radius;
		
		_inertia.m00 = value;
		_inertia.m11 = value;
		_inertia.m22 = value;
	}
}
