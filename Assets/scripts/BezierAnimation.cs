using UnityEngine;
using System.Collections;

public class BezierAnimation : MonoBehaviour
{
	public Bezier myBezier;
	private float t = 0f;
	public bool isAnimate = false;
	
	void Start()
	{
		t = 0f;
		//myBezier = new Bezier( new Vector3( -1f, 1f, -1f ), Random.insideUnitSphere * 2f, Random.insideUnitSphere * 2f, new Vector3( 3f, 1f, 3f ) );
	}
	
	void Update()
	{
		if (isAnimate) {
			Vector3 vec = myBezier.GetPointAtTime (t);
			transform.position = vec;
		
			t += 0.1f;
			if (t >= 1f)
				isAnimate = false;
		}
	}
}