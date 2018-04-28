using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

	public GameObject mBGMPrefab;

	private Text mTouchToStart;
	private Vector3 mTouchToStartInitPos;
	private float mTouchToStartPosY = 0.0f;
	private float mTouchToStartJumpVel = 0.0f;
	private float mTouchToStartJumpTime = 0.0f;

	private bool mIsReqLoad = false;
	private float mLoadWait = 0.0f;

	// Use this for initialization
	void Start () {
		if (GameObject.Find ("BGM(Clone)") == null) {
			Instantiate (mBGMPrefab);
		}

		mTouchToStart = GameObject.Find ("TouchToStart").GetComponent<Text>();
		mTouchToStartInitPos = mTouchToStart.transform.position;
		mTouchToStartJumpTime = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (!mIsReqLoad && ((Input.touchCount > 0) || Input.GetKey(KeyCode.Space))) {
			mIsReqLoad = true;
			mLoadWait = 0.5f;
			GetComponent<AudioSource> ().PlayOneShot (GetComponent<AudioSource> ().clip);
			iTween.ScaleTo (mTouchToStart.gameObject, iTween.Hash ("x", 2.0f, "y", 2.0f, "time", 0.48f));
			iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", 0.48f, "onupdate", "SetTextAlpha"));
			mTouchToStartPosY = 0.0f;
			mTouchToStartJumpVel = 0.0f;
		}

		if (mIsReqLoad){
			mLoadWait -= Time.deltaTime;
			if (mLoadWait <= 0.0f) {
				SceneManager.LoadScene ("Game");
			}
		}

		mTouchToStartJumpTime -= Time.deltaTime;
		if (mTouchToStartJumpTime <= 0.0f && !mIsReqLoad) {
			mTouchToStartJumpTime = 3.0f;
			mTouchToStartJumpVel = 2.0f;
		}

		mTouchToStartJumpVel *= Mathf.Pow(0.99f, Time.deltaTime);
		mTouchToStartJumpVel -= 0.15f * Time.deltaTime * 60;
		mTouchToStartPosY += mTouchToStartJumpVel;

		if (mTouchToStartPosY <= 0.0f) {
			mTouchToStartPosY = 0.0f;
			mTouchToStartJumpVel *= -0.75f;
		}

		{
			Vector3 pos = mTouchToStartInitPos;
			pos.y += mTouchToStartPosY * Time.deltaTime * 60;
			mTouchToStart.transform.position = pos;
		}
	}

	void SetTextAlpha(float alpha) {
		Color color = mTouchToStart.color;
		color.a = alpha;
		mTouchToStart.color = color;
	}
}
