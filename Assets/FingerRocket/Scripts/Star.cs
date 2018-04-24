using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// SE
	private AudioSource mAudioSourceDestroy;

	// Use this for initialization
	void Start () {
		// SE
		mAudioSourceDestroy = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnGet() {
		mAudioSourceDestroy.PlayOneShot (mAudioSourceDestroy.clip);
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
		Color color = GetComponent<SpriteRenderer> ().color;
		color.a = alpha;
		GetComponent<SpriteRenderer> ().color = color;
	}

	void Destroy()
	{
		Destroy (this.gameObject);
	}
}
