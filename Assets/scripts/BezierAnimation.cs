using UnityEngine;
using System.Collections;

public class BezierAnimation : MonoBehaviour
{
	public Bezier myBezier;
	private float t = 0f;
	public bool isAnimate = false;
	// use when stop animation while animating.
	public bool isStopAnimation = false;
	
	void Start()
	{
		t = 0f;
	}
	
	void Update()
	{
		if (isStopAnimation && isAnimate) {
			transform.position = myBezier.p3;
			t = 0f;
			isAnimate = false;
			isStopAnimation = false;
		} else if (isAnimate) {
			Vector3 vec = myBezier.GetPointAtTime (t);
			transform.position = vec;
		
			t += 0.15f;
			if (t >= 1f) {
				isAnimate = false;
				transform.position = myBezier.p3;
				t = 0f;
			}
		}
	}
}