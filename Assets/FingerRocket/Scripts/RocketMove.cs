﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketMove : MonoBehaviour {

	//----------------------------------
	// パラメータ
	//----------------------------------

	[SerializeField] private float mKd = 0.05f;
	[SerializeField] private float mRotKd = 0.5f;
	[SerializeField] private float mBaseSpeed = 0.75f;
	[SerializeField] private float mAccLow = 0.005f;
	[SerializeField] private float mAccHigh = 0.15f;
	[SerializeField] private float mRotAccLow = 0.0f;
	[SerializeField] private float mRotAccHigh = 10.0f;
	[SerializeField] private float mMoveDirKf = 1.0f;

	[SerializeField] private float mSmokeStartSizeLow = 0.1f;
	[SerializeField] private float mSmokeStartSizeHigh = 0.15f;

	[SerializeField] private float mTimeLimit = 180.0f;

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// BGM プレハブ
	public GameObject mBGMPrefab;

	// 速度
	private float mSpeed;
	private Vector3 mMoveDir;

	// 角速度
	private float mRotVel;

	// スライダー
	[SerializeField] private Slider mSliderLeft;
	[SerializeField] private Slider mSliderRight;

	// 距離
	[SerializeField] private Text mTextDistance;

	// タイム
	[SerializeField] private Text mTextTime;

	// スター
	[SerializeField] private Text mTextStar;

	// コースマネージャ
	public CourseManager mCourseManager;

	// スターマネージャ
	public StarManager mStarManager;

	// ゲームオーバーマネージャ
	public GameOverManager mGameOverManager;

	// コリジョン
	public CollisionManager mCollisionManager;

	// パーティクル
	public GameObject mParticleSystemSmokeLeft;
	public GameObject mParticleSystemSmokeRight;
	public GameObject mParticleSystemCrushSmoke;

	// SE
	private AudioSource mAudioSourceDestroy;

	// やられ
	private bool mIsDestroyed;

	// タイム
	private float mTime;

	// 残り時間
	private float mTimeLeft;

	// スター
	private int mStar;

	// ゴール
	private bool mIsGoal;

	// カメラサイズ
	private static float sSliderCameraSizeValue = 0.0f;

	// 無敵
	private static bool sIsInvincible = false;

	// 距離
	private int mCoursePartIndex = 0;

	// コース幅変更
	private bool mHasChangeCourseWidth = false;

	// Use this for initialization
	void Start () {
		// BGM
		if (GameObject.Find ("BGM(Clone)") == null) {
			Instantiate (mBGMPrefab);
		}

		// 速度
		mSpeed = 0.0f;
		mMoveDir = Vector3.up;

		// 角速度
		mRotVel = 0.0f;

		// スライダー
		mSliderLeft.enabled = false;
		mSliderRight.enabled = false;

		// SE
		mAudioSourceDestroy = GetComponent<AudioSource>();

		// やられ
		mIsDestroyed = false;

		// タイム
		mTime = 0.0f;

		// 残り時間
		mTimeLeft = mTimeLimit;

		// スター
		mStar = 0;

		// ゴール
		mIsGoal = false;

		// カメラサイズ
		// GameObject.Find ("SliderCameraSize").GetComponent<Slider> ().value = sSliderCameraSizeValue;

		// 無敵
		// GameObject.Find("ToggleInvincible").GetComponent<Toggle>().isOn = sIsInvincible;

		// 距離
		mCoursePartIndex = 0;

		// コース幅変更
		mHasChangeCourseWidth = false;
	}
	
	// Update is called once per frame
	void Update () {
		// スライダー
		if (!mSliderLeft.enabled && (mTime >= 0.1f)) {
			mSliderLeft.enabled = true;
			mSliderRight.enabled = true;
		}

		// sSliderCameraSizeValue = GameObject.Find ("SliderCameraSize").GetComponent<Slider> ().value;
		// GameObject.FindWithTag ("MainCamera").GetComponent<Camera> ().orthographicSize = Mathf.Lerp (5.0f, 200.0f, sSliderCameraSizeValue);

		// sIsInvincible = GameObject.Find ("ToggleInvincible").GetComponent<Toggle> ().isOn;

		if (mIsDestroyed) {
			// if (Input.GetKey (KeyCode.Space) || ((Input.touchCount > 0 ) && (Input.GetTouch(0).phase == TouchPhase.Began))) {
			//	Application.LoadLevel ("Game");
			// }
			return;
		}

		CalcMove ();

		CalcCollision ();

		CalcParticle ();

		mCoursePartIndex = mCourseManager.CheckInsideCoursePart (transform.position);
		if (mCoursePartIndex < 0)
			mCoursePartIndex = 0;
		mTextDistance.text = mCoursePartIndex.ToString ();

		// コース幅変更
		float achievement = mCourseManager.CheckAchievement(transform.position);
		if (!mHasChangeCourseWidth && (achievement >= 0.5f)) {
			mCourseManager.RequestChangeCourseWidth (CourseManager.CourseWidth * 0.8f);
			mHasChangeCourseWidth = true;
		}

		if (!mIsGoal) {
			mTime += Time.deltaTime;
			mTimeLeft -= Time.deltaTime;
			if (mTimeLeft < 0.0f)
				mTimeLeft = 0.0f;
		}
		// mTextTime.text = mTime.ToString ("F2");
		mTextTime.text = mTimeLeft.ToString("F1");

		mTextStar.text = mStar.ToString ();

		if (!mIsGoal && mCourseManager.CheckGoal(transform.position))
		{
			mIsGoal = true;
			// GameObject.Find ("TextGoal").GetComponent<Text> ().text = "GOAL!";

			mGameOverManager.GameOver (true, mCoursePartIndex, mStar, mTimeLeft);
			mStarManager.OnGameOver ();
		}
	}

	void CalcMove()
	{
		// 加速度
		float accLeft = Mathf.Lerp (mAccLow, mAccHigh, mSliderLeft.value);
		float accRight = Mathf.Lerp (mAccLow, mAccHigh, mSliderRight.value);

		// 角加速度
		float rotAccLeft = Mathf.Lerp (mRotAccLow, mRotAccHigh, mSliderLeft.value);
		float rotAccRight = Mathf.Lerp (mRotAccLow, mRotAccHigh, mSliderRight.value);

		// 回転ダンパー
		mRotVel *= Mathf.Pow(mRotKd, Time.deltaTime);

		// 回転更新
		mRotVel += ( - rotAccLeft + rotAccRight ) * Time.deltaTime;

		// 回転
		transform.RotateAround(transform.position, Vector3.forward, mRotVel);
		mMoveDir = Vector3.Slerp (mMoveDir, transform.up, mMoveDirKf);

		// ダンパー
		mSpeed *= Mathf.Pow (mKd, Time.deltaTime);

		// 加速
		mSpeed += (accLeft + accRight) * Time.deltaTime;

		// 位置更新
		Vector3 pos = transform.position;
		pos += (mBaseSpeed * Time.deltaTime + mSpeed ) * mMoveDir;
		transform.position = pos;
	}

	void CalcCollision()
	{
		if (sIsInvincible)
			return;
		
		if (mCollisionManager.CheckSphereCollision (transform.position, 0.1f) ||
			(mCourseManager.CheckInsideCoursePart(transform.position) < 0) ||
			(mTimeLeft <= 0.0f)) {
			mIsDestroyed = true;
			Destroy (GetComponent<SpriteRenderer> ());
			mAudioSourceDestroy.PlayOneShot (mAudioSourceDestroy.clip);
			iTween.ShakePosition (GameObject.FindWithTag ("MainCamera"), iTween.Hash ("x", 0.3f, "y", 0.3f, "time", 0.2f));
			mParticleSystemCrushSmoke.transform.position = transform.position;
			mParticleSystemCrushSmoke.GetComponent<ParticleSystem> ().Play ();
			mParticleSystemSmokeLeft.GetComponent<ParticleSystem> ().Stop ();
			mParticleSystemSmokeRight.GetComponent<ParticleSystem> ().Stop ();

			if (!mIsGoal) {
				mGameOverManager.GameOver (false, mCoursePartIndex, mStar, mTimeLeft);
				mStarManager.OnGameOver ();
			}
		}

		if (mStarManager.CheckSphereCollision (transform.position, 0.1f)) {
			++mStar;
		}
	}

	void CalcParticle()
	{
		mParticleSystemSmokeLeft.transform.position = transform.position - transform.up * 0.3f - transform.right * 0.07f;
		mParticleSystemSmokeRight.transform.position = transform.position - transform.up * 0.3f + transform.right * 0.085f;

		mParticleSystemSmokeLeft.GetComponent<ParticleSystem> ().startSize = Mathf.Lerp (mSmokeStartSizeLow, mSmokeStartSizeHigh, mSliderLeft.value);
		mParticleSystemSmokeRight.GetComponent<ParticleSystem> ().startSize = Mathf.Lerp (mSmokeStartSizeLow, mSmokeStartSizeHigh, mSliderRight.value);
	}
}
