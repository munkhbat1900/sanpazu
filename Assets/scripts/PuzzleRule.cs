using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleRule : System.Object {

	private int successCount;
	/// dictionary to record whether block is checked.
	/// key is block tag. value is pair of x-axis check and y-axis check
	SortedDictionary<int, bool> tagRightCheckDictionary;
	SortedDictionary<int, bool> tagUpCheckDictionary;

	// 成功したコマのタグをマップとして保存。
	// キー：コマのタグ、値：パズル成功番号。
	SortedDictionary<int, int> successBlockMap;

	private void CheckPuzzleByAxis(MoveDirection moveDirection, int x, int y, 
	                               SortedDictionary<int, GameObject> tagBlockDictionary) {
		int tag = Consts.GetTag (x, y);

		if (MoveDirection.X == moveDirection && HasChecked(MoveDirection.X, x, y)) {
			return;
		} else if (MoveDirection.Y == moveDirection && HasChecked(MoveDirection.Y, x, y)) {
			return;		
		}

		int continuousBlockCounter = 1;

		while (true) {
			PositionIndex nextBlockPositionIndex = new PositionIndex();
			if (moveDirection == MoveDirection.X) {
				nextBlockPositionIndex.xx = x + continuousBlockCounter;
				nextBlockPositionIndex.yy = y;
			} else {
				nextBlockPositionIndex.xx = x;
				nextBlockPositionIndex.yy = y + continuousBlockCounter;
			}

			int nextBlockTag = Consts.GetTag(nextBlockPositionIndex.xx, nextBlockPositionIndex.yy);
			GameObject block = tagBlockDictionary[tag];
			GameObject nextBlock = null;
			try {
				nextBlock = tagBlockDictionary[nextBlockTag];
			} catch (KeyNotFoundException ignore) {
				nextBlock = null;
			}

			if (nextBlockPositionIndex.xx <= Consts.BOARD_SIZE_X && nextBlockPositionIndex.yy <= Consts.BOARD_SIZE_Y
			    && nextBlock != null && block.renderer.name.Equals(nextBlock.renderer.name)) {

				// mark next block as checked
				if (moveDirection == MoveDirection.X) {
					tagRightCheckDictionary[tag] = true;
				} else {
					tagUpCheckDictionary[tag] = true;
				}
				continuousBlockCounter++;
			} else {
				if (continuousBlockCounter >= Consts.BLOCK_SUCCESS_COUNT) {
					successCount++;

					// 何番目のサクセスかを表すカウンタ。
					int successNumber = successCount;

					for (int i = 0; i < continuousBlockCounter; i++) {
						int xx = x;
						int yy = y;
						if (moveDirection == MoveDirection.X) {
							xx += i;
						} else {
							yy += i;
						}

						try {
							int tmp = successBlockMap[Consts.GetTag(xx, yy)];
							if (successNumber > tmp) {
								successNumber = tmp;
							}
						} catch(KeyNotFoundException ignore){}

					}

					int previosSuccessNumber = successNumber;
					for (int i = 0; i < continuousBlockCounter; i++) {
						PositionIndex positionIndex = new PositionIndex();
						if (MoveDirection.X == moveDirection) {
							positionIndex.xx = x + i;
							positionIndex.yy = y;
						} else {
							positionIndex.xx = x;
							positionIndex.yy = y + i;
						}
						int tmpTag = Consts.GetTag(positionIndex.xx, positionIndex.yy);
						try {
							if (successNumber < successBlockMap[tmpTag]) {
								previosSuccessNumber = successBlockMap[tmpTag];
							}
						} catch(KeyNotFoundException ignore){}
						successBlockMap[tmpTag] = successNumber;
					}

					// 既存のsuccessNumberがあれば、successNumberより大きければ、全コマに対してそれをおきかえる。
					if (previosSuccessNumber > successNumber) {
						List<int> keyList = new List<int>(successBlockMap.Keys);
						foreach (var key in keyList) {
							if (successBlockMap[key] == previosSuccessNumber) {
								successBlockMap[key] = successNumber;
							}
						}
					}
				}
				break;
			}
		}
	}

	private bool HasChecked(MoveDirection moveDirection, int x, int y) {
		int tag = Consts.GetTag (x, y);
		if (moveDirection == MoveDirection.X) {
			return tagRightCheckDictionary [tag];		
		} else {
			return tagUpCheckDictionary[tag];		
		}
	}

	/// <summary>
	/// Get blocks which are successful.
	/// </summary>
	/// <returns>The success block. dictionary key is successNumber. value is successful block tags</returns>
	/// <param name="tagBlockDictionary">Tag block dictionary.</param>
	public SortedDictionary<int, int> GetSuccessBlock(SortedDictionary<int, GameObject> tagBlockDictionary) {
		successBlockMap = new SortedDictionary<int, int> ();
		tagRightCheckDictionary = new SortedDictionary<int, bool> ();
		tagUpCheckDictionary = new SortedDictionary<int, bool> ();
		successCount = 0;
		for (int x = 1; x <= Consts.BOARD_SIZE_X; x++) {
			for (int y = 1; y <= Consts.BOARD_SIZE_Y; y++) {
				int tag = Consts.GetTag(x, y);
				tagRightCheckDictionary[tag] = false;
				tagUpCheckDictionary[tag] = false;
			}	
		}

		for (int x = 1; x <= Consts.BOARD_SIZE_X; x++) {
			for (int y = 1; y <= Consts.BOARD_SIZE_Y; y++) {
				CheckPuzzleByAxis(MoveDirection.X, x, y, tagBlockDictionary);
				CheckPuzzleByAxis(MoveDirection.Y, x, y, tagBlockDictionary);
			}		
		}
		return successBlockMap;
	}
}
