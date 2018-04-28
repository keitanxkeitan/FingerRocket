using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour {

	//----------------------------------
	// パラメータ
	//----------------------------------

	private const int cScoreDistance = 1;
	private const int cScoreStarNoGoal = 1;
	private const int cScoreStarGoal = 100;
	private const int cScoreGoalTimeBonus = 100;

	//----------------------------------
	// メンバ変数
	//----------------------------------

	private GameObject mGameOverBackground = null;

	// スライダー
	[SerializeField] private Slider mSliderLeft;
	[SerializeField] private Slider mSliderRight;

	// 距離
	[SerializeField] private Text mTextDistance;

	// 残り時間
	[SerializeField] private Text mTextTime;

	// スコア（リザルト）
	[SerializeField] private Text mTextResultScore;

	// ベスト（リザルト）
	[SerializeField] private Text mTextResultBest;

	// ランク（リザルト）
	[SerializeField] private Text mTextResultRank;

	// 距離（リザルト）
	[SerializeField] private Text mTextResultDistance;

	// スター（リザルト）
	[SerializeField] private Text mTextResultStar;

	// ゴールボーナス（リザルト）
	[SerializeField] private Text mTextResultGoalBonus;

	// ランキングマネージャ
	[SerializeField] private RankingManager mRankingManager;

	// ゲームオーバー
	private bool mIsGameOver = false;

	// ゴール
	private bool mHasGoal = false;

	// 経過時間
	private float mGameOverTime = 0.0f;

	// Use this for initialization
	void Start () {
		mGameOverBackground = GameObject.Find ("GameOverBackground");
		Debug.Assert (mGameOverBackground);

		mIsGameOver = false;

		mGameOverTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (mIsGameOver) {
			mGameOverTime += Time.deltaTime;

			if (mGameOverTime >= 1.0f) {
				if (Input.GetKey (KeyCode.Space) || ((Input.touchCount > 0) && (Input.GetTouch (0).phase == TouchPhase.Began))) {
					Application.LoadLevel ("Game");
				}
			}
		}

		mTextResultRank.text = "World Rank " + (mRankingManager.Ranking > 0 ? mRankingManager.Ranking.ToString () : "?");
	}

	public void GameOver(bool isGoal, int distance, int star, float time)
	{
		mIsGameOver = true;
		mHasGoal = isGoal;

		int score = CalcScore (isGoal, distance, star, time);

		// 保存
		int bestScore;
		{
			bestScore = PlayerPrefs.GetInt ("BestScore", -1);
			bool isLastSaveSucceeded = (PlayerPrefs.GetInt ("LastSaveSucceeded", 1) == 1);
			if ((bestScore < 0) || (score > bestScore) || !isLastSaveSucceeded)
			{
				bestScore = score;
				PlayerPrefs.SetInt ("BestScore", score);

				mRankingManager.SaveScore (score, isGoal, star, time, PlayerPrefs.GetInt("PlayNum"));
				mRankingManager.FetchRanking ();
			}
		}

		iTween.ValueTo(gameObject, iTween.Hash("from", 0.0f, "to", 0.65f, "time", 0.8f, "onupdate", "SetBackgroundAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", 0.8f, "onupdate", "SetTextAlpha"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", 0.8f, "onupdate", "SetTextResultAlpha"));
		iTween.ScaleTo (mSliderLeft.gameObject, iTween.Hash ("x", 0.0f, "y", 0.0f, "time", 0.4f, "easetype", "easeInBack"));
		iTween.ScaleTo (mSliderRight.gameObject, iTween.Hash ("x", 0.0f, "y", 0.0f, "time", 0.4f, "easetype", "easeInBack"));

		mTextResultScore.text = score.ToString ();
		mTextResultBest.text = "Best " + bestScore.ToString ();
		mTextResultRank.text = "World Rank " + (mRankingManager.Ranking > 0 ? mRankingManager.Ranking.ToString () : "?");
		mTextResultDistance.text = "Sector " + distance.ToString () + " x " + cScoreDistance.ToString () + " = " + (distance * cScoreDistance).ToString ();
		mTextResultStar.text = "Gem " + star.ToString () + " x " + (isGoal ? cScoreStarGoal : cScoreStarNoGoal).ToString () + " = " + (star * (isGoal ? cScoreStarNoGoal : cScoreStarGoal)).ToString ();
		mTextResultGoalBonus.text = "Goal Bonus " + time.ToString("F2") + " x " + cScoreGoalTimeBonus.ToString() + " = " + CalcGoalBonus (isGoal, time).ToString ();

		// 広告
		{
			int playNum = PlayerPrefs.GetInt ("PlayNum", 0);
			if (playNum > 0 && (playNum % 5 == 0) && (bestScore >= 10) && !isGoal) {
				GameObject.Find ("UnityAds").GetComponent<UnityAds> ().ShowAd ();
			}
		}
	}

	void SetBackgroundAlpha(float alpha) {
		mGameOverBackground.GetComponent<SpriteRenderer>().color = new Color(0,0,0, alpha);
	}

	void SetTextAlpha(float alpha) {
		{
			Color color = mTextDistance.color;
			color.a = alpha;
			mTextDistance.color = color;
		}
		{
			Color color = mTextTime.color;
			color.a = alpha;
			mTextTime.color = color;
		}
	}

	void SetTextResultAlpha(float alpha) {
		{
			Color color = mTextResultScore.color;
			color.a = alpha;
			mTextResultScore.color = color;
		}
		{
			Color color = mTextResultBest.color;
			color.a = alpha;
			mTextResultBest.color = color;
		}
		{
			Color color = mTextResultRank.color;
			color.a = alpha;
			mTextResultRank.color = color;
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
		return distance * cScoreDistance + star * (isGoal? cScoreStarGoal : cScoreStarNoGoal) + CalcGoalBonus (isGoal, time);
	}

	int CalcGoalBonus(bool isGoal, float time)
	{
		if (!isGoal)
			return 0;

		return (int)(time * cScoreGoalTimeBonus);
	}
}
