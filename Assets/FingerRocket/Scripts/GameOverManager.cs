using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour {

	//----------------------------------
	// メンバ変数
	//----------------------------------

	private GameObject mGameOverBackground = null;

	// スライダー
	[SerializeField] private Slider mSliderLeft;
	[SerializeField] private Slider mSliderRight;

	// 距離
	[SerializeField] private Text mTextDistance;

	// スコア（リザルト）
	[SerializeField] private Text mTextResultScore;

	// 距離（リザルト）
	[SerializeField] private Text mTextResultDistance;

	// スター（リザルト）
	[SerializeField] private Text mTextResultStar;

	// ゴールボーナス（リザルト）
	[SerializeField] private Text mTextResultGoalBonus;

	// ゴール
	private bool mHasGoal = false;

	// Use this for initialization
	void Start () {
		mGameOverBackground = GameObject.Find ("GameOverBackground");
		Debug.Assert (mGameOverBackground);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GameOver(bool isGoal, int distance, int star, float time)
	{
		mHasGoal = isGoal;

		iTween.ValueTo(gameObject, iTween.Hash("from", 0.0f, "to", 0.65f, "time", 0.8f, "onupdate", "SetBackgroundAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", 0.8f, "onupdate", "SetTextDistanceAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.8f, "onupdate", "SetTextResultAlpha"));
		iTween.ScaleTo (mSliderLeft.gameObject, iTween.Hash ("x", 0.0f, "y", 0.0f, "time", 0.4f, "easetype", "easeInBack"));
		iTween.ScaleTo (mSliderRight.gameObject, iTween.Hash ("x", 0.0f, "y", 0.0f, "time", 0.4f, "easetype", "easeInBack"));

		mTextResultScore.text = "Score " + CalcScore (isGoal, distance, star, time).ToString ();
		mTextResultDistance.text = "Sector " + distance.ToString ();
		mTextResultStar.text = "Gem " + star.ToString ();
		mTextResultGoalBonus.text = "Bonus " + CalcGoalBonus (isGoal, time).ToString ();
	}

	void SetBackgroundAlpha(float alpha) {
		mGameOverBackground.GetComponent<SpriteRenderer>().color = new Color(0,0,0, alpha);
	}

	void SetTextDistanceAlpha(float alpha) {
		Color color = mTextDistance.color;
		color.a = alpha;
		mTextDistance.color = color;
	}

	void SetTextResultAlpha(float alpha) {
		{
			Color color = mTextResultScore.color;
			color.a = alpha;
			mTextResultScore.color = color;
		}
		{
			Color color = mTextResultDistance.color;
			color.a = alpha;
			mTextResultDistance.color = color;
		}
		{
			Color color = mTextResultStar.color;
			color.a = alpha;
			mTextResultStar.color = color;
		}
		if(mHasGoal)
		{
			Color color = mTextResultGoalBonus.color;
			color.a = alpha;
			mTextResultGoalBonus.color = color;
		}
	}

	int CalcScore(bool isGoal, int distance, int star, float time)
	{
		return distance + star + CalcGoalBonus (isGoal, time);
	}

	int CalcGoalBonus(bool isGoal, float time)
	{
		if (!isGoal)
			return 0;

		return 100;
	}
}
