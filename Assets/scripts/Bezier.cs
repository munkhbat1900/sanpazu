using UnityEngine;
using System.Collections;

public class Bezier : System.Object {
	public Vector2 p0;
	public Vector2 p1;
	public Vector2 p2;
	public Vector2 p3;

	public float ti = 0f;
	
	private Vector2 b0 = Vector2.zero;
	private Vector2 b1 = Vector2.zero;
	private Vector2 b2 = Vector2.zero;
	private Vector2 b3 = Vector2.zero;
	
	private float Ax;
	private float Ay;
	private float Az;
	
	private float Bx;
	private float By;
	private float Bz;

	// Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
	// handle1 = v0 + v1
	// handle2 = v3 + v2
	public Bezier( Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3 )
	{
		this.p0 = v0;
		this.p1 = v1;
		this.p2 = v2;
		this.p3 = v3;
	}
	// 0.0 >= t <= 1.0
	public Vector2 GetPointAtTime( float t )
	{
		this.CheckConstant();
		float t2 = t * t;
		float t3 = t * t * t;
		float x = this.Ax * t3 + this.Bx * t2 + this.Cx * t + p0.x;
		float y = this.Ay * t3 + this.By * t2 + this.Cy * t + p0.y;
		return new Vector2( x, y, z );
	}
	
	private void SetConstant()
	{
		this.Bx = 3f * ( ( this.p3.x + this.p2.x ) - ( this.p0.x + this.p1.x ) ) - this.Cx;
		this.Ax = this.p3.x - this.p0.x - this.Cx - this.Bx;
		this.By = 3f * ( ( this.p3.y + this.p2.y ) - ( this.p0.y + this.p1.y ) ) - this.Cy;
		this.Ay = this.p3.y - this.p0.y - this.Cy - this.By;

		this.Bz = 3f * ( ( this.p3.z + this.p2.z ) - ( this.p0.z + this.p1.z ) ) - this.Cz;
		this.Az = this.p3.z - this.p0.z - this.Cz - this.Bz;
	}
	
	// Check if p0, p1, p2 or p3 have change
	private void CheckConstant()
	{
		if( this.p0 != this.b0 || this.p1 != this.b1 || this.p2 != this.b2 || this.p3 != this.b3 )
		{
			this.SetConstant();
			this.b0 = this.p0;
			this.b1 = this.p1;
			this.b2 = this.p2;
			this.b3 = this.p3;
		}
	}
}
