using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketMove : MonoBehaviour {

	//----------------------------------
	// 定数
	//----------------------------------

	private float cDamper = 0.55f;
	private float cRotDamper = 0.25f;
	private float cAccLow = 0.04f;
	private float cAccHigh = 0.1f;
	private float cRotAccLow = 0.0f;
	private float cRotAccHigh = 10.0f;

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// 速度
	private Vector3 mVel;

	// 角速度
	private float mRotVel;

	// スライダー
	[SerializeField] private Slider mSliderLeft;
	[SerializeField] private Slider mSliderRight;

	// Use this for initialization
	void Start () {
		// 速度
		mVel = Vector3.zero;

		// 角速度
		mRotVel = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		// 加速度
		float accLeft = Mathf.Lerp (cAccLow, cAccHigh, mSliderLeft.value);
		float accRight = Mathf.Lerp (cAccLow, cAccHigh, mSliderRight.value);

		// 角加速度
		float rotAccLeft = Mathf.Lerp (cRotAccLow, cRotAccHigh, mSliderLeft.value);
		float rotAccRight = Mathf.Lerp (cRotAccLow, cRotAccHigh, mSliderRight.value);

		// ダンパー
		mVel *= Mathf.Pow (cDamper, Time.deltaTime);

		// 加速
		mVel += transform.up * (accLeft + accRight) * Time.deltaTime;

		// 位置更新
		Vector3 pos = transform.position;
		pos += mVel;
		transform.position = pos;

		// 回転ダンパー
		mRotVel *= Mathf.Pow(cRotDamper, Time.deltaTime);

		// 回転更新
		mRotVel += ( - rotAccLeft + rotAccRight ) * Time.deltaTime;

		// 回転
		transform.RotateAround(transform.position, Vector3.forward, mRotVel);
	}
}
