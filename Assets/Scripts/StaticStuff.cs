using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;

public static class StaticStuff {
	public static School loggedInSchool;
	public static Teacher loggedInTeacher;
	public static Lesson currentLesson;
	public static List<LessonItem> currentLessonItems;
	public static TourMode tourMode;
	public static Student loggedInStudent;
	public static List<TourItem> studentScores = new List<TourItem>();
}

public enum TourMode {tour,quiz};




[System.Serializable]
public class ServerData{
	public string assetBundlesURL;
	public string ourPhpURL;
}


[System.Serializable]
public class School{
	public string school_name;
	public string school_password;
	public string school_php_url;
}

[System.Serializable]
public class Teacher{
	public string teacherName;
	public string teacherPassword;
	public string teacherEmail;
}
	
[System.Serializable]
public class Student{
	public string studentName;
	public string studentPassword;
	public string studentEmail;
}

[System.Serializable]
public class Lesson {
	public string lesson_name;
	public int lesson_items;
	public string lesson_intro;
	public string lesson_conclusion;
}

[System.Serializable]
public class LessonItem{
	public string item_name;
	public int item_index;
	public string item_lecture;
	public string item_question;
	public string item_answer;
}

[System.Serializable]
public class UIStuff{
	public SchoolLogin schoolLogin;
	public TeacherLogin teacherLogin;
	public LessonSelect lessonSelect;
	public GameObject loadingPannel;
	public RectTransform loadingProgressBar;
}

[System.Serializable]
public class SchoolLogin{
	public GameObject pannel;
	public InputField loginNameInput;
	public InputField loginPasswordInput;
	public Text loginFeedback;
}

[System.Serializable]
public class TeacherLogin{
	public GameObject pannel;
	public InputField loginNameInput;
	public InputField loginPasswordInput;
	public Text loginFeedback;
}

[System.Serializable]
public class LessonSelect{
	public GameObject pannel;
	public GameObject lessonSelectUI;
	public Text currentLessonText;
	public int currentLessonIdex;
	public List<Lesson> addedLessons;
}

[System.Serializable]
public class SceneMonitor{
	public enum MenuState{start,login,homeScreen,loadingLaunch}
	public MenuState menuState;
	public Transform dolly;
}


[System.Serializable]
public enum BuildType{
	PC,Android,Cardboard,GearVR
}

[System.Serializable]
public class UserCameraMovement{
	public Transform cameraTransform;

	public Vector2 _mouseAbsolute;
	public Vector2 _smoothMouse;

	public Vector2 clampInDegrees = new Vector2(360, 180);
	public bool lockCursor;
	public Vector2 sensitivity = new Vector2(2, 2);
	public Vector2 smoothing = new Vector2(3, 3);
	public Vector2 targetDirection;
	public Vector2 targetCharacterDirection;
}



[System.Serializable]
public class UISelector{
	public LayerMask selectableObjectsLayer;
	public GameObject lookedAtObject;
	public Transform lookObject;
	public Renderer crossHair;
}



// class to hold variables about managing the tour/quiz
[System.Serializable]
public class TourGuide{
	// transform of the player
	public Transform playerTrans;
	// speach at introduction of tour
	public string introductionTour;
	// introduction speach for quiz
	public string introductionQuiz;
	// conclusion speach of tour
	public string conclusion;
	// introduction lets start button
	public GameObject letsStartButton;
	// gameobjects to do stuff in intro
	public GameObject[] introActiveObjects;
	// index of our current tour item
	public int currentTourItem;
	// the UI gameObject for continueing tour and asnwering quwstions
	public GameObject uiObject;
}



// class to hold the items of the tour/quiz
[System.Serializable]
public class TourItem{
	// name of item
	public string itemName;
	// the lecture for the item
	public string lecture;
	// question asked in quiz
	public string question;
	// correct answer
	public string answer;
	// answe given by student
	public string answerGiven;
	// was the answer given the correct one
	public bool isCorrect;
	// time it takes student to answer 
	public float answerTime;
	// transform that the UI snaps to
	public Transform uiTransform;
	// game objects to trigger to do stuff 
	public GameObject[] activeObjects;
	// gameobjects that are triggered in transitions
	public GameObject[] activeTransitionObjects;
}


// login page for students in student lobby page
[System.Serializable]
public class StudentLoginPage{
	// pannel for student login page
	public GameObject studentLoginPannel;
	// where students enter their name
	public InputField studentNameInput;
	// where students enter their password
	public InputField studentPasswordInput;
	// the text writtem on the login button
	public Text studentLoginButtonText;
}


// main page for student lobby
[System.Serializable]
public class StudentMainPage{
	// pannel for student lobby main page
	public GameObject studentMainPagePannel;

}


//class for displaying quiz results to students
[System.Serializable]
public class StudentScoresPage{
	// main pannel displaying page
	public GameObject studentScoresPagePannel;
	// the pannel that the score items are added to that scrolls
	public GameObject studentsScoreScrollPannel;
	// prefab object of scoreitem
	public GameObject scoreItem;

}


// class for student calander
[System.Serializable]
public class StudentCalanderPage{
	public GameObject StudentCalanderPannel;

	public List <Memo> memos;

	public Text yearText;
	public Text monthText;

	public GameObject[] datesButtons;

	public DateTime dateTime;

	public Calendar cal = CultureInfo.InvariantCulture.Calendar;

	public int memoDay;

	public void Start(){
		dateTime = DateTime.Now;
		SetCalander();
	}

	public void AddYear(int years){
		dateTime = dateTime.AddYears (years);
		SetCalander ();
	}

	public void AddMonth(int months){
		dateTime = dateTime.AddMonths (months);
		SetCalander ();
	}
		
	void SetCalander(){
		SetYear ();
		SetMonth ();
		SetUpDays ();
	}
		
	void SetYear(){
		yearText.text = cal.GetYear (dateTime).ToString();
	}
		
	void SetMonth(){
		switch(cal.GetMonth(dateTime)){
		case 1:
			monthText.text = "January";
			break;
		case 2:
			monthText.text = "Febuary";
			break;
		case 3:
			monthText.text = "March";
			break;
		case 4:
			monthText.text = "April";
			break;
		case 5:
			monthText.text = "May";
			break;
		case 6:
			monthText.text = "June";
			break;
		case 7:
			monthText.text = "July";
			break;
		case 8:
			monthText.text = "August";
			break;
		case 9:
			monthText.text = "September";
			break;
		case 10:
			monthText.text = "October";
			break;
		case 11:
			monthText.text = "November";
			break;
		case 12:
			monthText.text = "December";
			break;
		}

	}

	void SetUpDays(){
		// turn off all the dates buttons
		foreach (GameObject datesButton in datesButtons) {
			datesButton.transform.GetComponentInChildren<Text>().text = "";
			datesButton.transform.GetComponentInChildren<Text>().color = Color.black;
			datesButton.GetComponent<Button> ().interactable = false;

		}
		// get the starting day of the week of the month 
		int weekDayOffest;
		DateTime dTime = new DateTime (dateTime.Year, dateTime.Month, 1);
		DayOfWeek firstDayOfMonth = dTime.DayOfWeek;
		int dayOfWeekOffset = new int();
		switch(firstDayOfMonth){
		case DayOfWeek.Sunday:
			dayOfWeekOffset = 0;
			break;
		case DayOfWeek.Monday:
			dayOfWeekOffset = 1;
			break;
		case DayOfWeek.Tuesday:
			dayOfWeekOffset = 2;
			break;
		case DayOfWeek.Wednesday:
			dayOfWeekOffset = 3;
			break;
		case DayOfWeek.Thursday:
			dayOfWeekOffset = 4;
			break;
		case DayOfWeek.Friday:
			dayOfWeekOffset = 5;
			break;
		case DayOfWeek.Saturday:
			dayOfWeekOffset = 6;
			break;
		}
		// turn on dates buttons and write the date on them
		for (int i = 0; i < cal.GetDaysInMonth (dateTime.Year, dateTime.Month); i++) {
			datesButtons [i + dayOfWeekOffset].transform.GetComponentInChildren<Text> ().text = (i + 1).ToString ();
			datesButtons [i + dayOfWeekOffset].GetComponent<Button> ().interactable = true;
			// if the button represnts today highlight the date text
			if (DateTime.Now.Day == i + 1 && DateTime.Now.Month == dateTime.Month && DateTime.Now.Year == dateTime.Year) {
				datesButtons [i + dayOfWeekOffset].transform.GetComponentInChildren<Text>().color = Color.red;
			}
		}

	}
}

[System.Serializable]
public class Memo{
	public string memoDate;
	public string memoTitle;
	public string memoStudent;
	public string memoText;
}

[System.Serializable]
public class MemoListPage{
	public GameObject memoListPagePannel;
	public Text topDate;
	public GameObject memoItemPrefab;
	public GameObject memoListParent;
}

[System.Serializable]
public class MemoPage{
	public GameObject memoPagePannel;
	public Text memoTitleText;
	public Text memoText;
}