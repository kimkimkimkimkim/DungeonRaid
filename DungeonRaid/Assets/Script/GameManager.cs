using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

	public GameObject attackPrefab;
	public GameObject defensePrefab;
	public GameObject goldPrefab;
	public GameObject expPrefab;
	public GameObject enemyPrefab;
	public GameObject linePrefab;
	public GameObject[] lines = new GameObject[6];

	private List<GameObject> drops = new List<GameObject>();
	private List<GameObject> removableBallList = new List<GameObject>();
	private List<Vector3>  lineArr = new List<Vector3>();
	private GameObject firstDrop;
	private string currentName;

	public enum TagDrop {
		dropAttack,
		dropDefense,
		dropGold,
		dropExp,
		dropEnemy
	}

	public enum TagCenter{
		centerAttack,
		centerDefense,
		centerGold,
		centerExp,
		centerEnemy
	}


	// Use this for initialization
	void Start () {
		//dropsにドロッププレハブを追加
		drops.Add(attackPrefab);
		drops.Add(defensePrefab);
		drops.Add(goldPrefab);
		drops.Add(expPrefab);
		drops.Add(enemyPrefab);
		//ドロップを生成
		CreateDrop(0,6);
		CreateDrop(1,6);
		CreateDrop(2,6);
		CreateDrop(3,6);
		CreateDrop(4,6);
		CreateDrop(5,6);
	}

	private void Update () {
		//if(!canTouch)return;
		/*
		if (Input.GetMouseButton(0)) //マウスがクリックされたら
        {	
			PointerEventData pointer = new PointerEventData(EventSystem.current);
			pointer.position = Input.mousePosition;
			List<RaycastResult> result = new List<RaycastResult>();
			EventSystem.current.RaycastAll(pointer, result);

			foreach (RaycastResult raycastResult in result)
			{
				//何かをドラッグしている時
				GameObject colObj = raycastResult.gameObject;
				Debug.Log(colObj.name);
			}
        }
		*/
		if (Input.GetMouseButtonDown (0) && firstDrop == null) {
			//ボールをドラッグし始めた時
			OnDragStart ();
		} else if (Input.GetMouseButtonUp (0)) {
			//ボールをドラッグし終わった時
			OnDragEnd ();
		} else if (firstDrop != null) {
			//ボールをドラッグしている途中
			OnDragging ();
		}
    }

	private void OnDragStart(){
		PointerEventData pointer = new PointerEventData(EventSystem.current);
		pointer.position = Input.mousePosition;
		List<RaycastResult> result = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointer, result);

		foreach (RaycastResult raycastResult in result)
		{
			//何かをドラッグしている時
			GameObject colObj = raycastResult.gameObject;
			if(raycastResult.gameObject.name.IndexOf("Drop_") != -1){
				Debug.Log(raycastResult.gameObject.name);
				//名前に"Ball"を含むオブジェクトをドラッグした時
				/*
				removableBallList = new List<GameObject>(); 
				firstBall = colObj; //はじめにドラッグしたボールを現在のボールに設定
				*/

				firstDrop = colObj;
				currentName = colObj.name; //現在のリストのボールの名前を設定

				//CheckList(colObj);
			}
		}
	}

	private void OnDragEnd(){
		if(firstDrop != null){
			//1つ以上のボールをなぞっている時
			int length = removableBallList.Count;
			//Debug.Log("リストの要素数: " + removableBallList.Count);
			if (length >= 3) {
				//消去するリストに３個以上のボールがあれば
				/*
				lineNum++;
				ballArr.Add(removableBallList);
				
				for(int i=0;i<length;i++){
					removableBallList[i].name = "_" + currentName;
				}
				//3つ目だったら攻撃
				if(lineNum == 3){
					ballArr.Add(removableBallList);
					this.GetComponent<BattleManager>().Attack(ballArr);

					//ballArr.Clear();
					lineNum = 0;
				}

				
				//currentScore += (CalculateBaseScore(length) + 50 * length);
				//StartCoroutine ("DropBall", length); //消した分だけボールを生成
				*/


			} else {
				//消去するリストに3個以上ボールがない時
				/*
				for (int j = 0; j < length; j++) {
					GameObject listedBall = removableBallList [j];
					ChangeColor (listedBall, 1.0f); //アルファ値を戻す
					listedBall.transform.GetChild(0).gameObject.SetActive(false); //点を非表示
					//listedBall.name = listedBall.name.Substring (1, 5);
				}
				for(int i=0;i<containerLine.transform.GetChild(lineNum).childCount;i++){
					Destroy(containerLine.transform.GetChild(lineNum).GetChild(i).gameObject);
				}
				*/
			}

			firstDrop = null; //変数の初期化
		}
	}

	private void OnDragging(){
		PointerEventData pointer = new PointerEventData(EventSystem.current);
		pointer.position = Input.mousePosition;
		List<RaycastResult> result = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointer, result);

		foreach (RaycastResult raycastResult in result)
		{
			//何かをドラッグしている時
			GameObject colObj = raycastResult.gameObject;
			if(raycastResult.gameObject.name.IndexOf("center") != -1){
				Debug.Log(raycastResult.gameObject.name);
				//名前に"Ball"を含むオブジェクトをドラッグした時
				/*
				removableBallList = new List<GameObject>(); 
				firstBall = colObj; //はじめにドラッグしたボールを現在のボールに設定
				currentName = colObj.name; //現在のリストのボールの名前を設定
				*/

				//CheckList(colObj);
			}
		}
	}

	//指定したLineに指定した個数のドロップを生成する
	//lineNum → 左から何番目のLineか（一番左が0）
	//dropCount　→ 落としたいドロップの個数
	private void CreateDrop(int lineNum,int dropCount){
		GameObject line = lines[lineNum];
		for(int i=0;i<dropCount;i++){
			int ran = UnityEngine.Random.Range(0,drops.Count); //何番目のドロップにするかの乱数
			GameObject drop = (GameObject)Instantiate(drops[ran]); //ドロップを生成
			drop.transform.SetParent(line.transform,false); //親をlineに指定
			float height = drop.GetComponent<RectTransform>().sizeDelta.y; //ドロップの高さを取得
			drop.transform.localPosition = new Vector3(0,i * height,0); //ドロップの座標を決定
		}
	}

}
