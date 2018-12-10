using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour {

	public int line;
	public int attack;
	public int hp;

	private void Update(){
		Vector3 position = transform.localPosition;
		//transform.localPosition = new Vector3(0,position.y,position.z);
	}
}
