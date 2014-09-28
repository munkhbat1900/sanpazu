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
	// tag of selected block. 
	private int selectedBlockTag;

	private void initBoard() {
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		boardWidth = sprite.bounds.size.x;
		boardHeight = sprite.bounds.size.y;

		cellWidth = boardWidth / BOARD_SIZE_X;
		cellHeight = boardHeight / BOARD_SIZE_Y;

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
			Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			computeSelectedBlockTag(touchPoint);
		}

		if (Event.current.type == EventType.MouseDrag) {
				
		}
	}
	/// <summary>
	/// Computes the selected block tag.
	/// </summary>
	/// <param name="touchPoint">Touch point.</param>
	void computeSelectedBlockTag(Vector2 touchPoint) {
		Vector2 min = boardSprite.bounds.min;
		Vector2 max = boardSprite.bounds.max;

		Debug.Log (string.Format ("touchPoint = {0} ", touchPoint));

		//Debug.Log (string.Format ("min = {0} ", min));
		//Debug.Log (string.Format ("max = {0} ", max));

		if (min.x > touchPoint.x || max.x < touchPoint.x) {
			Debug.Log("x exceeded");
			return;
		}

		if (min.y > touchPoint.y || max.y < touchPoint.y) {
			Debug.Log("exceeded");
			return;
		}

		int xIndex = (int)((touchPoint.x - min.x) / cellWidth) + 1;
		int yIndex = (int)((touchPoint.y - min.y) / cellHeight) + 1;

		Debug.Log(string.Format("xIndex = {0}, yIndex = {0}", xIndex, yIndex));
	}

/// <summary>
/// Gets the random block. and ensure that there is not same block in below and left side of this block located in (x,y).
/// </summary>
/// <returns>The random block.</returns>
/// <param name="x">The x coordinate.</param>
/// <param name="y">The y coordinate.</param>
	private GameObject GetRandomBlock(int x, int y) {
		GameObject newBlock;
		while (true) {
			newBlock = Blocks [Random.Range (0, Blocks.Length)];
			if (x == 1 && y == 1) {
				return newBlock;
			}

			if (x > 1) {
				GameObject leftBlock = tagBlockDictionry[getTag(x - 1, y)];
				if (newBlock.name == leftBlock.name) {
					continue;
				}
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

		Vector2 min = boardSprite.bounds.min;

		return new Vector2 (min.x + xx , min.y + yy);
	}

	// Use this for initialization
	void Start () {
		initBoard ();
	}
	
	// Update is called once per frame
	void Update () {
	}
}
