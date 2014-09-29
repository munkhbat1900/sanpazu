using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleRule : System.Object {
	/// <summary>
	/// Get blocks which are successful.
	/// </summary>
	/// <returns>The success block. dictionary key is successNumber. value is successful block tags</returns>
	/// <param name="tagBlockDictionary">Tag block dictionary.</param>
	public Dictionary<int, ArrayList> getSuccessBlock(Dictionary<int, GameObject> tagBlockDictionary) {
		for (int x = 1; x <= Consts.BOARD_SIZE_X; x++) {
			for (int y = 1; y <= Consts.BOARD_SIZE_Y; y++) {

			}		
		}
		return null;
	}
}
