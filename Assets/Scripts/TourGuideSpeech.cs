using UnityEngine;
using System.Collections;
using SpeechLib;
using System.IO;


public class TourGuideSpeech : MonoBehaviour {


	private SpVoice voice;

	void Awake(){
		voice = new SpVoice();
	}
		


	public void Say (string whatToSay) {
		voice.Speak (whatToSay, SpeechVoiceSpeakFlags.SVSFlagsAsync);
		StartCoroutine (WaitForSpeach ());
	}

	public void SayOnce (string whatToSay) {
		voice.Speak (whatToSay, SpeechVoiceSpeakFlags.SVSFlagsAsync);
		//StartCoroutine (WaitForSpeach ());
	}

	IEnumerator WaitForSpeach(){
		while (!voice.WaitUntilDone (1)) {
			yield return null;
		}
		transform.SendMessage ("DoneSpeaking");
	}

}
