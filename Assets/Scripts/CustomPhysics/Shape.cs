using UnityEngine;

// TODO: This can be used to make collisions.
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

public class Cube : Shape {
	private float _width;
	private float _height;
	private float _depth;

	public float width {
		get {return _width;}
		set {
			_width = value;
			ComputeInertia();
		}
	}

	public float height {
		get {return _height;}
		set {
			_height = value;
			ComputeInertia();
		}
	}

	public float depth {
		get {return _depth;}
		set {
			_depth = value;
			ComputeInertia();
		}
	}

	public void SetDimensions(float width, float height, float depth) {
		_width = width;
		_height = height;
		_depth = depth;

		ComputeInertia();
	}

	public Cube(float width, float height, float depth, float mass = 1.0f) {
		_mass = mass;
		SetDimensions(width, height, depth);
	}
	
	public override void ComputeInertia() {
		float value = 1.0f/12.0f * mass;
		
		float ww = width * width;
		float hh = height * height;
		float dd = depth * depth;
		
		_inertia.m00 = value * (hh + dd);
		_inertia.m11 = value * (ww + dd);
		_inertia.m22 = value * (ww + hh);
	}
}
