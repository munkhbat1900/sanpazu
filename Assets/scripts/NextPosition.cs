using UnityEngine;
using System.Collections;

public class NextPosition : MonoBehaviour {
		private int xval;
		private int yval;

	public NextPosition() {
		xval = int.MaxValue;
		yval = int.MaxValue;
	}
		
		public int X {
				get { return xval;}
				set { xval = value;}
		}
		
		public int Y {
			get { return yval;}
			set { yval = value;}
		}
}
