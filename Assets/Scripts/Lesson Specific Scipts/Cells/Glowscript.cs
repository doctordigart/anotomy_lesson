using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Glowscript : MonoBehaviour{
	//creates an array of public gameobjects
	public GlowObjects[] glowObjects;

	public float glowIntensity = 1f;

//	void OnEnable(){
//		AnimationEventCaller.OnAnimationHasFinnished += StopThing;
//	}
//
//	void OnDisable(){
//		AnimationEventCaller.OnAnimationHasFinnished -= StopThing;
//	}

	public void DoThing(int i){
		if (i >= glowObjects.Length) {
			return;
		}
		foreach (GameObject go in glowObjects[i].objectsToGlow) {
			go.GetComponent<Renderer> ().material.SetFloat ("_MKGlowPower", glowIntensity);
		}
	}

	public void DoThingTran(int i){
	}

	public void StopThing(int i){
		if (i >= glowObjects.Length) {
			return;
		}
		foreach (GameObject go in glowObjects[i].objectsToGlow) {
			go.GetComponent<Renderer> ().material.SetFloat ("_MKGlowPower", 0f);
		}	
	}
}

[System.Serializable]
public class GlowObjects{
	public GameObject[] objectsToGlow;
}