using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class Tour_Quiz : MonoBehaviour {

	#region Variables

	// overall state of the lesson
	[Header ("State of Tour/Quiz")]
	public LessonState lessonState;

	public enum LessonState
	{intro,moving,lecture,waitingForRespose,endOfTour,endOfQuiz};

	// here is the class for variables controlling the overall quiz
	[Header ("Variables for managing tour/quiz")]
	public TourGuide tourGuide;

	// list of the items of the tour
	[Header ("Items in Tour and quiz")]
	public List<TourItem> tourItems;

	// here we have variables for camera movement
	[Header ("Camera and user input")]
	public UserCameraMovement cameraMovement;
	// variables for the selector
	public UISelector uiSelector;

	#endregion


	#region MonoDevelopmentOverideCalls

	void OnEnable ()
	{
		// subscribe to the animation event caller
		AnimationEventCaller.OnAnimationHasFinnished += AnimationFinnished;
	}

	void OnDisable ()
	{
		// unsubscribe to the animation event caller
		AnimationEventCaller.OnAnimationHasFinnished -= AnimationFinnished;
	}


	void Start ()
	{
		// load our info from the server
		LoadTour ();
		// if we are in tour mode start the tour
		if (StaticStuff.tourMode == TourMode.tour) {
			StartTour ();
		}
		// if we are in quiz mode start quiz
		else if (StaticStuff.tourMode == TourMode.quiz) {
			StartQuiz ();
		}
	}


	void Update ()
	{
		// move camera every fram user input;
		ControlCamera ();
		// check if the user is looking at a selectable object;
		CheckForLook ();
		//check to see if the user has entered a click magnet pull ect..
		CheckForClick ();
	}

	#endregion


	#region UserInput/CameraInput

	void ControlCamera ()
	{
		#region PC_Web
		// this bit of code is for moving the camera in the pc build for other build targets simply comment this out
		Screen.lockCursor = cameraMovement.lockCursor;
		var targetOrientation = Quaternion.Euler (cameraMovement.targetDirection);
		var mouseDelta = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
		mouseDelta = Vector2.Scale (mouseDelta, new Vector2 (cameraMovement.sensitivity.x * cameraMovement.smoothing.x, cameraMovement.sensitivity.y * cameraMovement.smoothing.y));
		cameraMovement._smoothMouse.x = Mathf.Lerp (cameraMovement._smoothMouse.x, mouseDelta.x, 1f / cameraMovement.smoothing.x);
		cameraMovement._smoothMouse.y = Mathf.Lerp (cameraMovement._smoothMouse.y, mouseDelta.y, 1f / cameraMovement.smoothing.y);
		cameraMovement._mouseAbsolute += cameraMovement._smoothMouse;
		if (cameraMovement.clampInDegrees.x < 360)
			cameraMovement._mouseAbsolute.x = Mathf.Clamp (cameraMovement._mouseAbsolute.x, -cameraMovement.clampInDegrees.x * 0.5f, cameraMovement.clampInDegrees.x * 0.5f);
		var xRotation = Quaternion.AngleAxis (-cameraMovement._mouseAbsolute.y, targetOrientation * Vector3.right);
		cameraMovement.cameraTransform.localRotation = xRotation;
		if (cameraMovement.clampInDegrees.y < 360)
			cameraMovement._mouseAbsolute.y = Mathf.Clamp (cameraMovement._mouseAbsolute.y, -cameraMovement.clampInDegrees.y * 0.5f, cameraMovement.clampInDegrees.y * 0.5f);
		cameraMovement.cameraTransform.localRotation *= targetOrientation;
		var yRotation = Quaternion.AngleAxis (cameraMovement._mouseAbsolute.x, cameraMovement.cameraTransform.InverseTransformDirection (Vector3.up));
		cameraMovement.cameraTransform.localRotation *= yRotation;
		#endregion
	}

	void CheckForLook ()
	{
		RaycastHit hit;
		if (Physics.Raycast (uiSelector.lookObject.position, uiSelector.lookObject.forward, out hit, 100f, uiSelector.selectableObjectsLayer)) {
			uiSelector.crossHair.material.color = Color.green;
			uiSelector.lookedAtObject = hit.collider.gameObject;

			if (uiSelector.lookedAtObject != null) {
				uiSelector.lookedAtObject.gameObject.SendMessage ("LookAtBegan", tourGuide.currentTourItem, SendMessageOptions.DontRequireReceiver);
			}

		} else {
			uiSelector.crossHair.material.color = Color.red;

			if (uiSelector.lookedAtObject != null) {
				uiSelector.lookedAtObject.gameObject.SendMessage ("LookAtEnd", tourGuide.currentTourItem, SendMessageOptions.DontRequireReceiver);
			}
			uiSelector.lookedAtObject = null;
		}
	}

	void CheckForClick ()
	{

		if (Input.GetMouseButtonDown (0)) {
			UserClick ();
		}


	}



	void UserClick(){
		// if user is looking at a selectable object when user clicks call the method for selecting objects
		if (uiSelector.lookedAtObject) {
			DoSelectedButton (uiSelector.lookedAtObject.name);
		}
	}






	// does shit based on what the gameobjects are named in the scene
	void DoSelectedButton(string buttonName){
		// a switch that calls the appropriate method based on the clicked buttons name
		switch (buttonName){
		case "LetsStart":
			LetsStart ();
			break;
		case "PlayAgain":
			PlayAgain();
			break;
		case "PlayNextItem":
			PlayNextItem ();
			break;
		case "True":
			RecordAnswer ("True");
			break;
		case "False":
			RecordAnswer ("False");
			break;
		case "A":
			RecordAnswer ("A");
			break;
		case "B":
			RecordAnswer ("B");
			break;
		case "C":
			RecordAnswer ("C");
			break;
		case "D":
			RecordAnswer ("D");
			break;
		}
	}


	#endregion



	// starts tour/quiz from intro
	void LetsStart(){
		foreach (GameObject go in tourGuide.introActiveObjects) {
			go.SendMessage ("DoThing", -1);
		}
		// sets the lessonstate to be moving
		lessonState = LessonState.moving;
	}

	void LoadTour(){
		// load lesson stuff retrieved from server into tour guide
		tourGuide.introductionTour = StaticStuff.currentLesson.lesson_intro;
		tourGuide.conclusion = StaticStuff.currentLesson.lesson_conclusion;
		// load the tour guide items with info fetched from server
		for (int i = 0; i < StaticStuff.currentLessonItems.Count; i++) {
			tourItems [i].itemName = StaticStuff.currentLessonItems [i].item_name;
			tourItems [i].lecture = StaticStuff.currentLessonItems [i].item_lecture;
			tourItems [i].question = StaticStuff.currentLessonItems [i].item_question;
			tourItems [i].answer = StaticStuff.currentLessonItems [i].item_answer;
		}
	}

	void StartTour(){
		// say intoduction speach
		transform.SendMessage("Say", tourGuide.introductionTour);
	}

	void StartQuiz(){
		// say intoduction speach for quiz
		transform.SendMessage("Say", "Lets begin our quiz");
	}

	// this method turns on a items in tour mode
	void PlayTourItem(){
		// set the state to be at lecture
		lessonState = LessonState.lecture;
		// begin speaking lecture
		transform.SendMessage("Say", tourItems[tourGuide.currentTourItem].lecture);
		// play animation for active objects of this item
		foreach (GameObject go in tourItems[tourGuide.currentTourItem].activeObjects) {
			go.SendMessage ("DoThing", tourGuide.currentTourItem);
		}
	}

	void PlayQuizItem(){
		// set the state to be at lecture
		lessonState = LessonState.lecture;
		transform.SendMessage("Say", tourItems[tourGuide.currentTourItem].question);
		// play animation for active objects of this item
		foreach (GameObject go in tourItems[tourGuide.currentTourItem].activeObjects) {
			go.SendMessage ("DoThing", tourGuide.currentTourItem);
		}
	}

	void OpenTourUI(){
		// set the state to be waiting for the player to respond
		lessonState = LessonState.waitingForRespose;
		// set the UI to be at the position and rotation as our itm UI
		tourGuide.uiObject.transform.position = tourItems[tourGuide.currentTourItem].uiTransform.position;
		tourGuide.uiObject.transform.rotation = tourItems [tourGuide.currentTourItem].uiTransform.rotation;
		// turn on the UI object so its usable
		tourGuide.uiObject.SetActive(true);
		// turn on UI elements according to tourmde and question 
		if (StaticStuff.tourMode == TourMode.tour) {
			tourGuide.uiObject.transform.Find ("TourUI").gameObject.SetActive (true);
		}
		if (StaticStuff.tourMode == TourMode.quiz) {
			// start taking time for answer as soon as UI for answers is available for student to answer
			StartCoroutine(TimeAnswer());
			if (tourItems [tourGuide.currentTourItem].answer == "True" || tourItems [tourGuide.currentTourItem].answer == "False") {
				tourGuide.uiObject.transform.Find ("True_False").gameObject.SetActive (true);
				tourGuide.uiObject.transform.Find ("MultipleChoice").gameObject.SetActive (false);
			} 
			else {
				tourGuide.uiObject.transform.Find ("MultipleChoice").gameObject.SetActive (true);
				tourGuide.uiObject.transform.Find ("True_False").gameObject.SetActive (false);
			}
		}
	}


	// this method plays the spech or question again for the user
	void PlayAgain(){
		// stop animation for active objects of this item
		foreach (GameObject go in tourItems[tourGuide.currentTourItem].activeObjects) {
			go.SendMessage ("StopThing", tourGuide.currentTourItem);
		}
		// if we are in tour mode
		if (StaticStuff.tourMode == TourMode.tour) {
			// turn the ui off again
			tourGuide.uiObject.SetActive(false);
			// play the tour item again
			PlayTourItem();
		}
		if (StaticStuff.tourMode == TourMode.quiz) {
			// play the quiz question item again
			PlayQuizItem();
		}
	}


	void PlayertransAnimation(){
		foreach (GameObject go in tourItems[tourGuide.currentTourItem].activeObjects) {
			go.SendMessage ("StopThing", tourGuide.currentTourItem);
		}
		foreach (GameObject go in tourItems[tourGuide.currentTourItem].activeTransitionObjects) {
			go.SendMessage ("DoThingTran", tourGuide.currentTourItem);
		}
	}


	//coroutine that times students for their answers
	IEnumerator TimeAnswer(){
		// while waiting for response add deltatime to timer variable every frame
		while (lessonState != LessonState.moving) {
			tourItems [tourGuide.currentTourItem].answerTime = tourItems [tourGuide.currentTourItem].answerTime + Time.deltaTime;
			yield return new WaitForEndOfFrame ();
		}
	}

	//record the students answer for the quiz question
	void RecordAnswer(string studentAnswer){
		tourItems [tourGuide.currentTourItem].answerGiven = studentAnswer;
		if (studentAnswer == tourItems [tourGuide.currentTourItem].answer) {
			tourItems [tourGuide.currentTourItem].isCorrect = true;
		} else {
			tourItems [tourGuide.currentTourItem].isCorrect = false;
		}
		// store the answer in the satic variables
		StaticStuff.studentScores.Add(tourItems [tourGuide.currentTourItem]);
		// turn off interface
		tourGuide.uiObject.SetActive(false);
		// if the speaker is still speaking stop
		transform.SendMessage("StopSpeech");
		// stop animation for active objects of this item
		foreach (GameObject go in tourItems[tourGuide.currentTourItem].activeObjects) {
			go.SendMessage ("StopThing", tourGuide.currentTourItem);
		}
		// move on to the next question
		PlayNextItem();
	}


	// this method moves from one item to the next
	void PlayNextItem(){
		// check if we are at our tour/quiz end
		if (tourGuide.currentTourItem < tourItems.Count - 1) {
			// set the state to moving 
			lessonState = LessonState.moving;
			// play animation to next item
			PlayertransAnimation ();
			// index our item in list
			tourGuide.currentTourItem ++;
			// turn the ui off again
			tourGuide.uiObject.SetActive(false);
		} 
		else {
			if (StaticStuff.tourMode == TourMode.tour) {
				// set our lesson state to be the end of the tour
				lessonState = LessonState.endOfTour;
				// turn the ui off again
				tourGuide.uiObject.SetActive(false);
				// lets say the conclusion here
				transform.SendMessage("Say", tourGuide.conclusion);

			} 
			else if(StaticStuff.tourMode == TourMode.quiz){
				StartCoroutine (AddStudentScores ());
				//StartCoroutine (ScoreStudent ());
			}
		}
	}
		

	// called when introduction is finnished
	void IntroFinished(){
		// turn on lets start button so player can use it
		tourGuide.letsStartButton.SetActive (true);
	}


	// gets called when the player transform is finished animating
	void AnimationFinnished(){
		// determin if we are on the tour or quiz and play the appropriate tour item
		if (StaticStuff.tourMode == TourMode.tour) {
			// play the lecture
			PlayTourItem ();
		}
		// if we are in quiz mode play a quiz item
		else if (StaticStuff.tourMode == TourMode.quiz){
			PlayQuizItem ();
		}
	}

	// gets called when the speaker is done talking
	void DoneSpeaking(){
		switch (lessonState) {
		case LessonState.intro:
			// call the intro is finished method if speaker is caying intro
			IntroFinished ();
			//DisableGlow ();
			break;
		case LessonState.lecture:
			// open the tour UI if speaker is lectureing on an item
			OpenTourUI ();
			//DisableGlow ();
			break;
		case LessonState.endOfTour:
			// if were at the point of the tour end
			SceneManager.LoadSceneAsync ("StudentLobby");
			break;
		}

	}
		

	IEnumerator ScoreStudent(){
		WWWForm form = new WWWForm ();
		form.AddField ("action", "add_lesson_scores_table");
		form.AddField ("table_name", StaticStuff.loggedInTeacher.teacherName + "_" + StaticStuff.currentLesson.lesson_name + "_Grades");
		WWW w = new WWW (StaticStuff.loggedInSchool.school_php_url, form);
		yield return w;
		StartCoroutine (AddStudentScores ());
	}


	IEnumerator AddStudentScores(){		
		string student_scores_string  = "";
		for (int i = 0; i < tourItems.Count; i++){
			student_scores_string  = student_scores_string  + "('" + StaticStuff.loggedInStudent.studentName
				+ "', '" + tourItems[i].itemName
				+ "', '" + tourItems[i].answerTime.ToString() 
				+ "', '" + tourItems[i].isCorrect.ToString() + "')" + ",";
		}
		student_scores_string = student_scores_string.Substring(0, student_scores_string.Length - 1);
		WWWForm form = new WWWForm ();
		form.AddField ("action", "add_sudent_scores");
		form.AddField ("table_name", StaticStuff.loggedInTeacher.teacherName + "_" + StaticStuff.currentLesson.lesson_name + "_lesson_scores");
		form.AddField ("student_scores_string", student_scores_string);
		WWW w = new WWW (StaticStuff.loggedInSchool.school_php_url, form);
		yield return w;
		SendMail ();
	}

	void SendMail (){
		StartCoroutine (SendPHPMail ());
//		MailMessage mail = new MailMessage();
//
//		mail.From = new MailAddress("francisreedstapleton@yahoo.com");
//		mail.To.Add(StaticStuff.loggedInStudent.studentEmail);
//		mail.Subject = StaticStuff.loggedInStudent.studentName + "'s " + StaticStuff.currentLesson.lesson_name + " Scores";
//		string messageBody = "";
//		for (int i = 0; i < tourItems.Count; i++) {
//			messageBody = messageBody + tourItems [i].itemName + ": " + tourItems [i].isCorrect.ToString () + ": " + tourItems [i].answerTime.ToString () + "\n";
//		}
//		mail.Body = messageBody;
//
//		SmtpClient smtpServer = new SmtpClient("smtp.mail.yahoo.com");
//		smtpServer.Port = 587;
//		smtpServer.Credentials = new System.Net.NetworkCredential("francisreedstapleton@yahoo.com", "KittyCat22") as ICredentialsByHost;
//		smtpServer.EnableSsl = true;
//		ServicePointManager.ServerCertificateValidationCallback = 
//			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
//		{ return true; };
//		smtpServer.Send(mail);
//		Debug.Log("success");
//		SceneManager.LoadSceneAsync ("StudentLobby");
	}


	// this method sends a request to the schools server PHP file to send emails with the students grades
	IEnumerator SendPHPMail(){
		WWWForm form = new WWWForm ();
		form.AddField ("action", "send_mail");
		form.AddField ("headers", "From: " + StaticStuff.loggedInSchool.school_name);
		form.AddField ("tos", StaticStuff.loggedInStudent.studentEmail + "," +  StaticStuff.loggedInTeacher.teacherEmail);
		form.AddField("email_subject",StaticStuff.currentLesson.lesson_name + "_" + StaticStuff.loggedInStudent.studentName + "_Scores");
		// form the body of the email with scores
		string messageBody = "";
		for (int i = 0; i < tourItems.Count; i++) {
			messageBody = messageBody + tourItems [i].itemName + ": " + tourItems [i].isCorrect.ToString () + ": " + tourItems [i].answerTime.ToString () + "\n";
		}
		form.AddField ("email", messageBody);
		// send the request up to the sever
		WWW w = new WWW(StaticStuff.loggedInSchool.school_php_url,form);
		yield return w;
		// now load the lobby scene again
		SceneManager.LoadSceneAsync ("StudentLobby");
	}

}

