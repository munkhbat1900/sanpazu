using UnityEngine;
using System.Collections;

public class FpsShow : MonoBehaviour {
	public float mUpdateInterval = 1.0f;  	// 更新される頻度
	int mNumFrame = 0;						// フレーム数
	float mTimeCounter = 0.0f;				// 計測用カウンター
	float mTimer = 0.0f;					// タイマー
	float mFps = 0.0f;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;
		mTimeCounter = mUpdateInterval;
		guiText.text = "FPS:" + 0 + "  ( " + 0 * 1000.0f + "ms )" ;
	}
	
	// Update is called once per frame
	void Update () {
		// 1フレームの時間
		mTimeCounter -= Time.deltaTime;
		mTimer += Time.timeScale / Time.deltaTime;	//  時間間隔 / 1フレームの時間
		mNumFrame++;
		
		// 
		if( mTimeCounter < 0.0f ){
			mFps = mTimer / mNumFrame;
			guiText.text = "FPS:" + mFps.ToString() + "  ( " + 1.0f / mFps * 1000.0f + "ms )" ;
			
			mNumFrame = 0;
			mTimer = 0.0f;
			mTimeCounter = mUpdateInterval;
		}
	}
}
