using UnityEngine;
using System.Collections;

public class BodyDisply : MonoBehaviour {

	public GameObject body;
	public Renderer[] renderers;
	public void TurnOnOrgans () {
		
		renderers = body.GetComponentsInChildren<Renderer>( );
		foreach (Renderer r in renderers) {
			r.enabled = false;
		}
		Debug.Log(renderers.Length);
	}
}
