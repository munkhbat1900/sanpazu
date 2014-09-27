using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	// location and block dictionary
	private Dictionary<int, GameObject> tagBlockDictionry;
	// base tag
	private const int BASE_TAG = 10000;
	// coma array.  
	public GameObject[] Blocks;
	// game board sprite
	private SpriteRenderer boardSprite;

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

		this.boardSprite = sprite;
		PutBlock ();
	}

	private void PutBlock () {
		tagBlockDictionry = new Dictionary<int, GameObject> ();
		for (int x = 1; x <= BOARD_SIZE_X; x++) {
			for (int y = 1; y <= BOARD_SIZE_Y; y++) {
				GameObject block = GetRandomBlock(x, y);
				tagBlockDictionry.Add(getTag(x, y), block);
				Instantiate(block, GetBlockPosition(x, y), transform.rotation);
			}		
		}
	}

	void OnGUI ()
	{
		if (Event.current.type == EventType.MouseDown) {
			Debug.Log(Camera.main.ScreenToWorldPoint(Event.current.mousePosition));
		}
	}

	private GameObject GetRandomBlock(int x, int y) {
		GameObject newBlock;
		while (true) {
			bool b = false;
			newBlock = Blocks [Random.Range (0, Blocks.Length)];
			if (x == 1 && y == 1) {
				return newBlock;
			}

			if (x > 1) {
				GameObject leftBlock = tagBlockDictionry[getTag(x - 1, y)];
				if (newBlock.name == leftBlock.name) {
					continue;
				} else {
					b =  true;
				}
			} else {
				b = true;
			}

			if (y > 1) {
				GameObject belowBlock = tagBlockDictionry[getTag(x, y - 1)];
				if (newBlock.name == belowBlock.name) {
					continue;
				} else {
					break;
				}
			} else {
				break;
			}
		}
		return newBlock;
	}

	private int getTag(int x, int y) {
		return BASE_TAG + 10 * x + y;
	}

	private Vector2 GetBlockPosition(int x, int y) {
		float xx = cellWidth * x - cellWidth / 2;
		float yy = cellHeight * y - cellHeight / 2;

		Vector2 max = boardSprite.bounds.max;
		Vector2 min = boardSprite.bounds.min;

		return new Vector2 (min.x + xx , min.y + yy);
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
