using UnityEngine;
using System.Collections;


public class PlayAnimation : MonoBehaviour {

	public AnimationClip introClip;

	public AnimationClip[] animations;

	public AnimationClip[] tranAnimations;

	public void DoThing(int i){
		if (i >= animations.Length) {
			return;
		}
		// if sent the -1 intro play intro clip
		if (i == -1) {
			GetComponent<Animation> ().clip = introClip;
			GetComponent<Animation> ().Play ();
		}
		// else play the appropriate animation based on the tour item being sent
		else {
			//if (animations [i] != null) {
				GetComponent<Animation> ().clip = animations [i];
				GetComponent<Animation> ().Play ();
			}
		//}

	}

	public void DoThingTran(int i){
		if (i >= tranAnimations.Length) {
			return;
		}
		// if sent the -1 intro play intro clip
		if (i == -1) {
			GetComponent<Animation> ().clip = introClip;
			GetComponent<Animation> ().Play ();
		}
		// else play the appropriate animation based on the tour item being sent
		else {
		//	if (animations [i] != null) {
				GetComponent<Animation> ().clip = tranAnimations [i];
				GetComponent<Animation> ().Play ();
			}
		//}

	}

	public void StopThing(){
		GetComponent<Animation> ().Rewind ();
		GetComponent<Animation> ().Play ();
		GetComponent<Animation> ().Sample ();
		GetComponent<Animation> ().Stop ();
	}

}