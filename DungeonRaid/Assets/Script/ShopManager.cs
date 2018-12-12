using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour {

	public GameObject battleManager;

	public void ClickOKButton(){
		battleManager.GetComponent<BattleManager>().ShopOK();
	}
}
