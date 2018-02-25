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

	// 距離
	[SerializeField] private Text mTextDistance;

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

	// 移動距離
	private float mDistance;

	// Use this for initialization
	void Start () {
		// 速度
		mSpeed = 0.0f;
		mMoveDir = Vector3.up;

		// 角速度
		mRotVel = 0.0f;

		// SE
		mAudioSourceDestroy = GetComponent<AudioSource>();

		// やられ
		mIsDestroyed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (mIsDestroyed) {
			if (Input.GetKey (KeyCode.Space) || ((Input.touchCount > 0 ) && (Input.GetTouch(0).phase == TouchPhase.Began))) {
				Application.LoadLevel ("Game");
			}
			return;
		}

		CalcMove ();

		CalcCollision ();

		CalcParticle ();
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

		mDistance += mSpeed;
		mTextDistance.text = ((int)mDistance).ToString ();
	}

	void CalcCollision()
	{
		if (mCollisionManager.CheckSphereCollision (transform.position, 0.1f)) {
			mIsDestroyed = true;
			Destroy (GetComponent<SpriteRenderer> ());
			mAudioSourceDestroy.PlayOneShot (mAudioSourceDestroy.clip);
			iTween.ShakePosition (GameObject.FindWithTag ("MainCamera"), iTween.Hash ("x", 0.3f, "y", 0.3f, "time", 0.2f));
			mParticleSystemCrushSmoke.transform.position = transform.position;
			mParticleSystemCrushSmoke.GetComponent<ParticleSystem> ().Play ();
			mParticleSystemSmokeLeft.GetComponent<ParticleSystem> ().Stop ();
			mParticleSystemSmokeRight.GetComponent<ParticleSystem> ().Stop ();
		}
	}

	void CalcParticle()
	{
		mParticleSystemSmokeLeft.transform.position = transform.position - transform.up * 0.25f - transform.right * 0.05f;
		mParticleSystemSmokeRight.transform.position = transform.position - transform.up * 0.25f + transform.right * 0.05f;

		mParticleSystemSmokeLeft.GetComponent<ParticleSystem> ().startSize = Mathf.Lerp (0.1f, 0.15f, mSliderLeft.value);
		mParticleSystemSmokeRight.GetComponent<ParticleSystem> ().startSize = Mathf.Lerp (0.1f, 0.15f, mSliderRight.value);
	}
}
