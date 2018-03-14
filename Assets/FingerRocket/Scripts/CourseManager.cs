using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseManager : MonoBehaviour {

	//----------------------------------
	// 定数
	//----------------------------------

	// 部品タイプ
	public enum PartType
	{
		T2B,
		T2L,
		T2R,
		B2T,
		B2L,
		B2R,
		L2T,
		L2B,
		L2R,
		R2T,
		R2B,
		R2L,
		T2B_Sin,
		B2T_Sin,
		L2R_Sin,
		R2L_Sin,
		Num
	};

	// 方向
	private enum Dir
	{
		Up,
		Down,
		Left,
		Right,
		Num
	};

	//----------------------------------
	// パラメータ
	//----------------------------------

	// 部品サイズ
	[SerializeField] private static float mPartSize = 5.12f;
	public static float PartSize
	{
		get { return CourseManager.mPartSize; }
	}

	// コース幅
	[SerializeField] private static float mCourseWidth = 0.5f;
	public static float CourseWidth
	{
		get { return CourseManager.mCourseWidth; }
	}

	// 色
	[SerializeField] private static int mColorNum = 6;

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// 部品プレハブ
	public GameObject mPartPrefab;

	// 色
	public Color[] mCourseColor = new Color[mColorNum];
	public Color[] mBackgroundColor = new Color[mColorNum];

	// 部品リスト
	private List<GameObject> mParts = new List<GameObject>();

	// 色
	private static int sColorIndex = -1;

	// Use this for initialization
	void Start () {
		// 色決め
		int colorIndex = -1;
		do {
			colorIndex = Random.Range(0, mColorNum - 1);
		} while(sColorIndex == colorIndex);
		sColorIndex = colorIndex;

		// 背景色
		GameObject.FindWithTag ("MainCamera").GetComponent<Camera> ().backgroundColor = mBackgroundColor[sColorIndex];

		CreateCourse ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// 部品生成
	GameObject InstantiatePart(Vector3 pos, PartType partType, Color courseColor, Color backgroundColor)
	{
		GameObject part = Instantiate (mPartPrefab, pos, Quaternion.identity);
		Material material = part.GetComponent<SpriteRenderer> ().material;
		material.SetInt ("_PartType", (int)partType);
		material.SetColor ("_CourseColor", courseColor);
		material.SetColor ("_BackgroundColor", backgroundColor);
		material.SetFloat ("_CourseWidth", mCourseWidth);
		part.GetComponent<CoursePart> ().Setup (partType, pos);
		mParts.Add (part);
		return part;
	}

	// コース生成
	void CreateCourse()
	{
		Color courseColor = mCourseColor [sColorIndex];
		Color backgroundColor = mBackgroundColor [sColorIndex];

		const int cVisitedSize = 2048;
		bool[,] isVisited = new bool[cVisitedSize, cVisitedSize];
		for (int x = 0; x < cVisitedSize; ++x) {
			for (int y = 0; y < cVisitedSize; ++y) {
				if ((x == 0) || (x == cVisitedSize - 1) || (y == 0) || (y == cVisitedSize - 1)) {
					isVisited [y, x] = true;
				} else {
					isVisited [y, x] = false;
				}
			}
		}

		InstantiatePart (Vector3.zero - Vector3.up * mPartSize * 2, PartType.B2T, courseColor, backgroundColor);
		InstantiatePart (Vector3.zero - Vector3.up * mPartSize * 1, PartType.B2T, courseColor, backgroundColor);
		InstantiatePart (Vector3.zero - Vector3.up * mPartSize * 0, PartType.B2T, courseColor, backgroundColor);

		PartType prevPartType = PartType.B2T;
		Vector3 prevPartPos = Vector3.zero;
		Dir currDir = Dir.Up;
		int visitedPosX = cVisitedSize / 2;
		int visitedPosY = 1;
		isVisited [visitedPosY, visitedPosX] = true;
		int partNum = 0;

		List<PartType> partTypes = new List<PartType> ();
		do {
			partTypes.Clear();
			if ((currDir == Dir.Left) || (currDir == Dir.Right)) {
				bool isSpecial = false;
				if(partNum < 15)
				{
					isSpecial = false;
				}
				else if(partNum < 30)
				{
					isSpecial = Random.Range(0, 4) == 0;
				}
				else if(partNum < 60)
				{
					isSpecial = Random.Range(0, 2) == 0;
				}
				else if(partNum < 90)
				{
					isSpecial = Random.Range(0, 1) == 0;
				}
				else
				{
					isSpecial = true;
				}
				if(isSpecial)
				{
					List<int> table = new List<int>();
					if(partNum < 30)
					{
						table.Add(2);
						table.Add(2);
						table.Add(3);
						table.Add(3);
					}
					else if(partNum < 60)
					{
						table.Add(2);
						table.Add(2);
						table.Add(3);
						table.Add(3);
						table.Add(1);
						table.Add(1);
					}
					else if(partNum < 90)
					{
						table.Add(2);
						table.Add(2);
						table.Add(3);
						table.Add(3);
						table.Add(4);
						table.Add(4);
						table.Add(4);
						table.Add(4);
					}
					else
					{
						table.Add(1);
						table.Add(1);
						table.Add(1);
						table.Add(1);
						table.Add(2);
						table.Add(2);
						table.Add(3);
						table.Add(3);
						table.Add(3);
						table.Add(3);
					}

					int sum = 0;
					foreach(int i in table)
					{
						sum += i;
					}
					int random = Random.Range(0, sum);
					sum = 0;
					int pattern = 0;
					foreach(int i in table)
					{
						sum += i;
						if(random < sum)
						{
							break;
						}
						++pattern;
					}

					MakeSpecialPartTypes(pattern, partTypes);

					if(currDir == Dir.Right)
					{
						MirrorPartTypes(partTypes);
					}

					Dir dir = currDir;
					int vX = visitedPosX;
					int vY = visitedPosY;
					foreach(PartType partType in partTypes)
					{
						Vector2 delta2 = Dir2Delta2(dir);
						vX += (int)delta2.x;
						vY += (int)delta2.y;
						if(isVisited[vY, vX])
						{
							partTypes.Clear();
							break;
						}
						dir = PartType2Dir(partType);
					}
				}
			}

			if (partTypes.Count == 0) {
				PartType partType = Dir2RandomPartType (currDir, true);
				partTypes.Add(partType);
			}

			for(int i = 0; i < partTypes.Count; ++i) {
				if (partNum >= 15 && Random.Range(0, 4) == 0) {
					switch(partTypes[ i ]) {
					case PartType.T2B:
						partTypes[ i ] = PartType.T2B_Sin;
						break;
					case PartType.B2T:
						partTypes[ i ] = PartType.B2T_Sin;
						break;
					case PartType.L2R:
						partTypes[ i ] = PartType.L2R_Sin;
						break;
					case PartType.R2L:
						partTypes[ i ] = PartType.R2L_Sin;
						break;
					}
				}
			}

			foreach(PartType partType in partTypes)
			{
				Vector3 delta = Dir2Delta(currDir);
				Vector3 partPos = prevPartPos + delta;
				if(partNum == 127) {
					courseColor = mCourseColor[(sColorIndex + 1) % mColorNum];
					backgroundColor = mBackgroundColor[(sColorIndex + 1) % mColorNum];
				}
				InstantiatePart(partPos, partType, courseColor, backgroundColor);

				Vector2 delta2 = Dir2Delta2(currDir);
				visitedPosX += (int)delta2.x;
				visitedPosY += (int)delta2.y;
				isVisited[visitedPosY, visitedPosX] = true;

				prevPartPos = partPos;
				currDir = PartType2Dir(partType);

				++partNum;
			}
		} while (partNum < 128);
	}

	// 部品から方向
	Dir PartType2Dir(PartType partType)
	{
		switch (partType) {
		case PartType.B2T:
		case PartType.L2T:
		case PartType.R2T:
		case PartType.B2T_Sin:
			return Dir.Up;
		case PartType.T2B:
		case PartType.L2B:
		case PartType.R2B:
		case PartType.T2B_Sin:
			return Dir.Down;
		case PartType.T2L:
		case PartType.B2L:
		case PartType.R2L:
		case PartType.R2L_Sin:
			return Dir.Left;
		case PartType.T2R:
		case PartType.B2R:
		case PartType.L2R:
		case PartType.L2R_Sin:
			return Dir.Right;
		}
		return Dir.Up;
	}

	// 方向からテキトウな部品
	PartType Dir2RandomPartType(Dir dir, bool isForwardOnly)
	{
		PartType[] candidates = new PartType[(int)PartType.Num];
		int candidateNum = 0;

		switch (dir) {
		case Dir.Up:
			candidates [candidateNum++] = PartType.B2T;
			candidates [candidateNum++] = PartType.B2L;
			candidates [candidateNum++] = PartType.B2R;
			break;
		case Dir.Down:
			candidates [candidateNum++] = PartType.T2B;
			candidates [candidateNum++] = PartType.T2L;
			candidates [candidateNum++] = PartType.T2R;
			break;
		case Dir.Left:
			candidates [candidateNum++] = PartType.R2T;
			if(!isForwardOnly) candidates [candidateNum++] = PartType.R2B;
			candidates [candidateNum++] = PartType.R2L;
			break;
		case Dir.Right:
			candidates [candidateNum++] = PartType.L2T;
			if(!isForwardOnly) candidates [candidateNum++] = PartType.L2B;
			candidates [candidateNum++] = PartType.L2R;
			break;
		}

		int i = Random.Range (0, candidateNum);
		return candidates [i];
	}

	// 方向から移動量
	Vector3 Dir2Delta(Dir dir)
	{
		switch (dir) {
		case Dir.Up:
			return new Vector3 (0.0f, mPartSize, 0.0f);
		case Dir.Down:
			return new Vector3 (0.0f, -mPartSize, 0.0f);
		case Dir.Left:
			return new Vector3 (-mPartSize, 0.0f, 0.0f);
		case Dir.Right:
			return new Vector3 (mPartSize, 0.0f, 0.0f);
		}
		return Vector3.zero;
	}

	// 方向から移動量
	Vector2 Dir2Delta2(Dir dir)
	{
		switch (dir) {
		case Dir.Up:
			return new Vector2 (0.0f, 1.0f);
		case Dir.Down:
			return new Vector2 (0.0f, -1.0f);
		case Dir.Left:
			return new Vector2 (-1.0f, 0.0f);
		case Dir.Right:
			return new Vector2 (1.0f, 0.0f);
		}
		return Vector2.zero;
	}

	PartType MirrorPartType(PartType partType)
	{
		switch (partType) {
		case PartType.T2B:
		case PartType.B2T:
			return partType;
		case PartType.T2L:
			return PartType.T2R;
		case PartType.T2R:
			return PartType.T2L;
		case PartType.B2L:
			return PartType.B2R;
		case PartType.B2R:
			return PartType.B2L;
		case PartType.L2T:
			return PartType.R2T;
		case PartType.L2B:
			return PartType.R2B;
		case PartType.L2R:
			return PartType.R2L;
		case PartType.R2T:
			return PartType.L2T;
		case PartType.R2B:
			return PartType.L2B;
		case PartType.R2L:
			return PartType.L2R;
		}
		return PartType.T2B;
	}

	void MirrorPartTypes(List<PartType> partTypes)
	{
		for (int i = 0; i < partTypes.Count; ++i) {
			partTypes [i] = MirrorPartType (partTypes [i]);
		}
	}

	void MakeSpecialPartTypes(int pattern, List<PartType> partTypes)
	{
		switch(pattern)
		{
		case 0:
			/**
			 *	│┌
			 *	└┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2T);
			break;
		case 1:
			/**
			 *	┐┌
			 *	└┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2L);
			break;
		case 2:
			/**
			 * 	│┌
			 * 	││
			 * 	└┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2T);
			partTypes.Add (PartType.B2T);
			break;
		case 3:
			/**
			 * 	┐┌
			 * 	││
			 * 	└┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2T);
			partTypes.Add (PartType.B2L);
			break;
		case 4:
			/**
			 * 	│　┌
			 *	│┌┘
			 *	└┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2T);
			partTypes.Add (PartType.B2T);
			break;
		case 5:
			/**
			 * 	│　┌
			 *	│┌┘
			 *	└┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2T);
			partTypes.Add (PartType.B2L);
			break;
		case 6:
			/**
			 * 	│┌
			 *	│└┐
			 *	└─┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2R);
			partTypes.Add (PartType.L2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2T);
			partTypes.Add (PartType.B2T);
			break;
		case 7:
			/**
			 * 	┐┌
			 *	│└┐
			 *	└─┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2R);
			partTypes.Add (PartType.L2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2T);
			partTypes.Add (PartType.B2L);
			break;
		case 8:
			/**
			 *	　│┌
			 *	┌┘└┐
			 *	└──┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2R);
			partTypes.Add (PartType.L2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2L);
			partTypes.Add (PartType.R2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2R);
			partTypes.Add (PartType.L2T);
			partTypes.Add (PartType.B2T);
			break;
		case 9:
			/**
			 *	　┐┌
			 *	┌┘└┐
			 *	└──┘
			 */
			partTypes.Add (PartType.R2B);
			partTypes.Add (PartType.T2R);
			partTypes.Add (PartType.L2B);
			partTypes.Add (PartType.T2L);
			partTypes.Add (PartType.R2L);
			partTypes.Add (PartType.R2L);
			partTypes.Add (PartType.R2T);
			partTypes.Add (PartType.B2R);
			partTypes.Add (PartType.L2T);
			partTypes.Add (PartType.B2L);
			break;
		}
	}

	public bool CheckSphereCollision(Vector3 pos, float radius)
	{
		foreach(GameObject part in mParts)
		{
			if (part.GetComponent<CoursePart> ().CheckSphereCollision (pos, radius)) {
				return true;
			}	
		}
		return false;
	}

	public int CheckInsideCoursePart(Vector3 pos)
	{
		int i = 0;
		foreach (GameObject part in mParts) {
			if (part.GetComponent<CoursePart> ().CheckInside (pos)) {
				return i;
			}
			++i;
		}
		return -1;
	}
}
