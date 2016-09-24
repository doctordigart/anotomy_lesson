using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class LessonLobby : MonoBehaviour {

	// enum just to keep track of build type
	[Header("What our target build is")]
	public BuildType buildType;

	// info for student login page
	[Header("Student Login Page")]
	public StudentLoginPage studentLoginPage;

	// info for student main page
	[Header("Student Main Page")]
	public StudentMainPage studentMainPage;

	// info for page that displays students scores
	[Header("Students Scores Page")]
	public StudentScoresPage studentsScoresPage;

	// class for student calander
	[Header("Student Calander Class")]
	public StudentCalanderPage studentCalanderPage;

	//class for listing the memos on a page
	[Header("Memo List Page")] 
	public MemoListPage memoListPage;

	// class containing stuff related to the memo page
	[Header("Memo Page")]
	public MemoPage memoPage;

	//subscribe to some events
	void OnEnable(){
		MemoItem.OnClickedMemoItemButton += OnClickedMemoItem;
	}

	void OnDisable(){
		MemoItem.OnClickedMemoItemButton -= OnClickedMemoItem;
	}
		

	// when the lobby level starts sort out whether to start the studnt login or main page or display the students quiz scores
	void Start(){
		// if theres no student logged in open the student log in page
		if (StaticStuff.loggedInStudent == null) {
			studentLoginPage.studentLoginPannel.SetActive (true);
		}
		// if already logged in will see if we are begining or returning from the quiz
		else {
			// if starting or returning from the tour we open the students main page
			if (StaticStuff.tourMode == TourMode.tour) {
				studentMainPage.studentMainPagePannel.SetActive (true);
			} 
			// if we are returning from a quiz we open the scores page
			else if(StaticStuff.tourMode == TourMode.quiz) {
				// call the method for displaying the scores of the students
				DisplayScoresPage ();
			}
		}

	}


	// called when user clicks the student login button
	public void Button_StudentLogin(){
		// get info from server and test if login info is good
		StartCoroutine (CheckStudentLogin ());
	}

	//called when student clicks logout button at end of quiz
	public void Button_StudentLogout(){
		// turn off the students scores page and destroy the added scores from the last student
		studentsScoresPage.studentScoresPagePannel.SetActive(false);
		foreach (Transform scoreItemTran in studentsScoresPage.studentsScoreScrollPannel.transform) {
			Destroy (scoreItemTran.gameObject);
		}
		// reset static variables regarding the current student
		StaticStuff.loggedInStudent = new Student();
		StaticStuff.studentScores.Clear ();
		// reset it for the next student to login
		StaticStuff.tourMode = TourMode.tour;
		studentLoginPage.studentLoginPannel.SetActive (true);
	}


	// called when student clicks take tour
	public void Button_TakeTour(){
		// set to tour mode
		StaticStuff.tourMode = TourMode.tour;
		// load lesson level
		SceneManager.LoadScene ("Lesson");
	}

	// called when the student clicks play quiz button
	public void Button_TakeQuiz(){
		// set to quiz mode
		StaticStuff.tourMode = TourMode.quiz;
		// load out lesson scene
		SceneManager.LoadScene("Lesson");
	}

	// called when student clicks the calander button in the main menu
	public void Button_Calander(){
		studentCalanderPage.Start ();
		studentMainPage.studentMainPagePannel.SetActive (false);
		//studentCalanderPage.StudentCalanderPannel.SetActive (true);
		StartCoroutine(GetMemos());
	}

	// button to index calander years
	public void Button_AddYear(int years){
		studentCalanderPage.AddYear (years);
		DrawMemoDays ();
	}

	// button in the calander to index months
	public void Button_AddMont(int months){
		studentCalanderPage.AddMonth (months);
		DrawMemoDays ();
	}


	// draws the days apropriately on the calander
	void DrawMemoDays(){
		foreach (GameObject calBut in studentCalanderPage.datesButtons) {
			calBut.GetComponent<Image> ().color = Color.white;
		}
		foreach (GameObject calBut in studentCalanderPage.datesButtons) {
			if (calBut.GetComponent<Button> ().interactable) {
				int buttonDay = Int32.Parse (calBut.GetComponentInChildren<Text> ().text);
				DateTime dt = new DateTime (studentCalanderPage.dateTime.Year, studentCalanderPage.dateTime.Month, buttonDay);
				foreach (Memo m in studentCalanderPage.memos) {
					if (dt == DateTime.Parse (m.memoDate)) {
						calBut.GetComponent<Image> ().color = Color.green;
					}
				}
			}
		}
	}



	public void Button_DaySelected(GameObject dayButton){
		// if the button is not holding a menu return out
		if (dayButton.GetComponent<Image>().color == Color.white) {
			return;
		}
		studentCalanderPage.memoDay = Int32.Parse(dayButton.GetComponentInChildren<Text> ().text);
		studentCalanderPage.StudentCalanderPannel.SetActive (false);
		memoListPage.memoListPagePannel.SetActive (true);
		memoListPage.topDate.text = studentCalanderPage.dateTime.Month.ToString () + "/" +
			studentCalanderPage.memoDay.ToString () + "/" +
			studentCalanderPage.dateTime.Year.ToString ();
		DrawAddedMemos ();
	}

	public void Button_CalanderBack(){
		studentCalanderPage.StudentCalanderPannel.SetActive (false);
		studentMainPage.studentMainPagePannel.SetActive (true);
	}

	public void Button_MemoListBack(){
		ClearAddedMemos ();
		memoListPage.memoListPagePannel.SetActive (false);
		studentCalanderPage.StudentCalanderPannel.SetActive (true);
	}


	public void Button_MemoBack(){
		memoPage.memoPagePannel.SetActive (false);
		memoListPage.memoListPagePannel.SetActive (true);
		memoPage.memoTitleText.text = "";
		memoPage.memoText.text = "";
	}


	void DrawAddedMemos(){
		ClearAddedMemos ();
		DateTime dt = new DateTime (studentCalanderPage.dateTime.Year, studentCalanderPage.dateTime.Month, studentCalanderPage.memoDay);
		foreach (Memo m in studentCalanderPage.memos) {
			if(dt == DateTime.Parse(m.memoDate)){
				GameObject item;
				item = GameObject.Instantiate (memoListPage.memoItemPrefab) as GameObject;
				item.GetComponent<RectTransform>().SetParent(memoListPage.memoListParent.transform,false);
				item.GetComponent<MemoItem> ().SetMemoItem(m);
			}
		}

	}

	void ClearAddedMemos(){
		for (int c = 0; c < memoListPage.memoListParent.transform.childCount; c++) {
			Destroy (memoListPage.memoListParent.transform.GetChild(c).gameObject);
		}
	}


	// called when a student suessfully logs in
	void OnStudentLogin(Student s){
		// turn off the login page
		studentLoginPage.studentLoginPannel.SetActive (false);
		// turn on the main page
		studentMainPage.studentMainPagePannel.SetActive(true);
		// store student as static variable
		StaticStuff.loggedInStudent = s;
	}


	// called when a memo item in the list is clicked
	void OnClickedMemoItem(Memo m){
		memoPage.memoTitleText.text = m.memoTitle;
		memoPage.memoText.text = m.memoText;
		memoListPage.memoListPagePannel.SetActive (false);
		memoPage.memoPagePannel.SetActive (true);
	}

	// this method is called when the student returns from taking the quiz and displays their scores
	void DisplayScoresPage(){
		// first turn on the page itself
		studentsScoresPage.studentScoresPagePannel.SetActive (true);
		// add student items for each question
		for (int i = 0; i < StaticStuff.studentScores.Count; i++) {
			GameObject scoreItem = GameObject.Instantiate (studentsScoresPage.scoreItem) as GameObject;
			scoreItem.GetComponent<RectTransform>().SetParent(studentsScoresPage.studentsScoreScrollPannel.transform,false);
			scoreItem.GetComponent<ScoreItem> ().SetTextElements(StaticStuff.studentScores[i]);
		}
	}

	// coroutine method for checking student credintilas against database
	IEnumerator CheckStudentLogin(){
		List<Student> registeredStudents = new List<Student> ();
		WWWForm form = new WWWForm ();
		form.AddField ("action", "get_students");
		form.AddField ("table", StaticStuff.loggedInTeacher.teacherName + "_students");
		WWW w = new WWW (StaticStuff.loggedInSchool.school_php_url, form);
		yield return w;
		string[] received_data = Regex.Split (w.text, "</next>");
		int studentsSize = (received_data.Length - 1) / 3;
		for (var i = 0; i < studentsSize; i++) {
			Student s = new Student ();
			s.studentName = received_data [3 * i];
			s.studentPassword = received_data [3 * i + 1];
			s.studentEmail = received_data [3 * i + 2];
			registeredStudents.Add (s);
		}
		foreach (Student s in registeredStudents) {
			if (s.studentName == studentLoginPage.studentNameInput.text && s.studentPassword == studentLoginPage.studentPasswordInput.text) {
				OnStudentLogin (s);
				yield break;
			}
		}
		studentLoginPage.studentNameInput.text = "";
		studentLoginPage.studentPasswordInput.text = "";
		studentLoginPage.studentLoginButtonText.text = "Incorect Login";
	}



	// coroutine for getting memos
	IEnumerator GetMemos(){
		WWWForm form = new WWWForm ();
		form.AddField ("action", "get_memos");
		form.AddField ("table_name",  StaticStuff.loggedInTeacher.teacherName + "_memos");
		WWW w = new WWW (StaticStuff.loggedInSchool.school_php_url, form);
		yield return w;
		studentCalanderPage.memos.Clear ();
		string[] received_data = Regex.Split (w.text, "</next>");
		int defaultLessonsItemsSize = (received_data.Length - 1) / 4;
		for (var i = 0; i < defaultLessonsItemsSize; i++) {
			Memo m = new Memo ();
			m.memoDate = received_data [4 * i];
			m.memoTitle = received_data [4 * i + 1];
			m.memoStudent = received_data [4 * i + 2];
			m.memoText = received_data [4 * i + 3];
			if (m.memoStudent == "All Students" || m.memoStudent == StaticStuff.loggedInStudent.studentName) {
				studentCalanderPage.memos.Add (m);
			}
		}
		DrawMemoDays ();
		studentCalanderPage.StudentCalanderPannel.SetActive (true);
	}


}








