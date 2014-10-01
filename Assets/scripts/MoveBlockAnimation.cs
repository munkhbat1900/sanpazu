using UnityEngine;
using System.Collections;

public class MoveBlockAnimation : MonoBehaviour {
	public float moveTime;
	private float moveSpeed;
	private Vector3 destinationPoint;
	private bool isMoving = false;
	private Vector3 direction;

	public Vector2 DestinationPoint {
		get {return destinationPoint;}
		set {destinationPoint = value;}
	}

	public bool IsMoving {
		get {return isMoving;}
		set {isMoving = value;}
	}

	// Use this for initialization
	void Start () {
		//moveSpeed = (destinationPoint - transform.position).magnitude / moveTime;
		moveSpeed = 8.0f;
		direction = new Vector3 (0, 1, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (isMoving) {
			if ((destinationPoint - transform.position) .magnitude < 0.1) {
				isMoving = false;
				transform.position = destinationPoint;
				return;
			}
			transform.position += direction * moveSpeed * Time.deltaTime;
		}
	}
}
