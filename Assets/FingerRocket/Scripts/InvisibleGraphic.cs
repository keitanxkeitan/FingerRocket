//  InvisibleGraphic.cs
//  http://kan-kikuchi.hatenablog.com/entry/InvisibleGraphic
//
//  Created by kan.kikuchi on 2016.04.18.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 見えないGraphic(uGUIでボタンの当たり判定を大きくする用)
/// </summary>
public class InvisibleGraphic : Graphic {

	//頂点を生成する必要があるときのコールバック関数
	protected override void OnPopulateMesh(VertexHelper vh){
		base.OnPopulateMesh (vh);
		vh.Clear ();//頂点を全てクリアし、何も表示されないように
	}

	//表示する必要がないので、インスペクターに何も表示しないように
	#if UNITY_EDITOR

	[CustomEditor(typeof(InvisibleGraphic))]
	class GraphicCastEditor : Editor{
		public override void OnInspectorGUI() {
		}
	}

	#endif

}