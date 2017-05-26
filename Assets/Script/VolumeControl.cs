using UnityEngine;
using System.Collections;

public class VolumeControl : MonoBehaviour {

	public void setVolume(float volumeTarget){
		AudioListener.volume = volumeTarget;
	}

}
