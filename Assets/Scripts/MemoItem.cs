using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MemoItem : MonoBehaviour {

	//event for when the button is clicked
	public delegate void ClickedMemoItemButton(Memo m);
	public static event ClickedMemoItemButton OnClickedMemoItemButton;

	public Memo thisMemo;


	public void SetMemoItem(Memo m){
		thisMemo = m;
		GetComponentInChildren<Text> ().text = m.memoTitle;
	}

	public void Button_ClickedMemoItem(){
		OnClickedMemoItemButton (thisMemo);
	}

}
