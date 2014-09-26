using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour {

	// board size at the X-axis
	private int BOARD_SIZE_X = 7;
	// board size at the Y-axis
	private int BOARD_SIZE_Y = 5;
	// block prefab
	private GameObject blocks;
	// cell width
	private float cellWidth;
	// cell height
	private float cellHeight;
	// board width
	private float boardWidth;
	// board height
	private float boardHeight;

	// coma array.  
	public GameObject[] Blocks;

	private void initBoard() {
		Debug.Log("it is called");
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		boardWidth = sprite.bounds.size.x;
		boardHeight = sprite.bounds.size.y;

		cellWidth = boardWidth / BOARD_SIZE_X;
		cellHeight = boardHeight / BOARD_SIZE_Y;

		Debug.Log (string.Format("boardWidth = {0}", boardWidth));
		Debug.Log (string.Format("boardHeight = {0}", boardHeight));

		Debug.Log (string.Format("cellWidth = {0}", cellWidth));
		Debug.Log (string.Format("cellHeight = {0}", cellHeight));

		PutBlock ();
	}

	private void PutBlock () {
		for (int x = 0; x < BOARD_SIZE_X; x++) {
			for (int y = 0; y < BOARD_SIZE_Y; y++) {
				Instantiate(GetRandomBlock(x, y), GetBlockPosition(x, y), transform.rotation);
			}		
		}
	}

	void OnGUI ()
	{
		if (Event.current.type == EventType.MouseDown) {
			Debug.Log(Camera.main.ScreenToWorldPoint(Event.current.mousePosition));
			//Debug.Log("haha");
			//Debug.Log(string.Format("touchPoint = {0}", touch.position));
		}
	}

	private GameObject GetRandomBlock(int x, int y) {
		return Blocks [Random.Range (0, Blocks.Length)];
	} 

	private Vector2 GetBlockPosition(int x, int y) {
		float xx = cellWidth * x - cellWidth / 2;
		float yy = cellHeight * y - cellHeight / 2;
		return new Vector2 (xx, yy);
	}

	// Use this for initialization
	void Start () {
		initBoard ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < Input.touchCount; i++) {
			Touch touch = Input.GetTouch(i);
			Debug.Log(string.Format("touchPoint = {0}", touch.position));
		}
	}
}
