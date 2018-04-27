using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;
using System;

public class RankingManager : MonoBehaviour {

	private const string cClassName = "Ranking";
	private const string cPrefsObjectId = "ObjectId";
	private const string cScoreKey = "Score";
	private const string cIsGoalKey = "IsGoal";
	private const string cStarKey = "Star";
	private const string cTimeKey = "Time";
	private const string cPlayNumKey = "PlayNum";

	private int mRanking;
	public int Ranking
	{
		get { return mRanking; }
	}

	static bool sIsFirstTime = true;

	// Use this for initialization
	void Start () {
		// プレイ回数
		int playNum = PlayerPrefs.GetInt ("PlayNum", 0);
		++playNum;
		PlayerPrefs.SetInt ("PlayNum", playNum);

		if (sIsFirstTime) {
			mRanking = -1;
			sIsFirstTime = false;
		}

		FetchRanking ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SaveScore(int score, bool isGoal, int star, float time, int playNum)
	{
		string objectId = PlayerPrefs.GetString (cPrefsObjectId, "");
		bool isFirstTime = (objectId == "");

		NCMBObject ranking = new NCMBObject (cClassName);

		if (!isFirstTime)
		{
			ranking.ObjectId = objectId;
		}

		ranking [cScoreKey] = score;
		ranking [cIsGoalKey] = isGoal;
		ranking [cStarKey] = star;
		ranking [cTimeKey] = time;
		ranking [cPlayNumKey] = playNum;

		ranking.SaveAsync ((NCMBException e) => {
			if(e != null) {
				Debug.Log("Save Failed");
			} else {
				Debug.Log("Save Succeeded");
				Debug.Log(ranking.ObjectId);
				PlayerPrefs.SetString(cPrefsObjectId, ranking.ObjectId);
				FetchRanking();
			}
		});
	}

	public void FetchRanking()
	{
		// mRanking = -1;

		string objectId = PlayerPrefs.GetString (cPrefsObjectId, "");

		if (objectId == "") {
			return;
		}

		NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject> (cClassName);
		query.OrderByDescending (cScoreKey);

		Debug.Log ("query");

		query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

			if (e == null) {
				int ranking = 1;
				int sameScoreNum = 0;
				int prevScore = int.MaxValue;
				foreach( NCMBObject obj in objList ){
					int score = Convert.ToInt32(obj[cScoreKey]);
					
					if(score != prevScore)
					{
						ranking += sameScoreNum;
						sameScoreNum = 1;
						prevScore = score;
					}
					else
					{
						++sameScoreNum;
					}

					if(obj.ObjectId == objectId)
					{
						mRanking = ranking;
						break;
					}
				}
			} else {
				Debug.Log("Query Failed");
			}
		});
	}
}
