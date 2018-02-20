using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	//----------------------------------
	// メンバ変数
	//----------------------------------

	// ロケット
	[SerializeField] private GameObject mRocket;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		// カメラ位置更新
		Vector3 pos = transform.position;
		pos.x = mRocket.transform.position.x;
		pos.y = mRocket.transform.position.y;
		transform.position = pos;
	}
}
