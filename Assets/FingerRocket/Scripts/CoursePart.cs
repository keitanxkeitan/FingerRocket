﻿using System.Collections;
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

	bool CheckCourseCurve(Vector3 pos, Vector3 center, float min, float max)
	{
		float squaredDistance = (pos.x - center.x) * (pos.x - center.x) + (pos.y - center.y) * (pos.y - center.y);
		return (squaredDistance >= (min * min)) && (squaredDistance <= (max * max));
	}

	bool CheckCourse(Vector3 posLocal, float radius)
	{
		float cPartSizeHalf = CourseManager.cPartSize * 0.5f;
			
		float min = -cPartSizeHalf * CourseManager.cCourseWidth + radius;
		float max = +cPartSizeHalf * CourseManager.cCourseWidth - radius;

		if ((mPartType == CourseManager.PartType.T2B) || (mPartType == CourseManager.PartType.B2T)) {
			return (posLocal.x >= min) && (posLocal.x <= max);
		} else if ((mPartType == CourseManager.PartType.L2R) || (mPartType == CourseManager.PartType.R2L)) {
			return (posLocal.y >= min) && (posLocal.y <= max);
		}

		min += cPartSizeHalf;
		max += cPartSizeHalf;

		if ((mPartType == CourseManager.PartType.T2L) || (mPartType == CourseManager.PartType.L2T)) {
			return CheckCourseCurve (posLocal, new Vector3 (-cPartSizeHalf, cPartSizeHalf, 0.0f), min, max);
		} else if ((mPartType == CourseManager.PartType.T2R) || (mPartType == CourseManager.PartType.R2T)) {
			return CheckCourseCurve (posLocal, new Vector3 (cPartSizeHalf, cPartSizeHalf, 0.0f), min, max);
		} else if ((mPartType == CourseManager.PartType.B2L) || (mPartType == CourseManager.PartType.L2B)) {
			return CheckCourseCurve (posLocal, new Vector3 (-cPartSizeHalf, -cPartSizeHalf, 0.0f), min, max);
		} else if ((mPartType == CourseManager.PartType.B2R) || (mPartType == CourseManager.PartType.R2B)) {
			return CheckCourseCurve (posLocal, new Vector3 (cPartSizeHalf, -cPartSizeHalf, 0.0f), min, max);
		}
		return false;
	}

	public bool CheckSphereCollision(Vector3 pos, float radius)
	{
		float cPartSizeHalf = CourseManager.cPartSize * 0.5f;
		Vector3 posLocal = pos - mPos;
		if ((posLocal.x < -cPartSizeHalf) || (posLocal.x > cPartSizeHalf) || (posLocal.y < -cPartSizeHalf) || (posLocal.y > cPartSizeHalf)) {
			return false;
		}
		return !CheckCourse (posLocal, radius);
	}
}