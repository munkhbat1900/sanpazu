using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Bezier. reference http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/ 
/// </summary>
public class Bezier : System.Object
{
	public Vector3 p0;
	public Vector3 p1;
	public Vector3 p2;
	public Vector3 p3;
	
	private Vector3 b0 = Vector3.zero;
	private Vector3 b1 = Vector3.zero;
	private Vector3 b2 = Vector3.zero;
	private Vector3 b3 = Vector3.zero;
	
	private float Ax;
	private float Ay;
	private float Az;
	
	private float Bx;
	private float By;
	private float Bz;
	
	private float Cx;
	private float Cy;
	private float Cz;
	
	// Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
	// handle1 = v0 + v1
	// handle2 = v3 + v2
	public Bezier( Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3 )
	{
		this.p0 = v0;
		this.p1 = v1;
		this.p2 = v2;
		this.p3 = v3;
	}

	public Vector3 GetPointAtTime( float t ) {
		float u = 1 - t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;
		
		Vector3 p = uuu * p0; //first term
		p += 3 * uu * t * p1; //second term
		p += 3 * u * tt * p2; //third term
		p += ttt * p3; //fourth term
		
		return p;
	}
}