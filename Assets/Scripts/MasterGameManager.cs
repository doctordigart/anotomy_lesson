using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Linq;
using System;

public class MasterGameManager : MonoBehaviour {

	[Header("Current State of Menu")]
	public CurrentSate currentState;
	public enum CurrentSate {login,lessonSelect,loadingLesson};

	[Header("Data on Server")]
	public ServerData serverData;

	[Header("UI Elements")]
	public UIStuff uiStuff;

	[Header("Scene Monitor")]
	public SceneMonitor sceneMonitor;

	void OnEnable(){
		AnimationEventCaller.OnAnimationHasFinnished += OnAnimationFinished;

	}

	void OnDisable(){
		AnimationEventCaller.OnAnimationHasFinnished -= OnAnimationFinished;
	}





	#region StartPannelButtonFunctions
	// School Login Button
	public void Button_SchoolLogin(){
		//get list of registered schools
		StartCoroutine(GetSchools());
	}
	//Teacher login button
	public void Button_Teacher_Login ()
	{
		StartCoroutine (GetTeachers ());

	}
	// select level buttons
	public void Button_SelectLesson(bool isLeft){
		if (isLeft) {
			uiStuff.lessonSelect.currentLessonIdex = uiStuff.lessonSelect.currentLessonIdex - 1;
			if (uiStuff.lessonSelect.currentLessonIdex == -1) {
				uiStuff.lessonSelect.currentLessonIdex = uiStuff.lessonSelect.addedLessons.Count - 1;
			}
		} 
		else {
			uiStuff.lessonSelect.currentLessonIdex = uiStuff.lessonSelect.currentLessonIdex + 1;
			if (uiStuff.lessonSelect.currentLessonIdex == uiStuff.lessonSelect.addedLessons.Count) {
				uiStuff.lessonSelect.currentLessonIdex = 0;
			}
		}
		SetLessonSelectUI ();
	}
	// start lesson button
	public void Button_StartLesson ()
	{
		StaticStuff.currentLesson = uiStuff.lessonSelect.addedLessons [uiStuff.lessonSelect.currentLessonIdex];
		StartCoroutine (GetLessonItems ());
	}


	#endregion






	public void OnAnimationFinished(){
		switch (currentState) {
		case CurrentSate.login:
			currentState = CurrentSate.lessonSelect;
			uiStuff.lessonSelect.pannel.SetActive (true);
			StartCoroutine (GetLessons ());
			break;
		case CurrentSate.lessonSelect:
			currentState = CurrentSate.loadingLesson;
			uiStuff.loadingPannel.SetActive (true);
			StartCoroutine (LoadBundleScene ());
			break;
		}
	}









	IEnumerator GetSchools (){
		WWWForm form = new WWWForm ();
		form.AddField ("action", "get_schools");
		WWW w = new WWW (serverData.ourPhpURL, form);
		yield return w;
		List<School> registeredSchools = new List<School>();
		string[] received_data = Regex.Split (w.text, "</next>");
		int schoolsSize = (received_data.Length - 1) / 3;
		for (var i = 0; i < schoolsSize; i++) {
			School s = new School ();
			s.school_name = received_data [3 * i];
			s.school_password = received_data [3 * i + 1];
			s.school_php_url = received_data [3 * i + 2];
			registeredSchools.Add (s);
		}
		CheckSchoolCredentials (registeredSchools);
	}



	void CheckSchoolCredentials(List<School> registeredSchools){
		foreach (School s in registeredSchools) {
			if (s.school_name == uiStuff.schoolLogin.loginNameInput.text && s.school_password == uiStuff.schoolLogin.loginPasswordInput.text) {
				uiStuff.schoolLogin.loginFeedback.text = "Login Successful";
				uiStuff.schoolLogin.loginNameInput.text = "";
				uiStuff.schoolLogin.loginPasswordInput.text = "";
				uiStuff.schoolLogin.pannel.SetActive (false);
				uiStuff.teacherLogin.pannel.SetActive (true);
				StaticStuff.loggedInSchool = s;
				return;
			}
		}
		uiStuff.schoolLogin.loginFeedback.text = "Incorrect Login (Case Sensitive)";
		uiStuff.schoolLogin.loginNameInput.text = "";
		uiStuff.schoolLogin.loginPasswordInput.text = "";
	}




	IEnumerator GetTeachers (){
		WWWForm form = new WWWForm ();
		form.AddField ("action", "get_teachers");
		WWW w = new WWW (StaticStuff.loggedInSchool.school_php_url, form);
		yield return w;
		List<Teacher> registeredTeachers = new List<Teacher>();
		string[] received_data = Regex.Split (w.text, "</next>");
		int classSize = (received_data.Length - 1) / 3;
		for (var i = 0; i < classSize; i++) {
			Teacher t = new Teacher ();
			t.teacherName = received_data [3 * i];
			t.teacherPassword = received_data [3 * i + 1];
			t.teacherEmail = received_data [3 * i + 2];
			registeredTeachers.Add (t);
		}
		CheckTeacherCredentials (registeredTeachers);
	}




	void CheckTeacherCredentials(List<Teacher> registeredTeachers){
		foreach (Teacher t in registeredTeachers) {
			if (t.teacherName == uiStuff.teacherLogin.loginNameInput.text && t.teacherPassword == uiStuff.teacherLogin.loginPasswordInput.text) {
				uiStuff.teacherLogin.loginFeedback.text = "Login Successful";
				uiStuff.teacherLogin.loginNameInput.text = "";
				uiStuff.teacherLogin.loginPasswordInput.text = "";
				uiStuff.teacherLogin.pannel.SetActive (false);
				StaticStuff.loggedInTeacher = t;
				sceneMonitor.dolly.GetComponent<Animator> ().SetTrigger ("PlayAnimation");
				return;
			}
		}
		uiStuff.teacherLogin.loginFeedback.text = "Incorrect Login (Case Sensitive)";
		uiStuff.teacherLogin.loginNameInput.text = "";
		uiStuff.teacherLogin.loginPasswordInput.text = "";
	}



	IEnumerator GetLessons (){
		WWWForm form = new WWWForm ();
		form.AddField ("action", "get_added_lessons");
		form.AddField("table_name", StaticStuff.loggedInTeacher.teacherName + "_lessons");
		WWW w = new WWW (StaticStuff.loggedInSchool.school_php_url, form);
		yield return w;
		string[] received_data = Regex.Split (w.text, "</next>");
		uiStuff.lessonSelect.addedLessons = new List<Lesson>();
		int classSize = (received_data.Length - 1) / 4;
		for (var i = 0; i < classSize; i++) {
			Lesson l = new Lesson ();
			l.lesson_name = received_data [4 * i];
			l.lesson_items = Int32.Parse(received_data [4 * i + 1]);
			l.lesson_intro = received_data [4 * i +2];
			l.lesson_conclusion = received_data [4 * i +3];
			uiStuff.lessonSelect.addedLessons.Add (l);
		}
		uiStuff.lessonSelect.lessonSelectUI.SetActive (true);
		SetLessonSelectUI ();
	}


	void SetLessonSelectUI (){
		uiStuff.lessonSelect.currentLessonText.text = uiStuff.lessonSelect.addedLessons [uiStuff.lessonSelect.currentLessonIdex].lesson_name;
	}


	IEnumerator GetLessonItems(){
		WWWForm form = new WWWForm ();
		form.AddField ("action", "get_lesson_items_to_edit");
		form.AddField ("table_name", StaticStuff.loggedInTeacher.teacherName + "_" + StaticStuff.currentLesson.lesson_name + "_lesson_items");
		WWW w = new WWW (StaticStuff.loggedInSchool.school_php_url, form);
		yield return w;
		string[] received_data = Regex.Split (w.text, "</next>");
		StaticStuff.currentLessonItems = new List<LessonItem>();
		int classSize = (received_data.Length - 1) / 5;
		for (var i = 0; i < classSize; i++) {
			LessonItem l = new LessonItem ();
			l.item_name = received_data [5 * i];
			l.item_index = Int32.Parse(received_data [5 * i + 1]);
			l.item_lecture = received_data [5 * i +2];
			l.item_question = received_data [5 * i +3];
			l.item_answer = received_data [5 * i +4];
			StaticStuff.currentLessonItems.Add (l);
		}
		sceneMonitor.dolly.GetComponent<Animator> ().SetTrigger ("PlayAnimation");
		uiStuff.lessonSelect.pannel.SetActive (false);
	}


	IEnumerator LoadBundleScene(){
		AssetBundle bundle;
		while (!Caching.ready) {
			yield return null;
		}
		string bundleURL = serverData.assetBundlesURL + "/" + StaticStuff.currentLesson.lesson_name + "/" + BuildType.PC.ToString() + "/" + StaticStuff.currentLesson.lesson_name.ToLower();
		//string bundleURL = "File:///C://Users/Frank/Desktop/dinosaurs";
		var www = WWW.LoadFromCacheOrDownload(bundleURL, 1);
		StartCoroutine (TrackProgress (www));
		yield return www;
		bundle = www.assetBundle;
		SceneManager.LoadScene ("StudentLobby");
	}


	IEnumerator TrackProgress(WWW www){
		while (www.isDone == false) {
			Debug.Log (www.progress);
			uiStuff.loadingProgressBar.localScale = new Vector3 (www.progress, 1f, 1f);
			yield return new WaitForSeconds (.1f);
		}
	}


}

