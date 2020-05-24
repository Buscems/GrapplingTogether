// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class PlayerMovement : MonoBehaviour {


    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player player1;
    public Player player2;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public static bool playingAlone;

    Vector3 startPos;
    [HideInInspector]
    public bool ready;

    public GrapplingHook grappling;

    //Assingables
    public Transform playerCam;
    public Transform orientation;
    
    //Other
    private Rigidbody rb;

    //Rotation and look
    private float xRotation;
    public float sensitivity;
    private float sensMultiplier = 1f;
    
    //Movement
    public float moveSpeed;
    public float maxSpeed;
    public bool grounded;
    public LayerMask whatIsGround;
    Vector3 velocity;
    float lookX, lookY;
    
    public float counterMovement;
    private float threshold = 0.01f;
    public float maxSlopeAngle;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce;
    public float slideCounterMovement;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce;
    
    //Input
    public float x, y;
    bool jumping, sprinting, crouching;
    
    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    AudioSource splash;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        //Rewired Code
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        splash = GetComponent<AudioSource>();
    }
    
    void Start() {
        startPos = transform.position;
        playerScale =  transform.localScale;
        Cursor.visible = false;
    }

    
    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        MyInput();
        Look();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        if (!playingAlone)
        {
            if (playerNum == 1)
            {
                x = player2.GetAxisRaw("MoveX");
                y = player2.GetAxisRaw("MoveZ");
                jumping = player1.GetButton("Jump");
            }
            if (playerNum == 2)
            {
                x = player1.GetAxisRaw("MoveX");
                y = player1.GetAxisRaw("MoveZ");
                jumping = player2.GetButton("Jump");
            }
        }
        else
        {
            x = player1.GetAxisRaw("MoveX");
            y = player1.GetAxisRaw("MoveZ");
            jumping = player1.GetButton("Jump");
        }
        crouching = Input.GetKey(KeyCode.LeftControl);
     
        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch() {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        //Extra gravity
        //original
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        //my version
        /*
        if (!grounded)
        {
            velocity.y -= 5 * Time.fixedDeltaTime;
        }
        else if(!jumping)
        {
            velocity.y = 0;
        }
        */
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;
        
        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = 1.3f;
            multiplierV = 1.3f;
        }
        
        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        //original
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        //my version
        //velocity.z = (orientation.transform.forward * y).magnitude * Mathf.Sign(y);
        //velocity.x = (orientation.transform.right * x).magnitude * Mathf.Sign(x);
        //rb.MovePosition(rb.position + velocity + orientation.transform.forward * y * Time.fixedDeltaTime * multiplier * multiplierV);
        //rb.MovePosition(rb.position + velocity + orientation.transform.right * x * Time.fixedDeltaTime * multiplier);
    }

    private void Jump() {
        if (grounded && readyToJump) {
            readyToJump = false;

            //Add jump forces
            //original
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //velocity.y = jumpForce;

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    
    private void ResetJump() {
        readyToJump = true;
    }
    
    private float desiredX;
    private void Look() {
        if (!playingAlone)
        {
            if (playerNum == 1)
            {
                lookX = player1.GetAxis("LookX") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
                lookY = player1.GetAxis("LookY") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
            }
            if (playerNum == 2)
            {
                lookX = player2.GetAxis("LookX") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
                lookY = player2.GetAxis("LookY") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
            }
        }
        else
        {
            lookX = player1.GetAxis("LookX") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
            lookY = player1.GetAxis("LookY") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        }
        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + lookX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        
        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
        
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Respawn")
        {
            rb.velocity = Vector3.zero;
            transform.position = startPos;
            grappling.StopGrapple();
            splash.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Finish")
        {
            ready = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            ready = false;
        }
    }

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
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
