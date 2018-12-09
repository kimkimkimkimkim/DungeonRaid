using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public GameManager attackPrefab;
	public GameManager defensePrefab;
	public GameManager goldPrefab;
	public GameManager expPrefab;
	public GameManager enemyPrefab;
	public GameObject[] lines = new GameObject[6];

	// Use this for initialization
	void Start () {
		
	}

	//指定したLineに指定した個数のドロップを生成する
	//lineNum → 左から何番目のLineか
	//dropCount　→ 落としたいドロップの個数
	private void CreateDrop(int lineNum,int dropCount){

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
