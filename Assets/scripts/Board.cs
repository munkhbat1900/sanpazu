using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {
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
	// coma array.  
	public GameObject[] Blocks;
	// game board sprite
	private SpriteRenderer boardSprite;
	// tag of selected block. 
	private int selectedBlockTag;

	// default layer sorting order
	private const int DEFAULT_LAYER_SORTING_ORDER = 5;

	private void initBoard() {
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		boardWidth = sprite.bounds.size.x;
		boardHeight = sprite.bounds.size.y;

		cellWidth = boardWidth / Consts.BOARD_SIZE_X;
		cellHeight = boardHeight / Consts.BOARD_SIZE_Y;

		//Debug.Log (string.Format ("cellWidth = {0} ", cellWidth));
		//Debug.Log (string.Format ("cellHeight = {0} ", cellHeight));

		this.boardSprite = sprite;
		PutBlock ();
	}

	private void PutBlock () {
		tagBlockDictionry = new Dictionary<int, GameObject> ();
		for (int x = 1; x <= Consts.BOARD_SIZE_X; x++) {
			for (int y = 1; y <= Consts.BOARD_SIZE_Y; y++) {
				GameObject blockInstance = (GameObject)Instantiate(GetRandomBlock(x, y), GetBlockPosition(x, y), transform.rotation);
				blockInstance.renderer.sortingOrder = DEFAULT_LAYER_SORTING_ORDER;
				tagBlockDictionry.Add(getTag(x, y), blockInstance);
			}		
		}
	}

	void OnGUI ()
	{
		if (Event.current.type == EventType.MouseDown) {
			Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (computeSelectedBlockTag(touchPoint)){
				boostBlock(touchPoint);
			}
		} 

		if (Event.current.type == EventType.MouseDrag) {
			Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			//Debug.Log(string.Format("drag point = {0}", touchPoint));
			GameObject tapBlock = tagBlockDictionry [selectedBlockTag];
			tapBlock.renderer.sortingOrder = DEFAULT_LAYER_SORTING_ORDER + 1;
			tapBlock.transform.position = touchPoint;
			exchangeComa(touchPoint);
		}

		if (Event.current.type == EventType.MouseUp) {
			//Debug.Log("mouse up");
			BlockMoveEnd();
		}
	}

	/// <summary>
	/// process at the end of block moving.
	/// </summary>
	void BlockMoveEnd() {
		GameObject tapBlock = tagBlockDictionry [selectedBlockTag];
		tapBlock.renderer .sortingOrder = DEFAULT_LAYER_SORTING_ORDER;
		selectedBlockTag = -1;
		resetBlockPosition();
	}

	/// <summary>
	/// Resets the all block position to right position.
	/// </summary>
	void resetBlockPosition() {
		for (int x = 1; x <= Consts.BOARD_SIZE_X; x++) {
			for (int y = 1; y <= Consts.BOARD_SIZE_Y; y++) {
				GameObject block = tagBlockDictionry[getTag(x, y)];
				block.transform.position = GetBlockPosition(x, y);
			}		
		}
	}

	/// <summary>
	/// Exchanges the coma.
	/// </summary>
	/// <param name="touchPoint">Touch point.</param>
	void exchangeComa(Vector2 touchPoint) {
		int passedBlockTag = getPassedBlockTag (touchPoint);
		if (selectedBlockTag == passedBlockTag) {
			return;		
		}


		GameObject selectedBlock = tagBlockDictionry [selectedBlockTag];

		tagBlockDictionry [selectedBlockTag] = tagBlockDictionry [passedBlockTag];
		tagBlockDictionry [passedBlockTag] = selectedBlock;

		PositionIndex passedBlockPositionIndex = GetPositionIndexFromTag (selectedBlockTag);
		tagBlockDictionry [selectedBlockTag].transform.position = GetBlockPosition (passedBlockPositionIndex.xx, passedBlockPositionIndex.yy);
		selectedBlockTag = passedBlockTag;
		//InitBezierAnimation (passedBlockTag);
	}

	void InitBezierAnimation(int passedBlockTag) {
		// move along the block bezier curve. 
//		PositionIndex passedPositionIndex = GetPositionIndexFromTag (passedBlockTag);
//		Vector2 startPoint = GetBlockPosition (passedPositionIndex.xx, passedPositionIndex.yy);
//		PositionIndex selectedBlockPositionIndex = GetPositionIndexFromTag (selectedBlockTag);
//		Vector2 endPoint = GetBlockPosition (selectedBlockPositionIndex.xx, selectedBlockPositionIndex.yy);
//
//		Vector2 controlPoint1;
//		Vector2 controlPoint2;
//		if (passedPositionIndex.xx != selectedBlockPositionIndex.xx) {
//			// movement along x-axis
//			controlPoint1 = new Vector2(endPoint.x, endPoint.y - cellHeight / 2);
//			controlPoint2 = new Vector2(startPoint.x, startPoint.y - cellHeight / 2);
//		} else {
//			// movement along y-axis
//			controlPoint1 = new Vector2(endPoint.x + cellWidth / 2, endPoint.y);
//			controlPoint2 = new Vector2(startPoint.x + cellWidth / 2, startPoint.y);
//		}

		//bezier = new Bezier (startPoint, controlPoint1, endPoint, controlPoint2);
		 //bezier = new Bezier (controlPoint1, startPoint, controlPoint2, endPoint);
		//bezier = new Bezier (endPoint, controlPoint2, startPoint, controlPoint1);
		//bezier = new Bezier (controlPoint2, endPoint, controlPoint1, startPoint);
		//isBlockMoving = true;
	}

	PositionIndex GetPositionIndexFromTag(int tag) {
		int x = (tag - Consts.BASE_TAG) / 10;
		int y = (tag - Consts.BASE_TAG) % 10;

		PositionIndex positionIndex = new PositionIndex ();
		positionIndex.xx = x;
		positionIndex.yy = y;
		return positionIndex;
	}

	/// <summary>
	/// change block position. set the tap point to be bottom of the block.
	/// </summary>
	void boostBlock(Vector2 tapPoint) {
		GameObject tapBlock = tagBlockDictionry [selectedBlockTag];
		tapBlock.transform.position = tapPoint;
	}

	/// <summary>
	/// Computes the selected block tag.
	/// </summary>
	/// <param name="touchPoint">Touch point.</param>
	bool computeSelectedBlockTag(Vector2 touchPoint) {
		Vector2 min = boardSprite.bounds.min;
		Vector2 max = boardSprite.bounds.max;

		if (min.x > touchPoint.x || max.x < touchPoint.x) {
			Debug.Log("x exceeded");
			return false; 
		}

		if (min.y > touchPoint.y || max.y < touchPoint.y) {
			Debug.Log("exceeded");
			return false;
		}

		int xIndex = (int)((touchPoint.x - min.x) / cellWidth) + 1;
		int yIndex = (int)((touchPoint.y - min.y) / cellHeight) + 1;

		selectedBlockTag = getTag (xIndex, yIndex);
		return true;
	}

	int getPassedBlockTag(Vector2 touchPoint) {
		Vector2 min = boardSprite.bounds.min;
		Vector2 max = boardSprite.bounds.max;

		int xIndex = (int)((touchPoint.x - min.x) / cellWidth) + 1;
		int yIndex = (int)((touchPoint.y - min.y) / cellHeight) + 1;

		if (min.y > touchPoint.y) {
			yIndex = 1;		
		}

		if (max.y < touchPoint.y) {
			yIndex = Consts.BOARD_SIZE_Y;		
		}

		if (min.x > touchPoint.x || max.x < touchPoint.x) {
			BlockMoveEnd();		
		}

		return getTag (xIndex, yIndex);
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
		return Consts.BASE_TAG + 10 * x + y;
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
