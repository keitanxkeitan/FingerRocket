using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Advertisement.Initialize ("1678763");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowAd()
	{
		if (Advertisement.IsReady ())
			Advertisement.Show();
	}
}
