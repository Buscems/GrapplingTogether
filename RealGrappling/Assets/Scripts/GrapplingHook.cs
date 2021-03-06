﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;
using UnityEngine.SceneManagement;

public class GrapplingHook : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player player1;
    public Player player2;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public static bool playingAlone;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask grappleMask;

    public Transform gunTip, playerCamera, player;

    public float maxGrappleDistance;

    public float maxJointDistance;
    public float minJointDistance;

    private SpringJoint joint;

    private void Awake()
    {

        //Rewired Code
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;

        lr = GetComponent<LineRenderer>();

        if (SceneManager.GetActiveScene().name == "Title")
        {
            playingAlone = false;
        }

    }

    private void Update()
    {
        if (!playingAlone)
        {
            if (playerNum == 1)
            {
                if (player2.GetButtonDown("Shoot"))
                {
                    Debug.Log("pls2");
                    StartCoroutine("StartGrapple");
                }
                else if (player2.GetButtonUp("Shoot"))
                {
                    StopGrapple();
                }
            }
            if (playerNum == 2)
            {
                if (player1.GetButtonDown("Shoot"))
                {
                    Debug.Log("pls1");
                    StartCoroutine(StartGrapple());
                }
                else if (player1.GetButtonUp("Shoot"))
                {
                    StopGrapple();
                }
            }
        }
        else
        {
            if (player1.GetButtonDown("Shoot"))
            {
                StartCoroutine("StartGrapple");
            }
            else if (player1.GetButtonUp("Shoot"))
            {
                StopGrapple();
            }
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    IEnumerator StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(origin: playerCamera.position, direction: playerCamera.forward, out hit, maxGrappleDistance, grappleMask))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * maxJointDistance;
            joint.minDistance = distanceFromPoint * minJointDistance;

            //joint.spring = 75f;
            //joint.damper = 50f;
            //joint.massScale = 33f;

            joint.spring = 10;
            joint.damper = 0;
            joint.massScale = 5;

            lr.positionCount = 2;

            
            yield return new WaitForSeconds(.3f);
            StopGrapple();

        }
    }

    void DrawRope()
    {
        if (!joint) return;
        lr.positionCount = 2;
        lr.SetPosition(index: 0, gunTip.position);
        lr.SetPosition(index: 1, grapplePoint);
    }

    public void StopGrapple()
    {
        Destroy(joint);
        lr.positionCount = 0;
    } 

    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
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
