using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoursePart : MonoBehaviour {

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// 部品タイプ
	private CourseManager.PartType mPartType;

	// 位置
	private Vector3 mPos;

	public void Setup(CourseManager.PartType partType, Vector3 pos)
	{
		mPartType = partType;
		mPos = pos;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
