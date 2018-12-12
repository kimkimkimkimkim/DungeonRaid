using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour {

	public GameObject attackPrefab;
	public GameObject defensePrefab;
	public GameObject goldPrefab;
	public GameObject expPrefab;
	public GameObject enemyPrefab;
	public GameObject linePrefab;
	public GameObject containerLine;
	public GameObject battleManager;
	public GameObject[] lines = new GameObject[6];
	public bool canScreenTouch = true; //画面タッチを許可するかどうか

	private List<GameObject> drops = new List<GameObject>();
	private List<GameObject> removableBallList = new List<GameObject>();
	private List<Vector3>  lineArr = new List<Vector3>();
	private GameObject firstDrop; //最初のドロップ
	private GameObject lastDrop; //直前のドロップ
	private string currentName;
	private float heightDrop; //ドロップの高さ
	private float heightLine; //fieldの高さ

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
		//lineheightの取得
		heightLine = Screen.width;
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
		StartCoroutine(DelayMethod(0.05f,() => {
			MoveDrop ();
		}));
	}

	private void Update () {
		if(canScreenTouch){
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
				Debug.Log("local : " + colObj.transform.localPosition);
				Debug.Log("world : " + colObj.transform.position);
				//名前に"Drop_"を含むオブジェクトをドラッグした時
				ChangeColor(raycastResult.gameObject.name,0.35f); //関係ないやつは暗くする
				
				removableBallList = new List<GameObject>(); //ドラッグしているドロップを入れる配列を生成
				firstDrop = colObj; //ドロップ開始の合図
				currentName = colObj.name; //現在のリストのボールの名前を設定
				PushList(colObj); //リストに追加
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
				
				//currentNameによってActionの分岐
				
				if(currentName.IndexOf("Attack") != -1 || currentName.IndexOf("Enemy") != -1){
					//AttackかEnemyの時
					battleManager.GetComponent<BattleManager>().Attack(removableBallList);
				}else if(currentName.IndexOf("Defense") != -1){
					battleManager.GetComponent<BattleManager>().Defense(removableBallList);
				}else if(currentName.IndexOf("Gold") != -1){
					battleManager.GetComponent<BattleManager>().Gold(removableBallList);
				}else if(currentName.IndexOf("Exp") != -1){
					battleManager.GetComponent<BattleManager>().Hp(removableBallList);
				}

			} else {
				//消去するリストに3個以上ボールがない時
				battleManager.GetComponent<BattleManager>().Attack(removableBallList);
			}
			//Lineを削除
			for(int i=0;i<containerLine.transform.childCount;i++){
				Destroy(containerLine.transform.GetChild(i).gameObject);
			}
			firstDrop = null; //変数の初期化
			ChangeColor("",1);
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
				GameObject center = colObj;
				GameObject drop = colObj.transform.parent.gameObject;
				
				//centerをドラッグしたとき
				bool isExist = false; //リストの中にあるかどうか
				for(int i=0;i<removableBallList.Count;i++){
					if(removableBallList[i] == drop) isExist = true;
				}
				if(!isExist){
					//リストの中にない場合
					float dist = Vector2.Distance ((Vector2)lastDrop.transform.position, (Vector2)colObj.transform.position); //直前のドロップと現在のドロップとの距離
					//float distRef = heightDrop * (float)Math.Sqrt(2);
					float distRef = 1.5f;
					Debug.Log("lastPos : " + lastDrop.transform.localPosition );
					Debug.Log("nowPos : " + center.transform.localPosition);
					Debug.Log("dist : " + dist + "  distref : " + distRef);
					if(dist <= distRef){
						//距離が一定値以下のとき
						PushList(drop); //dropをリストに追加
					}
				}else if(removableBallList.Count >= 2){
					if(drop == removableBallList[removableBallList.Count-2]){
						//リストにあるけど直前のドロップだったとき
						PopList();
					}
				}
			}
		}
	}

	public void MoveDrop(){
		int lineCount =lines.Length; //ラインの個数
		//yield return new WaitForSeconds(0.05f);
		for(int i=0;i<lineCount;i++){
			GameObject line = lines[i];
			int dropCount = line.transform.childCount; //そのラインのドロップの個数
			for(int j=0;j<dropCount;j++){
				GameObject drop = line.transform.GetChild(j).gameObject; //ドロップ
				iTween.MoveTo(drop,iTween.Hash("position",new Vector3(0,-1 * heightLine/2 + (j*heightDrop),0)
					,"isLocal",true,"easetype",iTween.EaseType.easeInSine,"time",0.4f));
			}
		}
	}

	private void PushList(GameObject drop){
		removableBallList.Add(drop);
		lastDrop = drop;
		DrowLine();
	}

	private void PopList(){
		//EnemyDropだったら罰を非表示
		if(removableBallList[removableBallList.Count-1].name.IndexOf("Enemy") != -1){
			removableBallList[removableBallList.Count-1].transform.GetChild(4).gameObject.SetActive(false);
		}
		removableBallList.RemoveAt(removableBallList.Count-1); //末尾を削除
		lastDrop = removableBallList[removableBallList.Count-1];
		DrowLine();
	}

	private void DrowLine(){
		//削除
		for(int i=0;i<containerLine.transform.childCount;i++){
			Destroy(containerLine.transform.GetChild(i).gameObject);
		}

		for(int i=0;i<removableBallList.Count-1;i++){
			GameObject newLine = (GameObject)Instantiate(linePrefab);
			newLine.transform.SetParent(containerLine.transform,false);
        	//LineRenderer line = newLine.AddComponent<LineRenderer> ();
			LineRenderer line = newLine.GetComponent<LineRenderer>();
			line.SetPosition(0,removableBallList[i].transform.GetChild(1).position + new Vector3(0,0,-10));
			line.SetPosition(1,removableBallList[i+1].transform.GetChild(1).position + new Vector3(0,0,-10));
			line.startWidth = 0.1f;
			line.endWidth = 0.1f;
		}

		if(currentName.IndexOf("Attack") != -1 || currentName.IndexOf("Enemy") != -1){
			//敵を倒せるか判定
			battleManager.GetComponent<BattleManager>().JudgeCanDefeat(removableBallList);
		}
	}

	//色を変更する（暗くしたり、元に戻したり）
	//name → 色を変更しないオブジェクトの名前
	//color → 色
	private void ChangeColor(string name,float color){
		int lineCount =lines.Length; //ラインの個数
		for(int i=0;i<lineCount;i++){
			GameObject line = lines[i];
			int dropCount = line.transform.childCount; //そのラインのドロップの個数
			for(int j=0;j<dropCount;j++){
				GameObject drop = line.transform.GetChild(j).gameObject;
				if(name.IndexOf("Attack") != -1 || name.IndexOf("Enemy") != -1){
					//AttackかEnemyのとき
					//AttackかEnemy以外の時は色変更
					if(drop.name.IndexOf("Attack") == -1 && drop.name.IndexOf("Enemy") == -1){
						drop.transform.GetChild(0).GetComponent<Image>().color = new Color(color,color,color); //画像を暗く
						drop.transform.GetChild(1).gameObject.SetActive(false); //centerを非アクティブ
					}else{
						drop.transform.GetChild(1).gameObject.SetActive(true); //centerをアクティブ
					}
				}else{
					//AttackかEnemy以外
					//自分以外の種類は色変更
					if(drop.name != name){
						drop.transform.GetChild(0).GetComponent<Image>().color = new Color(color,color,color); //画像を暗く
						drop.transform.GetChild(1).gameObject.SetActive(false); //centerを非アクティブ
					}else{
						drop.transform.GetChild(1).gameObject.SetActive(true); //centerをアクティブ
					}
				}
			}
		}

	}

	//指定したLineに指定した個数のドロップを生成する
	//lineNum → 左から何番目のLineか（一番左が0）
	//dropCount　→ 落としたいドロップの個数
	public void CreateDrop(int lineNum,int dropCount){
		GameObject line = lines[lineNum];
		float linehight = line.GetComponent<RectTransform>().sizeDelta.y;
		for(int i=0;i<dropCount;i++){
			int ran = UnityEngine.Random.Range(0,drops.Count); //何番目のドロップにするかの乱数
			GameObject drop = (GameObject)Instantiate(drops[ran]); //ドロップを生成
			if(ran == 4){
				//ドロップがEnemyDropの時はステータスを入力
				//drop.GetComponent<DropManager>().attack = 1;
				//drop.GetComponent<DropManager>().hp = 5;
				drop.transform.Find("TextAttack").gameObject.GetComponent<Text>().text 
					= 1.ToString();
				drop.transform.Find("TextHp").gameObject.GetComponent<Text>().text 
					= 5.ToString();
			}
			drop.transform.SetParent(line.transform,false); //親をlineに指定
			heightDrop = drop.GetComponent<RectTransform>().sizeDelta.y; //ドロップの高さを取得
			drop.transform.localPosition = new Vector3(0,linehight/2 + (i) * heightDrop,0); //ドロップの座標を決定
			drop.GetComponent<DropManager>().line = lineNum;
		}
	}

	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

}
