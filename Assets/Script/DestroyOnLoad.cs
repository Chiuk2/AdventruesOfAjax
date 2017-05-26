using UnityEngine;
using System.Collections;

public class DestroyOnLoad : MonoBehaviour {
public float timeToDestruction = 1.0f;
	// Use this for initialization
	void Start () {
		Destroy(this.gameObject, timeToDestruction);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
