using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketMove : MonoBehaviour {

	//----------------------------------
	// 定数
	//----------------------------------

	private float cKd = 0.4f;
	private float cRotKd = 0.2f;
	private float cAccLow = 0.04f;
	private float cAccHigh = 0.04f;
	private float cRotAccLow = 0.0f;
	private float cRotAccHigh = 5.0f;
	private float cMoveDirKf = 1.0f;

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// 速度
	private float mSpeed;
	private Vector3 mMoveDir;

	// 角速度
	private float mRotVel;

	// スライダー
	[SerializeField] private Slider mSliderLeft;
	[SerializeField] private Slider mSliderRight;

	// コリジョン
	public CollisionManager mCollisionManager;

	// Use this for initialization
	void Start () {
		// 速度
		mSpeed = 0.0f;
		mMoveDir = Vector3.up;

		// 角速度
		mRotVel = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		CalcMove ();

		CalcCollision ();
	}

	void CalcMove()
	{
		// 加速度
		float accLeft = Mathf.Lerp (cAccLow, cAccHigh, mSliderLeft.value);
		float accRight = Mathf.Lerp (cAccLow, cAccHigh, mSliderRight.value);

		// 角加速度
		float rotAccLeft = Mathf.Lerp (cRotAccLow, cRotAccHigh, mSliderLeft.value);
		float rotAccRight = Mathf.Lerp (cRotAccLow, cRotAccHigh, mSliderRight.value);

		// 回転ダンパー
		mRotVel *= Mathf.Pow(cRotKd, Time.deltaTime);

		// 回転更新
		mRotVel += ( - rotAccLeft + rotAccRight ) * Time.deltaTime;

		// 回転
		transform.RotateAround(transform.position, Vector3.forward, mRotVel);
		mMoveDir = Vector3.Slerp (mMoveDir, transform.up, cMoveDirKf);

		// ダンパー
		mSpeed *= Mathf.Pow (cKd, Time.deltaTime);

		// 加速
		mSpeed += (accLeft + accRight) * Time.deltaTime;

		// 位置更新
		Vector3 pos = transform.position;
		pos += mSpeed * mMoveDir;
		transform.position = pos;
	}

	void CalcCollision()
	{
		if (mCollisionManager.CheckSphereCollision (transform.position, 0.1f)) {
			Application.LoadLevel ("Game");
		}
	}
}
