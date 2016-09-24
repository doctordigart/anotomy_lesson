using UnityEngine;
using System.Collections;

public class MaterialScript : MonoBehaviour {

	public string WhatToSay;
	
	public void LookAtBegan(){
		GetComponent<Renderer> ().material.color = Color.blue;
	}

	public void LookAtEnd(){
		GetComponent<Renderer> ().material.color = Color.white;
	}

	public void Click(){
		GetComponent<Renderer> ().material.color = Color.white;
		GetComponent<Animation> ().Play ();
		GameObject.Find("scriptholder").SendMessage("SayOnce", WhatToSay);
	
	}
}
