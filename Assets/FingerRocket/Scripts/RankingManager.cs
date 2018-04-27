using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NCMB;

public class RankingManager : MonoBehaviour {

	private const string cClassName = "Ranking";
	private const string cPrefsObjectId = "ObjectId";
	private const string cScoreKey = "score";

	private int mRanking;
	public int Ranking
	{
		get { return mRanking; }
	}

	static bool sIsFirstTime = true;

	// Use this for initialization
	void Start () {
		if (sIsFirstTime) {
			mRanking = -1;
			sIsFirstTime = false;
		}

		FetchRanking ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SaveScore(int score)
	{
		string objectId = PlayerPrefs.GetString (cPrefsObjectId, "");
		bool isFirstTime = (objectId == "");

		NCMBObject ranking = new NCMBObject (cClassName);

		if (!isFirstTime)
		{
			ranking.ObjectId = objectId;
		}

		ranking [cScoreKey] = score;

		ranking.SaveAsync ((NCMBException e) => {
			if(e != null) {
				Debug.Log("Save Failed");
			} else {
				Debug.Log("Save Succeeded");
				Debug.Log(ranking.ObjectId);
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
		query.OrderByAscending (cScoreKey);

		Debug.Log ("query");

		query.FindAsync ((List<NCMBObject> objList ,NCMBException e) => {

			if (e == null) {
				int ranking = 1;
				foreach( NCMBObject obj in objList ){
					if(obj.ObjectId == objectId)
					{
						mRanking = ranking;
						break;
					}
					++ranking;
				}
			} else {
				Debug.Log("Query Failed");
			}
		});
	}
}
