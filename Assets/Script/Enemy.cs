using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;		//to reload scene when player dies

public class Enemy : MonoBehaviour {

	private Vector3 init_position;	//initial position
	public float horiz = 4;			//enemy horizontal distance
	public float vert = 1;			//enemy vertical distance
	Transform movement;				//manipulate position
	public int hitDamage = 1;

	public GameObject enemyParticle;

	//turning variables
	public float turn;
	public float beg;
	public float end;
	public float secend;
	private bool turnalready = true;	

	// Use this for initialization
	void Start () {

		movement = GetComponent<Transform>();	//retreive object position

		init_position = movement.position;		//initial position is set

		//rotate when beginning and end position
		beg = init_position.x;					//to be compared
		end = init_position.x;					//to be compared

	}

	// Update is called once per frame
	void Update() {

		//change initial position using mathf.pingpong()
		//mathf.pingpong() moves object's x-axis and y-axis back and forth hence ping pong
		turn = init_position.x + horiz * Mathf.PingPong (Time.time, horiz);

		//finds max x value
		if (turn > end){
			secend = end;	//second to last result
			end = turn; 	//update end result
		}

		//float to int
		int intbeg = (int)beg;
		int intturn = (int)turn;
		int intend = (int)end;
		int intsecend = (int)end;

		//if turn position is at end position, enemy rotates
		if (intturn < intsecend) {
			/*
			Debug.Log("turn val: " +intturn);
			Debug.Log("intend val: " +intend);
			Debug.Log("intsecend val: " +intsecend);
			*/
			if (intturn == intend-1) {
				if (turnalready == true) {
					transform.Rotate (0, 180, 0);
					Instantiate (enemyParticle, gameObject.transform.position, Quaternion.identity);
					turnalready = false;
				}
			} 
		}
		//if turn position is at beginning pos, enemy rotates
		if (intturn > intbeg) {
			/*
			Debug.Log("turn val: " +intturn);
			Debug.Log("intbeg val: " +intbeg);
			*/
			if (intturn == intbeg+1) {
				if (turnalready == false) {
					transform.Rotate (0, 180, 0);
					Instantiate (enemyParticle, gameObject.transform.position, Quaternion.identity);
					turnalready = true;
				}
			} 
		}
		//it's not perfect but its the best i could do in a couple hours

		movement.position = new Vector3(turn, init_position.y + vert*Mathf.PingPong(Time.time, vert), init_position.z);

	}

	//function when enemy object collides with player object
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