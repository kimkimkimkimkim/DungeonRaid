using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour {

	public GameObject battleManager;

	//OKボタンをおす
	public void ClickOKButton(){
		battleManager.GetComponent<BattleManager>().LevelUpOK();
	}

}
