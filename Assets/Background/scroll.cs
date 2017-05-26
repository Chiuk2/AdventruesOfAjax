using UnityEngine;
using System.Collections;

public class scroll : MonoBehaviour {
	//endless background scroll implementation
	public float speed = 0.5f;	//take care of background speed

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector2 offset = new Vector2(Time.time * speed, 0);					//move background img horizontally

		GetComponent<Renderer> ().material.mainTextureOffset = offset;		//background img keeps moving    

	}
}
