using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreItem : MonoBehaviour {

	public Text questionName;

	public Text isCorrectText;

	public Text timeText;


	// called by script when score item object is intsantiated
	public void SetTextElements(TourItem tourItem){
		// draw the name of the question
		questionName.text = tourItem.itemName;
		// draw text to say if item is correct
		if (tourItem.isCorrect) {
			isCorrectText.text = "Correct";
			isCorrectText.color = Color.green;
		} else {
			isCorrectText.text = "Incorrect";
			isCorrectText.color = Color.red;
		}
		// draw the time it took to answer the question
		timeText.text = tourItem.answerTime.ToString();
	}

}
