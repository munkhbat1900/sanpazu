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
	private SortedDictionary<int, GameObject> tagBlockDictionry;
	// coma array.  
	public GameObject[] Blocks;
	// game board sprite
	private SpriteRenderer boardSprite;
	// tag of selected block. 
	private int selectedBlockTag;

	// default layer sorting order
	private const int DEFAULT_LAYER_SORTING_ORDER = 5;

	private PuzzleRule puzzleRule;

	private float moveBlocksAnimationTime;

	private float shrinkAnimationTime;

	// don't get touch event while blocks are animating.
	private bool isAnimating;

	private void initBoard() {
		SpriteRenderer sprite = GetComponent<SpriteRenderer> ();
		boardWidth = sprite.bounds.size.x;
		boardHeight = sprite.bounds.size.y;

		cellWidth = boardWidth / Consts.BOARD_SIZE_X;
		cellHeight = boardHeight / Consts.BOARD_SIZE_Y;

		this.boardSprite = sprite;
		puzzleRule = new PuzzleRule ();
		PutBlock ();
		moveBlocksAnimationTime = tagBlockDictionry [Consts.GetTag(1, 1)].GetComponent<MoveBlockAnimation> ().moveTime;
		shrinkAnimationTime = tagBlockDictionry [Consts.GetTag(1, 1)].GetComponent<ShrinkAnimation>().shrinkTime;
		isAnimating = false;
	}

	private void PutBlock () {
		tagBlockDictionry = new SortedDictionary<int, GameObject> ();
		for (int x = 1; x <= Consts.BOARD_SIZE_X; x++) {
			for (int y = 1; y <= Consts.BOARD_SIZE_Y; y++) {
				GameObject blockInstance = (GameObject)Instantiate(GetRandomBlock(x, y), GetBlockPosition(x, y), transform.rotation);
				blockInstance.renderer.sortingOrder = DEFAULT_LAYER_SORTING_ORDER;
				tagBlockDictionry.Add(Consts.GetTag(x, y), blockInstance);
			}		
		}
	}

	void OnGUI ()
	{
		if (Event.current.type == EventType.MouseDown && !isAnimating) {
			Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (computeSelectedBlockTag(touchPoint)){
				boostBlock(touchPoint);
			}
		} 

		if (Event.current.type == EventType.MouseDrag && !isAnimating) {
			Vector2 touchPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			GameObject tapBlock = tagBlockDictionry [selectedBlockTag];
			tapBlock.renderer.sortingOrder = DEFAULT_LAYER_SORTING_ORDER + 1;
			tapBlock.transform.position = touchPoint;
			exchangeComa(touchPoint);
			boostBlock(touchPoint);
		}

		if (Event.current.type == EventType.MouseUp && !isAnimating) {
			BlockMoveEnd();
		}
	}

	/// <summary>
	/// process at the end of block moving.
	/// </summary>
	void BlockMoveEnd() {
		if (selectedBlockTag != -1) {
			GameObject tapBlock = tagBlockDictionry [selectedBlockTag];
			tapBlock.renderer.sortingOrder = DEFAULT_LAYER_SORTING_ORDER;
			selectedBlockTag = -1;
			resetBlockPosition ();
		}
		SortedDictionary<int, int> successBlockMap = puzzleRule.getSuccessBlock (tagBlockDictionry);
		if (successBlockMap != null && successBlockMap.Count != 0) {
			isAnimating = true;
			end (successBlockMap);
		} else {
			isAnimating = false;	
		}
	}
	 
	private void end(SortedDictionary<int, int> successBlockMap) {
		generateNewBlocks (successBlockMap);
		StartCoroutine ("removeBlocksAndGenerateNewBlocks", successBlockMap);
	}

	IEnumerator removeBlocksAndGenerateNewBlocks(SortedDictionary<int, int> successBlockMap) {
		SortedDictionary<int, int> tmpSuccessBlockMap = new SortedDictionary<int, int>(successBlockMap);
		while (successBlockMap.Count != 0) {
			SortedDictionary<int, int> removinBlockDictionary = getRemovingBlocksByType (successBlockMap);
			removeBlocksAnimation (removinBlockDictionary);
			yield return new WaitForSeconds (shrinkAnimationTime);
			removeBlocks (removinBlockDictionary);
			removeFromSuccessBlockMap(successBlockMap, removinBlockDictionary);
		}
		moveBlockAnimation (tmpSuccessBlockMap);
		int successCount = SuccessCount (tmpSuccessBlockMap);
		StartCoroutine ("moveBlocks", successCount);
	}

	IEnumerator moveBlocks(int successCount) {
		yield return new WaitForSeconds (moveBlocksAnimationTime);
		BlockMoveEnd ();
	}

	private void moveBlockAnimation(SortedDictionary<int, int> successBlockMap) {
		searchNewPosition (successBlockMap);
		List<int> keyList = new List<int>(tagBlockDictionry.Keys);
		// key old tag. value new tag.
		SortedDictionary<int, int> movedBlockDictionary = new SortedDictionary<int, int> ();
		
		SortedDictionary<int, GameObject> newTagBlockDictionary = new SortedDictionary<int, GameObject> ();
		
		foreach (var key in keyList) {
			GameObject block = tagBlockDictionry[key];
			
			int nextPosX = block.GetComponent<NextPosition>().X;
			int nextPosY = block.GetComponent<NextPosition>().Y;
			
			if (nextPosX != int.MaxValue && nextPosY != int.MaxValue) {
				Vector2 newPosition = GetBlockPosition(nextPosX, nextPosY);
				PositionIndex positionIndex = Consts.GetPositionIndexFromTag(key);
				block.GetComponent<MoveBlockAnimation>().DestinationPoint = newPosition;
				block.GetComponent<MoveBlockAnimation>().IsMoving = true;
				
				int newTag = Consts.GetTag(nextPosX, nextPosY);
				movedBlockDictionary[key] = newTag;
				newTagBlockDictionary.Add(newTag, block);
				block.GetComponent<NextPosition>().X = int.MaxValue;
				block.GetComponent<NextPosition>().Y = int.MaxValue;
			} else {
				newTagBlockDictionary.Add(key, block);
			}
		}
		tagBlockDictionry.Clear ();
		tagBlockDictionry = newTagBlockDictionary;
	}

	private void removeBlocksAnimation(SortedDictionary<int, int> successBlockMap) {
		foreach (var pair in successBlockMap) {
			GameObject block = tagBlockDictionry[pair.Key];
			block.GetComponent<ShrinkAnimation>().IsShrinking = true;
		}
	}

	private void removeBlocks(SortedDictionary<int, int> successBlockMap) {
		foreach (var pair in successBlockMap) {
			Destroy(tagBlockDictionry[pair.Key]);
			tagBlockDictionry.Remove(pair.Key);
		}
	}

	private SortedDictionary<int, int> getRemovingBlocksByType(SortedDictionary<int, int> successBlockMap) {
		List<int> values = new List<int>(successBlockMap.Values);
		values.Sort ();
		int minValue = values[0];
		
		SortedDictionary<int, int> removinBlockDictionary = new SortedDictionary<int, int> ();
		foreach (var pair in successBlockMap) {
			if (pair.Value == minValue) {
				removinBlockDictionary.Add(pair.Key, minValue);
			}		
		}
		return removinBlockDictionary;
	}
	
	private int SuccessCount (SortedDictionary<int, int> successBlockMap) {
		List<int> values = new List<int>(successBlockMap.Values);
		values.Sort ();
		int minValue = values[0];
		int successCount = 1;
		
		SortedDictionary<int, int> removinBlockDictionary = new SortedDictionary<int, int> ();
		foreach (var pair in successBlockMap) {
			if (pair.Value != minValue) {
				successCount++;
			}		
		}
		return successCount;
	}
	
	private SortedDictionary<int, int> removeFromSuccessBlockMap(SortedDictionary<int, int> successBlockMap, 
	                                                             SortedDictionary<int, int> removinBlockDictionary) {
		foreach (var pair in removinBlockDictionary) {
			if (successBlockMap.ContainsKey(pair.Key)) {
				successBlockMap.Remove(pair.Key);
			}		
		}
		return successBlockMap;
	}

	private void generateNewBlocks(SortedDictionary<int, int> successBlockMap) {
		int[] ycounter  = new int[Consts.BOARD_SIZE_X + 1];
		
		for (int i = 1; i <= Consts.BOARD_SIZE_X; i++) {
			ycounter[i] = -1;
		}

		foreach (var pair in successBlockMap) {
			PositionIndex positionIndex = Consts.GetPositionIndexFromTag(pair.Key);

			// create new block.
			int newBlockTag = Consts.GetTag(positionIndex.xx, ycounter[positionIndex.xx]);

			GameObject blockInstance = (GameObject)Instantiate(createRandomNewObject(), GetBlockPosition(positionIndex.xx, ycounter[positionIndex.xx]), 
			                                                   transform.rotation);
			blockInstance.renderer.sortingOrder = DEFAULT_LAYER_SORTING_ORDER;
			tagBlockDictionry.Add(newBlockTag, blockInstance);
			ycounter[positionIndex.xx] = ycounter[positionIndex.xx] - 1;
		}
	}

	private void searchNewPosition(SortedDictionary<int, int> successBlockMap) {
		foreach (var pair in successBlockMap) {
			PositionIndex positionIndex = Consts.GetPositionIndexFromTag(pair.Key);
			int xIndex = positionIndex.xx;
			int yIndex = positionIndex.yy;

			foreach (var pair1 in tagBlockDictionry) {
				PositionIndex posIndex = Consts.GetPositionIndexFromTag(pair1.Key);
				if (xIndex == posIndex.xx && yIndex > posIndex.yy) {
					setNewPosition(posIndex);
				}
			}
		}
	}

	void setNewPosition(PositionIndex positionIndex) {
		GameObject block = tagBlockDictionry [Consts.GetTag (positionIndex.xx, positionIndex.yy)];

		int nextPosY = block.GetComponent<NextPosition> ().Y;
		if (nextPosY == int.MaxValue) {
			nextPosY = positionIndex.yy;
		}
		
		nextPosY++;
		if (nextPosY == 0) {
			nextPosY++;		
		}
		
		block.GetComponent<NextPosition> ().X = positionIndex.xx;
		block.GetComponent<NextPosition> ().Y = nextPosY;
	}

	private GameObject createRandomNewObject() {
		return Blocks [Random.Range (0, Blocks.Length)];
	}

	/// <summary>
	/// Resets the all block position to right position.
	/// </summary>
	void resetBlockPosition() {
		for (int x = 1; x <= Consts.BOARD_SIZE_X; x++) {
			for (int y = 1; y <= Consts.BOARD_SIZE_Y; y++) {
				GameObject block = tagBlockDictionry[Consts.GetTag(x, y)];
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

		PositionIndex passedBlockPositionIndex = Consts.GetPositionIndexFromTag (selectedBlockTag);
		tagBlockDictionry [selectedBlockTag].transform.position = GetBlockPosition (passedBlockPositionIndex.xx, passedBlockPositionIndex.yy);
		selectedBlockTag = passedBlockTag;
	}

	/// <summary>
	/// change block position. set the tap point to be bottom of the block.
	/// </summary>
	void boostBlock(Vector2 tapPoint) {
		GameObject tapBlock = tagBlockDictionry [selectedBlockTag];
		Vector2 max = tapBlock.renderer.bounds.max;
		Vector2 min = tapBlock.renderer.bounds.min;
		tapBlock.transform.position = new Vector2 (tapBlock.transform.position.x, tapPoint.y + (max.y - min.y) / 3);
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

		selectedBlockTag = Consts.GetTag (xIndex, yIndex);
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

		return Consts.GetTag (xIndex, yIndex);
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
				GameObject leftBlock = tagBlockDictionry[Consts.GetTag(x - 1, y)];
				if (newBlock.name == leftBlock.name) {
					continue;
				}
			}

			if (y > 1) {
				GameObject belowBlock = tagBlockDictionry[Consts.GetTag(x, y - 1)];
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
