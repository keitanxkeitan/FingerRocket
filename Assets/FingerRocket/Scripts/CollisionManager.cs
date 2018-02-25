using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

	//----------------------------------
	// メンバ変数
	//----------------------------------

	public CourseManager mCourseManager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool CheckSphereCollision(Vector3 pos, float radius)
	{
		if (mCourseManager.CheckSphereCollision (pos, radius)) {
			return true;
		}
		return false;
	}
}
