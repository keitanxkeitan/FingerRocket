using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// 位置
	private Vector3 mPos;

	// 影
	private GameObject mShadow = null;
	private Vector3 mShadowPos;

	// SE
	private AudioSource mAudioSourceDestroy;

	// ゲット
	private bool mHasGot = false;

	// Use this for initialization
	void Start () {
		// 位置
		mPos = transform.position;

		// 影
		mShadow = transform.Find("StarShadow").gameObject;
		Debug.Assert (mShadow);
		mShadowPos = mShadow.transform.position;

		// SE
		mAudioSourceDestroy = GetComponent<AudioSource>();

		// ゲット
		mHasGot = false;
	}

	// Update is called once per frame
	void Update () {
		if (!mHasGot) {
			float timeRatio = (float)((Time.realtimeSinceStartup * 60) % 60) / 60;
			transform.position = mPos + Vector3.up * Mathf.Sin (Mathf.PI * 2.0f * timeRatio) * 0.03f;
		}
		mShadow.transform.position = mShadowPos;
	}

	public void OnGet() {
		mHasGot = true;
		mAudioSourceDestroy.PlayOneShot (mAudioSourceDestroy.clip);
		FadeOut ();
		Invoke ("Destroy", 1.0f);
	}

	public void OnGameOver() {
		mHasGot = true;
		FadeOut ();
		Invoke ("Destroy", 1.0f);
	}

	void FadeOut()
	{
		iTween.MoveBy (this.gameObject, iTween.Hash ("y", 0.5f, "time", 1.0f));
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", 1.0f, "onupdate", "SetAlpha"));
	}

	void SetAlpha(float alpha)
	{
		{
			Color color = GetComponent<SpriteRenderer> ().color;
			color.a = alpha;
			GetComponent<SpriteRenderer> ().color = color;
		}
		{
			Color color = Color.black;
			color.a = 127.0f / 255.0f * alpha;
			mShadow.GetComponent<SpriteRenderer> ().color = color;
		}
	}

	void Destroy()
	{
		Destroy (this.gameObject);
	}
}
