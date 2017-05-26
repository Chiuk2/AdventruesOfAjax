using UnityEngine;
using System.Collections;

public class Finish : MonoBehaviour {
	private ParticleSystem[] flagParticles;
	// Use this for initialization
	void Start () {
		flagParticles = GetComponentsInChildren<ParticleSystem>( );
	}
	
	void OnTriggerEnter(Collider other){
		if(other.gameObject.tag == "Player"){
			//logic for a gameover with a win condition may go here
			foreach(ParticleSystem particle in flagParticles){
				particle.Play();
			}
			other.SendMessage("setGameOver", "You Win!");
		}
	}
}