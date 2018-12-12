using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class BattleManager : MonoBehaviour {

	public GameObject maincamera; //カメラ
	public GameObject particlePrefab; //攻撃エフェクトパーティクルのプレハブ
	public GameObject gameManager;
	public GameObject textAtkBase; //基礎攻撃力
	public GameObject textAtk; //攻撃力テキスト
	public GameObject textDef; //守備力テキスト
	public GameObject textHp; //HPテキスト
	public GameObject sliderHp; //HPスライダー
	public GameObject sliderGold; //ゴールドスライダー
	public GameObject sliderExp; //経験値スライダー
	public GameObject imageDim; //操作不可能を示すための画像
	public GameObject field; //フィールド
	public GameObject shop; //ショップ
	public GameObject levelup; //レベルアップ
	public int atkBase; //基礎攻撃力
	public int atkNow; //現在の攻撃力
	public int atkRise; //攻撃力上昇値
	public int maxDef; //最大守備力
	public int nowDef; //現在の守備力
	public int maxHp; //最大体力
	public int nowHp; //現在の体力
	public int maxGold; //最大ゴールド
	public int nowGold; //所持ゴールド
	public int maxExp; //最大経験値
	public int nowExp; //経験値


	// Use this for initialization
	void Start () {
		//UIの更新
		UpdateUIAtk(atkBase);
		UpdateUIDef();
		UpdateUIHp();
		UpdateUIGold();
		UpdateUIExp();
	}

	//レベルアップ
	private void LevelUp(){
		if(nowExp >= maxExp){
			//LevelUpする
			nowExp = 0;
			UpdateUIExp();
			levelup.SetActive(true);
		}else{
			//LevelUpしない
			Shop();
		}
	}

	//レベルアップのOKボタンを押したら
	public void LevelUpOK(){
		levelup.SetActive(false);
		Shop();
	}


	//ショップ
	private void Shop(){
		if(nowGold >= maxGold){
			//Shopを開く
			nowGold = 0;
			UpdateUIGold();
			shop.SetActive(true);
		}else{
			//Shopを開かない
			imageDim.SetActive(false);
			gameManager.GetComponent<GameManager>().canScreenTouch = true;
		}
	}

	//ショップのOKボタン押したら
	public void ShopOK(){
		imageDim.SetActive(false);
		gameManager.GetComponent<GameManager>().canScreenTouch = true;
		shop.SetActive(false);
	}

	//EnemyAttack
	private void EnemyAttack(){
		int atkEnemy = 0; //敵の総攻撃力
		int lineCount = field.transform.childCount; //ラインの個数
		for(int i=0;i<lineCount;i++){
			GameObject line = field.transform.GetChild(i).gameObject;
			int dropCount = line.transform.childCount;
			for(int j=0;j<dropCount;j++){
				GameObject drop = line.transform.GetChild(j).gameObject; //ドロップ
				if(drop.name.IndexOf("Enemy") != -1){
					//ドロップがEnemyドロップなら
					atkEnemy += int.Parse(drop.transform.Find("TextAttack").gameObject.GetComponent<Text>().text);
				}
			}
		}
		imageDim.SetActive(true); //Dimの表示
		gameManager.GetComponent<GameManager>().canScreenTouch = false; //画面操作不可にする
		if(atkEnemy == 0){
			LevelUp();
			return; //敵の攻撃がない場合すぐリターン
		}
		//敵の攻撃がある場合

		//攻撃開始
		StartCoroutine(DelayMethod(0.5f,() => {
			for(int i=0;i<lineCount;i++){
				GameObject line = field.transform.GetChild(i).gameObject;
				int dropCount = line.transform.childCount;
				for(int j=0;j<dropCount;j++){
					GameObject drop = line.transform.GetChild(j).gameObject; //ドロップ
					if(drop.name.IndexOf("Enemy") != -1){
						//ドロップがEnemyドロップなら
						//int attackId = charaState[i];
						float timeAnim = 1f;
						/* circleの作成 */
						GameObject particle = (GameObject)Instantiate(particlePrefab);
						particle.transform.position = drop.transform.position;

						//軌跡設定
						float disX = 0.5f;
						float disY = 0.5f;
						Vector3[] path = {
							new Vector3(particle.transform.position.x + (disX * (UnityEngine.Random.Range(0,6)-2))
								, particle.transform.position.y + (disY * (j-5)),0f), //中間地点
							sliderHp.transform.parent.parent.position, //到達点
						};

						//DOTweenを使ったアニメ作成
						particle.transform.DOPath(path,timeAnim,PathType.CatmullRom)
							.SetEase(Ease.OutSine);

						/* Pirticleの消去とエフェクトの表示 */
						/*
						StartCoroutine(DelayMethod(timeAnim,() => {
							imageDim.SetActive(false);
							gameManager.GetComponent<GameManager>().canScreenTouch = true;
							nowHp -= atkEnemy;
							Debug.Log("nowHp : " + nowHp);
							UpdateUIHp();
							sliderHp.transform.parent.parent.gameObject.GetComponent<CameraShake>().Shake(0.5f,20.1f);
						}));
						*/
						
					}
				}
			}

			//攻撃終了
			StartCoroutine(DelayMethod(1f,() => {
				//imageDim.SetActive(false);
				//gameManager.GetComponent<GameManager>().canScreenTouch = true;
				nowHp -= atkEnemy;
				Debug.Log("nowHp : " + nowHp);
				UpdateUIHp();
				sliderHp.transform.parent.parent.gameObject.GetComponent<CameraShake>().Shake(0.5f,20.1f);

				StartCoroutine(DelayMethod(0.6f,() => {
					LevelUp();
				}));
			}));
		
		}));
	}

	//Action
	public void Attack(List<GameObject> removableBallList){
		UpdateUIAtk(atkBase);
		int length = removableBallList.Count;
		if(length <= 2)return; //配列の要素が2つ以下だったらリターン
		int[] count = new int[]{0,0,0,0,0,0};
		int atk = atkBase;
		//攻撃力計算
		for(int i=0;i<length;i++){
			if(removableBallList[i].name.IndexOf("Attack") != -1)atk += atkRise; //Attackだったら攻撃力上昇
		}
		//敵を倒せるか計算
		for(int i=0;i<length;i++){
			if(removableBallList[i].name.IndexOf("Enemy") != -1){
				//敵だったら
				int hpEnemy = int.Parse(removableBallList[i].transform.Find("TextHp").GetComponent<Text>().text);
				if(hpEnemy - atk <= 0){
					//敵を倒せる
					Destroy(removableBallList[i]); //ドロップを削除
					count[removableBallList[i].GetComponent<DropManager>().line]++; //どのLineのオブジェクトなのかカウント
					//経験値取得
					nowExp++;
				}else{
					//敵が倒せない
					removableBallList[i].transform.Find("TextHp").GetComponent<Text>().text = (hpEnemy - atk).ToString();
				}
			}else{
				//Attackだったら
				Destroy(removableBallList[i]); //ドロップを削除
				count[removableBallList[i].GetComponent<DropManager>().line]++; //どのLineのオブジェクトなのかカウント
			}
		}

		UpdateUIExp();

		for(int i=0;i<6;i++){
			gameManager.GetComponent<GameManager>().CreateDrop(i,count[i]);
		}
		StartCoroutine(DelayMethod(0.05f,() => {
			gameManager.GetComponent<GameManager>().MoveDrop();
		}));

		//相手のターン
		EnemyAttack();

	}

	public void Defense(List<GameObject> removableBallList){
		int length = removableBallList.Count;
		int[] count = new int[]{0,0,0,0,0,0};
		for(int i=0;i<length;i++){
			Destroy(removableBallList[i]); //ドロップを削除
			count[removableBallList[i].GetComponent<DropManager>().line]++; //どのLineのオブジェクトなのかカウント
		}
		for(int i=0;i<6;i++){
			gameManager.GetComponent<GameManager>().CreateDrop(i,count[i]);
		}
		StartCoroutine(DelayMethod(0.05f,() => {
			gameManager.GetComponent<GameManager>().MoveDrop();
		}));

		//更新
		nowDef = (nowDef+length > maxDef)? maxDef : nowDef + length; //経験値更新
		UpdateUIDef();

		//相手のターン
		EnemyAttack();
	}

	public void Gold(List<GameObject> removableBallList){
		int length = removableBallList.Count;
		int[] count = new int[]{0,0,0,0,0,0};
		for(int i=0;i<length;i++){
			Destroy(removableBallList[i]); //ドロップを削除
			count[removableBallList[i].GetComponent<DropManager>().line]++; //どのLineのオブジェクトなのかカウント
		}
		for(int i=0;i<6;i++){
			gameManager.GetComponent<GameManager>().CreateDrop(i,count[i]);
		}
		StartCoroutine(DelayMethod(0.05f,() => {
			gameManager.GetComponent<GameManager>().MoveDrop();
		}));

		//更新
		nowGold += length; //経験値更新
		UpdateUIGold();

		//相手のターン
		EnemyAttack();
	}

	public void Exp(List<GameObject> removableBallList){
		int length = removableBallList.Count;
		int[] count = new int[]{0,0,0,0,0,0};
		for(int i=0;i<length;i++){
			Destroy(removableBallList[i]); //ドロップを削除
			count[removableBallList[i].GetComponent<DropManager>().line]++; //どのLineのオブジェクトなのかカウント
		}
		for(int i=0;i<6;i++){
			gameManager.GetComponent<GameManager>().CreateDrop(i,count[i]);
		}
		StartCoroutine(DelayMethod(0.05f,() => {
			gameManager.GetComponent<GameManager>().MoveDrop();
		}));

		//更新
		nowExp += length; //経験値更新
		UpdateUIExp();

		//相手のターン
		EnemyAttack();
	}

	public void Hp(List<GameObject> removableBallList){
		int length = removableBallList.Count;
		int[] count = new int[]{0,0,0,0,0,0};
		for(int i=0;i<length;i++){
			Destroy(removableBallList[i]); //ドロップを削除
			count[removableBallList[i].GetComponent<DropManager>().line]++; //どのLineのオブジェクトなのかカウント
		}
		for(int i=0;i<6;i++){
			gameManager.GetComponent<GameManager>().CreateDrop(i,count[i]);
		}
		StartCoroutine(DelayMethod(0.05f,() => {
			gameManager.GetComponent<GameManager>().MoveDrop();
		}));

		//更新
		nowHp = (nowHp + length > maxHp)? maxHp : nowHp + length; //経験値更新
		UpdateUIHp();

		//相手のターン
		EnemyAttack();
	}

	//UpdateUI
	private void UpdateUIAtk(int nowAtk){

		if(int.Parse(textAtkBase.GetComponent<Text>().text) < nowAtk){
			//攻撃力が上昇していたら
			//textAtkBaseをはねさせる
			float timeAnim = 0.5f;
			float dist = 50f;
			iTween.MoveTo(textAtkBase, iTween.Hash("y",dist,"time",timeAnim/2,"easetype",iTween.EaseType.easeOutCirc
			,"isLocal",true));
			iTween.MoveTo(textAtkBase, iTween.Hash("delay",timeAnim/2 + 0.01f,"y",0,"time",timeAnim/2
				,"easetype",iTween.EaseType.easeInCirc,"isLocal",true));
		}
		
		textAtkBase.GetComponent<Text>().text = nowAtk.ToString();
		textAtk.GetComponent<Text>().text = " + " + atkRise;
	}

	private void UpdateUIDef(){
		textDef.GetComponent<Text>().text = nowDef + "  /  " + maxDef;
	}

	private void UpdateUIHp(){
		textHp.GetComponent<Text>().text = nowHp + "/" + maxHp;
		sliderHp.GetComponent<Slider>().maxValue = maxHp;
		//sliderHp.GetComponent<Slider>().value = nowHp;
		iTween.ValueTo(gameObject, iTween.Hash("from",sliderHp.GetComponent<Slider>().value,"to",nowHp
			,"onupdate","UpdateHpSlider","onupdatetarget",gameObject));
	}

	private void UpdateHpSlider(float value){
		sliderHp.GetComponent<Slider>().value = value;
	}

	private void UpdateUIGold(){
		sliderGold.GetComponent<Slider>().maxValue = maxGold;
		//sliderGold.GetComponent<Slider>().value = nowGold;
		iTween.ValueTo(gameObject, iTween.Hash("from",sliderGold.GetComponent<Slider>().value,"to",nowGold
			,"onupdate","UpdateGoldSlider","onupdatetarget",gameObject));
	}

	private void UpdateGoldSlider(float value){
		sliderGold.GetComponent<Slider>().value = value;
	}

	private void UpdateUIExp(){
		sliderExp.GetComponent<Slider>().maxValue = maxExp;
		//sliderExp.GetComponent<Slider>().value = nowExp;
		iTween.ValueTo(gameObject, iTween.Hash("from",sliderExp.GetComponent<Slider>().value,"to",nowExp
			,"onupdate","UpdateExpSlider","onupdatetarget",gameObject));
	}

	private void UpdateExpSlider(float value){
		sliderExp.GetComponent<Slider>().value = value;
	}


	//敵を倒せるか判定する
	public void JudgeCanDefeat(List<GameObject> removableBallList){
		int atk = atkBase; //攻撃力
		for(int i=0;i<removableBallList.Count;i++){
			//攻撃力を加算
			if(removableBallList[i].name.IndexOf("Attack") != -1)atk += atkRise; //加算
		}
		for(int i=0;i<removableBallList.Count;i++){
			//敵を倒せるか判定
			if(removableBallList[i].name.IndexOf("Enemy") != -1){
				//敵の場合
				int hpEnemy = int.Parse(removableBallList[i].transform.Find("TextHp").gameObject.GetComponent<Text>().text);
				if(hpEnemy - atk <= 0){
					//敵を倒せる
					removableBallList[i].transform.GetChild(4).gameObject.SetActive(true);
				}else{
					//敵を倒せない
					removableBallList[i].transform.GetChild(4).gameObject.SetActive(false);
				}
			}
		}
		//UIに反映
		UpdateUIAtk(atk);
	}

	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}

}
