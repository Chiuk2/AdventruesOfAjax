using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    private float inputDirection;               //X value of moveVector
    private float verticalVelocity;             //Y value of moveVector
	private bool isRunning = false;						//is player running
	private bool isFacingLeft = false;
	private string[] cheatCode = {"o", "p", "o", "p"};

    public float gravity = 25.0f;               //constantly subtracting y axis, unless on ground
    public float speed = 5.5f;                 	//speed of the player
	public float jumpForce = 12.0f;				//height of the jump
	public float jumpShortForce = 5.0f;			//shortest jump height
	private float currentheight = 0.0f;
	private float previousheight = 0.0f;
	private float travel = 0.0f;
	public int playerHealth = 3;
	public int hitDamage = 1;
	public int fallDamage = 3;
	private int cheatIndex = 0;

	//-------------------------  UI stuff
	//keeps count of carrots collected
	//if all carrots collected, player wins
	//new purpose of game: player defeats enemies to collect all carrots
	//cool right?
	private int carrotbro = 0;
	public Text countcarrot;		//text of carrots collected
	public Text playerwins;			//text when player wins
	public GameObject overlay;			//UI overlay with menu items
	public Image[] showlives;			//sprite of player lives
	public Sprite heart_empty;
	public Sprite heart_full;
	//-------------------------  UI stuff

	private CharacterController controller;     //collision for player
	private Renderer playerRender;				//The renderer to change color
	Vector3 impact = Vector3.zero;
    private Vector3 moveVector;                 //representation of 3d vectors for player
	private Vector3 lastMotion;					//maybe useful for future implementations
	private Animator anim;
	private AudioSource playerAudio;
	
	public AudioClip jumpClip;
	// public AudioClip collectClip;
	
	public GameObject enemyParticle;
	public GameObject carrotParticle;

	//private bool wallJump = false;				//limits one wall jump, resets when player lands
	private bool isFalling = false;
	private bool onWall = false;
	//------------------------------- new wall jump stuff
	private bool isWallSliding = false;
	private bool isWallJump = false;
	//--------------------------------
	private bool isJumping = false;
	private bool jumpIsCanceled = false;
	public bool invincible = false;
	public bool ableKnockback = true;
	// Use this for initialization
	void Start () {
	anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
		playerAudio = GetComponent<AudioSource>();
		playerHealth = 3;
		setscreencount (); //function call to set text for carrot/life count
		playerwins.text = "";
		playerRender = GetComponentInChildren<Renderer>();
		overlay.SetActive(false);
	}
		

    // Update is called once per frame
    void Update() {
        if (impact.magnitude > 0.2)
        {
            controller.Move(impact * Time.deltaTime);
        }
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
        moveVector = Vector3.zero;
		//Left Right Movement
		inputDirection = Input.GetAxis ("Horizontal") * speed;

		currentheight = transform.position.y;
		travel = currentheight - previousheight;
		if (travel < 0)
			isFalling = true;
		else
			isFalling = false;
		previousheight = currentheight;
		/* here it checks if player is on ground, and resets wall jump is true. It also 
		 * ensures player falls based on the variable gravity by minusing the verticalVelocity,
		 * which is the Y movement for the player. */

		if (isPlayerGrounded() == true)
        {
			isWallJump = false;
			onWall = false;
			gravity = 25.0f;
            verticalVelocity = 0;
	
			if (Input.GetKey (KeyCode.X)) {
				speed = 9.25f;
				isRunning = true;
			} else {
				speed = 5.5f;
				isRunning = false;
			}

			//if (wallJump == true)
			//	wallJump = false;
			if (Input.GetKeyDown(KeyCode.C))
            {
				//Debug.Log ("key being pressed");
                //make player jump
                //verticalVelocity = jumpForce;
				playerAudio.clip = jumpClip;
				playerAudio.Play();
				isJumping = true;
            }

        }

        else
        {
			if (onWall == true) {
				moveVector.x = lastMotion.x;
			}
			verticalVelocity -= gravity*Time.deltaTime;
        }
		if (onWall == false) {
			moveVector.x = inputDirection;
		}
	//----------------------------------------var_jump
	if(Input.GetKeyUp(KeyCode.C))
	{
		jumpIsCanceled = true;
	}
	if(isJumping){
	    verticalVelocity = jumpForce;
	    isJumping = false;
	}
	if(jumpIsCanceled && !isWallJump){
	    if(controller.velocity.y > jumpShortForce)
	    {
		verticalVelocity = jumpShortForce;
	    }
	    jumpIsCanceled = false;
	}
	//--------------------------------------var_jump
	
		moveVector.y = verticalVelocity; 
        controller.Move(moveVector * Time.deltaTime);
		lastMotion = moveVector;
	//------------------------ new wall jump stuff
		if (controller.collisionFlags != CollisionFlags.Sides && isPlayerGrounded () == false) {
			gravity = 25.0f;
			isWallSliding = false;
		}
		else{
			isWallSliding = true;
			isWallJump = true;
		}
	//-------------------------- new wall jump stuff
	if (inputDirection < 0){
		if (!isFacingLeft){
			transform.Rotate(0, 180, 0);
		}
		isFacingLeft = true;
	}
	else if (inputDirection > 0){
		if (isFacingLeft){
			transform.Rotate(0,180, 0);
		}
		isFacingLeft = false;
	}
	//cheat code to jump to the end of the level
	if (cheatIndex < cheatCode.Length && Input.GetKeyDown(cheatCode[cheatIndex])) {
		cheatIndex++;
	}
	else if (cheatIndex == cheatCode.Length) {
		transform.position = new Vector3(174.0f, 25.0f, 0.3392f);
		cheatIndex = 0;
	}
	
	anim.SetFloat("hSpeed", Mathf.Abs(inputDirection));
	anim.SetFloat("vSpeed", verticalVelocity);
	anim.SetBool("isGrounded", isPlayerGrounded());
	anim.SetBool("isSprinting", isRunning);
	anim.SetBool("isOnWall", isWallSliding);

    }

	/* here we use the library function OnControllerColliderHit, which is called everytime player
	 * collides on a surface, in order to signal if the player is touching wall on one of its sides.
	 * This will also check to see if player presses up arrow to wall jump and if it is its first time
	 * doing so. bool variable wallJump limits the player to one wall jump */

	private void OnControllerColliderHit(ControllerColliderHit hit){
		if (controller.collisionFlags == CollisionFlags.Sides && isPlayerGrounded () == false) {
			onWall = false;
			switch (hit.gameObject.tag) {
                case "Enemy":
                    Debug.Log("collide with player");
                    loseHealth(hitDamage);
                    break;
				case "Collectible":
					Debug.Log ("collected by player");
					Instantiate (carrotParticle, hit.gameObject.transform.position, Quaternion.identity);
					Destroy (hit.gameObject);
					carrotbro += 1;
					Debug.Log ("carrotbro bottom");
					setscreencount (); //function call to set text for carrot/life count
					break;
                default:
				break;
			}
			if (Input.GetKeyDown (KeyCode.C)) {
				onWall = true;
				moveVector = hit.normal * speed; 
				verticalVelocity = jumpForce;
				playerAudio.clip = jumpClip;
				playerAudio.Play();
				isJumping = true;
			}
			if (isFalling == true) {
				//needs something to stop veritcal velocity in motion
				gravity = 3.0f;

			} else {
				gravity = 25.0f;
			}
		}  else if (controller.collisionFlags == CollisionFlags.Above && isPlayerGrounded () == false /*|| look at this isPlayerGrounded () == true*/) {
			/* Kyle when something is in the air but something is still off when flying enemy lands on grounded character from above no damage. 
			 * oh and i forgot cs 135 can we do something like ((above && false) || (above && true)) 
			 * so the code can be a little more condensed and less redundant? 
			 * ^ these are my ?s about it
			 * But the problems:
			 * 1 standing still under enemy, no damage to player
			 * 2 standing still with horizontal moving enemy, no damage to player (i tried changing collisions boxes but idk)
			 * ohh  if u play level holding down sprint, ajax's is practically a superninja (especially in air and wall jump), which is cool lol
			*/

			switch (hit.gameObject.tag) {
			case "Enemy":
				Debug.Log("player under bee");
				loseHealth(hitDamage);
				//note: maybe have knockback have effect when falling in abyss
				break;

			case "Collectible":
				Debug.Log ("player under carrot");
				Instantiate (carrotParticle, hit.gameObject.transform.position, Quaternion.identity);
				Destroy (hit.gameObject);
				carrotbro += 1;
				Debug.Log (carrotbro);
				//playerAudio.clip = collectClip;
				//playerAudio.Play();
				setscreencount (); //function call to set text for carrot/life count
				break;

			default:
				break;
			}
		}  else if (controller.collisionFlags == CollisionFlags.Sides && isPlayerGrounded () == true) {
			switch (hit.gameObject.tag) {
                case "Enemy":
                    Debug.Log("collide with player too");
                    loseHealth(hitDamage);
                    //note: maybe have knockback have effect when falling in abyss
                    break;

                case "Collectible":
					Debug.Log ("collected by player");
					Instantiate (carrotParticle, hit.gameObject.transform.position, Quaternion.identity);
					Destroy (hit.gameObject);
					carrotbro += 1;
					Debug.Log (carrotbro);
					//playerAudio.clip = collectClip;
					//playerAudio.Play();
					setscreencount (); //function call to set text for carrot/life count
				break;

			default:
				break;
			}
		}
		if((controller.collisionFlags & CollisionFlags.Below)!=0)
		{
			switch (hit.gameObject.tag) {
			case "Enemy":
				verticalVelocity = jumpForce / 2;
				Instantiate (enemyParticle, hit.gameObject.transform.position, Quaternion.identity);
				Destroy (hit.gameObject);
				break;
			case "Collectible":
				Destroy (hit.gameObject);
				Debug.Log ("carrot top hehe");
				Instantiate (carrotParticle, hit.gameObject.transform.position, Quaternion.identity);
				carrotbro += 1;
				Debug.Log (carrotbro);
				setscreencount (); //function call to set text for carrot/life count
				break;
			case "Abyss":
				verticalVelocity = 2*jumpForce;
				loseHealth (fallDamage);
				break;
			default:
				break;
			}
		}
	}

	/* Checks to see if player is on the ground or not */

	bool isPlayerGrounded(){
		Vector3 leftRayStart;
		Vector3 rightRayStart;
		RaycastHit hitInfo;

		leftRayStart = controller.bounds.center;
		rightRayStart = controller.bounds.center;

		leftRayStart.x -= controller.bounds.extents.x;
		rightRayStart.x += controller.bounds.extents.x;

		Debug.DrawRay(leftRayStart, Vector3.down, Color.cyan);
		Debug.DrawRay(rightRayStart, Vector3.down, Color.green);

		if (Physics.Raycast (rightRayStart, Vector3.down, out hitInfo, (controller.height / 2.0f) + 0.1f)) {
			if (hitInfo.collider.gameObject.tag != "Player")
				return true;
		}
		if (Physics.Raycast (leftRayStart, Vector3.down, out hitInfo, (controller.height / 2.0f) + 0.1f)) {
			if (hitInfo.collider.gameObject.tag != "Player")
				return true;
		}
		return false;
	}

	public void loseHealth(int damage)
	{
		if (!invincible) {	
			playerHealth -= damage;
			Debug.Log (playerHealth);
			setscreencount (); //function call to set text for carrot/life count
            addImpact();
            checkGameOver ();
			invincible = true;
			Invoke ("resetInvulnerability", 1);
			//turns the player red, enable color tint in the shader tab to use
			playerRender.material.SetColor("_Color", Color.red);	
		}
	}

	public void checkGameOver()
	{
		if(playerHealth <= 0)
		{
			//SceneManager.LoadScene ("Trial");
			setGameOver("Game Over");
		}
	}

	public void resetInvulnerability()
	{
		invincible = false;
		//turns the player normal,enable color tint in the shader tab to use
		playerRender.material.SetColor("_Color", Color.white);	
	}

	//displays various counts on screen
	//when player collects all carrots, displays "win" text
	public void setscreencount()
	{
		countcarrot.text = carrotbro.ToString ();
		if (playerHealth < 3){
			showlives[0].sprite = heart_empty;
			if (playerHealth < 2){
				showlives[1].sprite = heart_empty;
				if(playerHealth < 1){
					showlives[2].sprite = heart_empty;
				}
			}
		}
		if (carrotbro == 40) {
			playerwins.text = "You're a carrot master!!!!";
			//funny thing if player collects too fast it'll overcount
		}
	}

    public void addImpact()
    {
        impact = new Vector3(-3, 3, 0);
        impact = transform.TransformDirection(impact);
        impact *= speed;
        impact.y -= gravity * Time.deltaTime;

	}
	
	public void setGameOver(string gameOverText){
		Instantiate (enemyParticle, this.transform.position, Quaternion.identity);
		playerwins.text = gameOverText;
		overlay.SetActive(true);
		speed = 0;
		playerRender.enabled = false;
		controller.enabled = false;
	}
}
