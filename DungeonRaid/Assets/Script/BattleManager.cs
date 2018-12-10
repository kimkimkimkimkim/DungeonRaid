﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleManager : MonoBehaviour {

	public GameObject maincamera; //カメラ
	public GameObject gameManager;
	public GameObject textAtkBase; //基礎攻撃力
	public GameObject textAtk; //攻撃力テキスト
	public GameObject textDef; //守備力テキスト
	public GameObject textHp; //HPテキスト
	public GameObject sliderHp; //HPスライダー
	public GameObject sliderGold; //ゴールドスライダー
	public GameObject sliderExp; //経験値スライダー
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

	//EnemyAttack
	private void EnemyAttack(){
		StartCoroutine(DelayMethod(0.5f,() => {
			sliderHp.transform.parent.parent.gameObject.GetComponent<CameraShake>().Shake(0.5f,20.1f);
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