using UnityEngine;

// TODO: This can be used to make collisions.
public abstract class Shape {
	protected Matrix4x4 _inertia;
	protected bool dirtyInertia = true;
	public Matrix4x4 inertia {
		get {
			if (dirtyInertia) UpdateInertia();
			
			return _inertia;
		}
	}

	protected float _mass;
	public float mass {
		get {return _mass;}
		set {
			_mass = value;
			dirtyInertia = true;
		}
	}

	public Shape() {
		_inertia = Matrix4x4.identity;
	}

	
	private void UpdateInertia() {
		ComputeInertia();
		dirtyInertia = false;
	}

	public abstract void ComputeInertia();
}

public class Ellipsoid : Shape {
	private float _a, _b, _c;

	public float a {
		get {return _a;}
		set {
			_a = value;
			dirtyInertia = true;
		}
	}

	public float b {
		get {return _b;}
		set {
			_b = value;
			dirtyInertia = true;
		}
	}

	public float c {
		get {return _c;}
		set {
			_c = value;
			dirtyInertia = true;
		}
	}

	public Ellipsoid(float a, float b, float c, float mass = 1.0f) {
		_mass = mass;

		SetDimensions(a, b, c);
	}
	
	public void SetDimensions(float a, float b, float c) {
		_a = a;
		_b = b;
		_c = c;

		dirtyInertia = true;
	}
	
	public override void ComputeInertia() {
		float value = 1.0f/5.0f * mass;
		
		float aa = a * a;
		float bb = b * b;
		float cc = c * c;
		
		_inertia.m00 = value * (bb + cc);
		_inertia.m11 = value * (aa + cc);
		_inertia.m22 = value * (aa + bb);
	}
}

public class Sphere : Shape {
	private float _radius;

	public float radius {
		get {return _radius;}
		set {
			_radius = value;
			dirtyInertia = true;
		}
	}

	public Sphere(float radius, float mass = 1.0f) {
		_radius = radius;
		_mass = mass;

		dirtyInertia = true;
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
			dirtyInertia = true;
		}
	}

	public float height {
		get {return _height;}
		set {
			_height = value;
			dirtyInertia = true;
		}
	}

	public float depth {
		get {return _depth;}
		set {
			_depth = value;
			dirtyInertia = true;
		}
	}

	public Cube(float width, float height, float depth, float mass = 1.0f) {
		_mass = mass;
		SetDimensions(width, height, depth);
	}
	
	public void SetDimensions(float width, float height, float depth) {
		_width = width;
		_height = height;
		_depth = depth;

		dirtyInertia = true;
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
