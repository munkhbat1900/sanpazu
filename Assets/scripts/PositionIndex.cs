using System.Collections;
/// <summary>
/// Position.
/// </summary>
public class PositionIndex {
	private int xIndex;
	private int yIndex;

	public int xx {
		set {this.xIndex = value;}
		get {return this.xIndex;}
	}

	public int yy {
		set {this.yIndex = value;}
		get {return this.yIndex;}
	}
}
