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

    bool isGrounded;

    Vector3 velocity;
    float velocityY = 0;

    Rigidbody rb;

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

    private void Awake()
    {
        //Rewired Code
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        Gravity();
    }

    private void FixedUpdate()
    {
        FixedMovement();
    }

    void Movement()
    {
        if (playerNum == 1)
        {
            velocity = new Vector3(player2.GetAxis("MoveX"), velocityY, player2.GetAxis("MoveZ")) * speed;
            //jump logic
            if (onPlatformTimer > 0)
            {
                if (player1.GetButtonDown("Jump"))
                {
                    velocity.y = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                }
            }
            if (onPlatformTimer < 0 && doubleJump)
            {
                if (player1.GetButtonDown("Jump"))
                {
                    velocity.y = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                    doubleJump = false;
                }
            }
            if (player1.GetButton("Jump") && isJumping)
            {
                velocity.y = jumpVel;
                jumpTimer -= Time.deltaTime;
            }

            if (player1.GetButtonUp("Jump") || jumpTimer <= 0)
            {
                isJumping = false;
            }
        }
        else if(playerNum == 2)
        {
            velocity = new Vector3(player1.GetAxis("MoveX"), velocityY, player1.GetAxis("MoveZ")) * speed;
            //jump logic
            if (onPlatformTimer > 0)
            {
                if (player2.GetButtonDown("Jump"))
                {
                    velocity.y = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                }
            }
            if (onPlatformTimer < 0 && doubleJump)
            {
                if (player2.GetButtonDown("Jump"))
                {
                    velocity.y = jumpVel;
                    jumpTimer = jumpTimerMax;
                    isJumping = true;
                    doubleJump = false;
                }
            }
            if (player2.GetButton("Jump") && isJumping)
            {
                velocity.y = jumpVel;
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
        //gravity logic
        if (velocity.y > -maxDownVel)
        { //if we haven't reached maxDownVel
            if (velocity.y > 0)
            { //if player is moving up
                velocity.y -= gravityUp * Time.fixedDeltaTime;
            }
            else
            { //if player is moving down
                velocity.y -= gravityDown * Time.fixedDeltaTime;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        foreach (ContactPoint2D contact in collisionInfo.contacts)
        {
            //am I coming from the top/bottom?
            if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
            {
                velocity.y = 0; //stop vertical velocity
                if (contact.normal.y >= 0)
                { //am I hitting the top of the platform?

                    onTopOfPlatform = true;
                }
                //am I hitting the bottom of a platform?
                if (contact.normal.y < 0)
                {
                    //hitHead = true;
                    velocity.y = 0;
                    //gotHitTimer = 0;
                    //maxKnockbackTime = 0;

                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collisionInfo)
    {
        foreach (ContactPoint2D contact in collisionInfo.contacts)
        {
            //am I coming from the top/bottom?
            if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
            {
                //velocity.y = 0; //stop vertical velocity
                if (contact.normal.y >= 0)
                { //am I hitting the top of the platform?

                    onTopOfPlatform = true;
                }
                //am I hitting the bottom of a platform?
                if (contact.normal.y < 0)
                {
                    //hitHead = true;
                    velocity.y = 0;
                    //gotHitTimer = 0;
                    //maxKnockbackTime = 0;

                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collisionInfo)
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
