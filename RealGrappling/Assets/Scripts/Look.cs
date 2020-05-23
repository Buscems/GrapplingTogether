using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class Look : MonoBehaviour
{
    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player player1;
    public Player player2;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public float sensitivity;

    public Transform body;

    float Xrotation;

    Vector2 lookDir;

    private void Awake()
    {
        //Rewired Code
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (playerNum == 1)
        {
            lookDir.x = player1.GetAxis("LookX") * sensitivity * Time.deltaTime;
            lookDir.y = player1.GetAxis("LookY") * sensitivity * Time.deltaTime;
        }
        else if(playerNum == 2)
        {
            lookDir.x = player2.GetAxis("LookX") * sensitivity * Time.deltaTime;
            lookDir.y = player2.GetAxis("LookY") * sensitivity * Time.deltaTime;
        }
        Xrotation -= lookDir.y;
        Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(Xrotation, 0, 0f);

        body.Rotate(Vector3.up * lookDir.x);
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
