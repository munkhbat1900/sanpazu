using UnityEngine;
using System.Collections;

public class ShrinkAnimation : MonoBehaviour {

	public float targetScale = 0.1f;
	private float shrinkSpeed;
	private bool isShrinking = false;
	public float shrinkTime = 0.5f;

	public bool IsShrinking {
		get {return isShrinking;}
		set {isShrinking = value;}
	}

	// Use this for initialization
	void Start () {
		//isShrinking = true;
		shrinkSpeed = transform.localScale.x / shrinkTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (isShrinking) {
			transform.localScale -= Vector3.one * Time.deltaTime * shrinkSpeed;
			if (transform.localScale.x < targetScale) {
				isShrinking = false;
			}
		}
	}
}
