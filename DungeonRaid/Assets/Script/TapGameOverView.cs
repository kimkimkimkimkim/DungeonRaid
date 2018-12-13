using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapGameOverView : MonoBehaviour {

	public bool canTap = false;

	//Sceneの再読み込み
	public void ReLoad(){
		if(!canTap)return;
		SceneManager.LoadScene("GameScene"); // シーンの名前かインデックスを指定
	}
}
