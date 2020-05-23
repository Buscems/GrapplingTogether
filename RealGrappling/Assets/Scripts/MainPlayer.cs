using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class MainPlayer : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player player1;
    public Player player2;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    [Header("Movement")]

    public float speed;

    public Vector3 velocity;
    float velocityY;

    public Rigidbody rb;

    [Header("Gravity Variables")]
    public float gravityUp;
    public float gravityDown;
    public float jumpVel;
    public float jumpTimerMax;
    bool isJumping;
    float jumpTimer;
    bool doubleJump;
    public float maxDownVel;
    public float onPlatformTimer;
    public float onPlatformTimerMax;
    public bool onTopOfPlatform;

    [Header("Character Parts")]
    List<Collider> characterParts = new List<Collider>();
    bool isRagdoll;

    [Header("Test")]
    public bool test;

    private void Awake()
    {
        //Rewired Code
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;

        if (!test)
        {
            Collider[] characterJoints = gameObject.GetComponentsInChildren<Collider>();
            foreach (Collider c in characterJoints)
            {
                if (c.gameObject != this.gameObject)
                {
                    c.isTrigger = true;
                    c.attachedRigidbody.isKinematic = true;
                    characterParts.Add(c);

                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateRagdoll();
        }

    }

    private void FixedUpdate()
    {
        FixedMovement();

        Gravity();
    }

    void Movement()
    {
        if (test)
        {
            velocity = new Vector3(player1.GetAxis("MoveX") * -speed, velocityY, player1.GetAxis("MoveZ") * -speed);
            //jump logic
            if (onPlatformTimer > 0)
            {
                if (player1.GetButtonDown("Jump"))
                {
                    velocityY = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                }
            }
            if (onPlatformTimer < 0 && doubleJump)
            {
                if (player1.GetButtonDown("Jump"))
                {
                    velocityY = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                    doubleJump = false;
                }
            }
            if (player1.GetButton("Jump") && isJumping)
            {
                velocityY = jumpVel;
                jumpTimer -= Time.deltaTime;
            }

            if (player1.GetButtonUp("Jump") || jumpTimer <= 0)
            {
                isJumping = false;
            }

            velocity = transform.worldToLocalMatrix.inverse * velocity;

            //set timer that will let the player jump slightly off the platform
            if (onTopOfPlatform)
            {
                onPlatformTimer = onPlatformTimerMax;
                doubleJump = true;
            }
            else
            {
                onPlatformTimer -= Time.deltaTime;
            }

            return;
        }
        if (playerNum == 1)
        {
            velocity = new Vector3(player2.GetAxis("MoveX") * speed, velocityY, player2.GetAxis("MoveZ") * speed);
            //jump logic
            if (onPlatformTimer > 0)
            {
                if (player1.GetButtonDown("Jump"))
                {
                    velocityY = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                }
            }
            if (onPlatformTimer < 0 && doubleJump)
            {
                if (player1.GetButtonDown("Jump"))
                {
                    velocityY = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                    doubleJump = false;
                }
            }
            if (player1.GetButton("Jump") && isJumping)
            {
                velocityY = jumpVel;
                jumpTimer -= Time.deltaTime;
            }

            if (player1.GetButtonUp("Jump") || jumpTimer <= 0)
            {
                isJumping = false;
            }
        }
        else if(playerNum == 2)
        {
            velocity = new Vector3(player1.GetAxis("MoveX") * speed, velocityY, player1.GetAxis("MoveZ") * speed);
            //jump logic
            if (onPlatformTimer > 0)
            {
                if (player2.GetButtonDown("Jump"))
                {
                    velocityY = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                }
            }
            if (onPlatformTimer < 0 && doubleJump)
            {
                if (player2.GetButtonDown("Jump"))
                {
                    velocityY = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                    doubleJump = false;
                }
            }
            if (player2.GetButton("Jump") && isJumping)
            {
                velocityY = jumpVel;
                jumpTimer -= Time.deltaTime;
            }

            if (player2.GetButtonUp("Jump") || jumpTimer <= 0)
            {
                isJumping = false;
            }
        }
        velocity = transform.worldToLocalMatrix.inverse * velocity;

        

        //set timer that will let the player jump slightly off the platform
        if (onTopOfPlatform)
        {
            onPlatformTimer = onPlatformTimerMax;
            doubleJump = true;
        }
        else
        {
            onPlatformTimer -= Time.deltaTime;
        }

    }

    void FixedMovement()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    void Gravity()
    {
        if (!isRagdoll)
        {
            //gravity logic
            if (velocityY > -maxDownVel)
            { //if we haven't reached maxDownVel
                if (velocityY > 0)
                { //if player is moving up
                    velocityY -= gravityUp * Time.fixedDeltaTime;
                }
                else
                { //if player is moving down
                    velocityY -= gravityDown * Time.fixedDeltaTime;
                }
            }
        }
    }

    void ActivateRagdoll()
    {
        if (isRagdoll)
        {
            this.GetComponent<BoxCollider>().enabled = true;
            if (this.GetComponent<Animator>() != null)
            {
                this.GetComponent<Animator>().enabled = true;
                //add back original animator avatar
                this.GetComponent<Animator>().avatar = null;
            }
            foreach (Collider c in characterParts)
            {
                if (c.gameObject != this.gameObject)
                {
                    c.isTrigger = true;
                    c.attachedRigidbody.velocity = Vector3.zero;
                    c.attachedRigidbody.isKinematic = true;
                }
            }
            isRagdoll = false;
        }
        else
        {
            this.GetComponent<BoxCollider>().enabled = false;
            if (this.GetComponent<Animator>() != null)
            {
                this.GetComponent<Animator>().enabled = false;
                this.GetComponent<Animator>().avatar = null;
            }
            foreach (Collider c in characterParts)
            {
                if (c.gameObject != this.gameObject)
                {
                    c.isTrigger = false;
                    c.attachedRigidbody.isKinematic = false;
                }
            }
            isRagdoll = true;
        }
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        Debug.Log(collisionInfo.gameObject.name);
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            //am I coming from the top/bottom?
            if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
            {
                velocityY = 0; //stop vertical velocity
                if (contact.normal.y >= 0)
                { //am I hitting the top of the platform?

                    onTopOfPlatform = true;
                }
                //am I hitting the bottom of a platform?
                if (contact.normal.y < 0)
                {
                    //hitHead = true;
                    velocityY = 0;
                    //gotHitTimer = 0;
                    //maxKnockbackTime = 0;

                }
            }
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            //am I coming from the top/bottom?
            if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
            {
                //velocityY = 0; //stop vertical velocity
                if (contact.normal.y >= 0)
                { //am I hitting the top of the platform?

                    onTopOfPlatform = true;
                }
                //am I hitting the bottom of a platform?
                if (contact.normal.y < 0)
                {
                    //hitHead = true;
                    velocityY = 0;
                    //gotHitTimer = 0;
                    //maxKnockbackTime = 0;

                }
            }
        }
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
        onTopOfPlatform = false;
    }

    //[REWIRED METHODS]
    //these two methods are for ReWired, if any of you guys have any questions about it I can answer them, but you don't need to worry about this for working on the game - Buscemi
    void OnControllerConnected(ControllerStatusChangedEventArgs arg)
    {
        CheckController(player1);
        CheckController(player2);
    }

    void CheckController(Player player)
    {
        foreach (Joystick joyStick in player.controllers.Joysticks)
        {
            var ds4 = joyStick.GetExtension<DualShock4Extension>();
            if (ds4 == null) continue;//skip this if not DualShock4
            switch (playerNum)
            {
                case 4:
                    ds4.SetLightColor(Color.yellow);
                    break;
                case 3:
                    ds4.SetLightColor(Color.green);
                    break;
                case 2:
                    ds4.SetLightColor(Color.blue);
                    break;
                case 1:
                    ds4.SetLightColor(Color.red);
                    break;
                default:
                    ds4.SetLightColor(Color.white);
                    Debug.LogError("Player Num is 0, please change to a number > 0");
                    break;
            }
        }
    }

}
