using UnityEngine;
using System.Collections;

// declare constants.
public static class Consts : System.Object {
	// board size at the X-axis
	public const int BOARD_SIZE_X = 7;
	// board size at the Y-axis
	public const int BOARD_SIZE_Y = 5;
	// base tag
	public const int BASE_TAG = 10000;
	// block number which is regarded as success
	public const int BLOCK_SUCCESS_COUNT = 3;
	// puzzle operating time limit
	public const float PUZZLE_OPERATION_LIMIT = 4.0f;

	public static int GetTag(int x, int y) {
		int yy = y;
		if (y < 1) {
			yy = BOARD_SIZE_Y + y * (-1) + 1;
		}
		return BASE_TAG + x * 1000 + yy;
	}

	public static PositionIndex GetPositionIndexFromTag(int tag) {
		int xIndex = (tag - BASE_TAG) / 1000;
		int yIndex = (tag - BASE_TAG) % 100;
		int y = yIndex;
		if (yIndex > BOARD_SIZE_Y) {
			y = BOARD_SIZE_Y - yIndex + 1;
		}
		
		PositionIndex positionIndex = new PositionIndex ();
		positionIndex.xx = xIndex;
		positionIndex.yy = y;
		return positionIndex;
	}
}
