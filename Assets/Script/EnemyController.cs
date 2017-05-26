using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public float hTravel;		//travelling horizontal distance
	public float vTravel;		//travelling vertical distance
	public float speed;			//movement speed (horizontal and vertical) of enemy
	public int hitDamage = 1;
	public GameObject enemyParticle;
	
	private float xOrig;		//x origin for current movement
	private float yOrig;		//y origin for current movement
	private float xTarget;		//x target for current movement
	private float yTarget;		//y target for current movement
	private float xMovFrac = 0.0f;		//fraction of the distance between the origin and the target (used for interpolation)
	private float yMovFrac = 0.0f;
	private float xPos;			//current x position for readability
	private float yPos;			//current y position for readability
	private float xDist;		//horizontal movement distance
	private float yDist;		//vertical movement distance

	void Start () {
		xOrig = transform.position.x;
		yOrig = transform.position.y;
		xTarget = xOrig + hTravel;
		yTarget = yOrig + vTravel;
		xDist = Mathf.Abs(xOrig - xTarget);
		yDist = Mathf.Abs(yOrig - yTarget);
	
	}
	

	void Update () {
		xPos = Mathf.Lerp(xOrig, xTarget, xMovFrac);
		yPos = Mathf.Lerp(yOrig, yTarget, yMovFrac);
		transform.position = new Vector3(xPos, yPos, transform.position.z);
        
        // .. and increate the movFrac interpolater
        xMovFrac += speed/xDist * Time.deltaTime;
		yMovFrac += speed/yDist * Time.deltaTime;
        
        // now check if the interpolator has reached 1.0
        // and swap maximum and minimum of both x and y so game object moves
        // in the opposite direction.
        if (xMovFrac > 1.0f){
            float temp = xTarget;
            xTarget = xOrig;
            xOrig = temp;
			xMovFrac = 0.0f;
			transform.Rotate(0,180, 0);
		}
		if (yMovFrac > 1.0f){
			float temp = yTarget;
			yTarget = yOrig;
			yOrig = temp;
            yMovFrac = 0.0f;

		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		//if enemy box collider contacts player, destroys player
		//removes player
		//when player is destroyed, loads scene again
		if (other.gameObject.tag == "Player") {
			other.SendMessage ("loseHealth", hitDamage);
			}
	}
}
