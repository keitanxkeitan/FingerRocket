using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarManager : MonoBehaviour {

	//----------------------------------
	// パラメータ
	//----------------------------------

	// スター半径
	[SerializeField] private static float mStarRadius = 0.2f;

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// スタープレハブ
	public GameObject mStarPrefab;

	// スターリスト
	private List<GameObject> mStars = new List<GameObject>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// スター生成
	public GameObject InstantiateStar(Vector3 pos)
	{
		GameObject star = Instantiate (mStarPrefab, pos, Quaternion.identity);
		mStars.Add (star);
		return star;
	}

	public bool CheckSphereCollision(Vector3 pos, float radius)
	{
		foreach(GameObject star in mStars)
		{
			if (Vector3.Distance (pos, star.transform.position) <= (radius + mStarRadius)) {
				mStars.Remove (star);
				star.GetComponent<Star> ().OnGet ();
				return true;
			}
		}
		return false;
	}

	public void OnGameOver()
	{
		foreach (GameObject star in mStars) {
			star.GetComponent<Star> ().OnGameOver ();
		}
	}
}
