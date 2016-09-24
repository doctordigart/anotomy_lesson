using UnityEngine;
using System.Collections;

public class AnimationEventCaller : MonoBehaviour {

	public delegate void AnimationHasFinnished ();
	public static event AnimationHasFinnished OnAnimationHasFinnished;

	public void AnimationFinished(){
		OnAnimationHasFinnished ();
	}

}
