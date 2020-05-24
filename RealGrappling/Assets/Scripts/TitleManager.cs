using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{

    public PlayerMovement player1, player2;
    private bool player1On, player2On;

    public TextMeshProUGUI amountText;

    public int timer;

    public Animator fade;

    bool dont;

    string player;

    // Start is called before the first frame update
    void Start()
    {
        amountText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(player1On || player2On)
        {
            amountText.enabled = true;
        } 
        else
        {
            amountText.enabled = false;
        }

        /*
        if(player1.ready && player2.ready)
        {
            amountText.enabled = true;
            if (!dont)
            {
                player = "Together";
                StopAllCoroutines();
                StartCoroutine(StartTimer());
            }
        }
        else if(player1.ready || player2.ready)
        {
            amountText.enabled = true;
            if (!dont)
            {
                player = "Alone";
                StopAllCoroutines();
                StartCoroutine(StartTimer());
            }
        }
        else
        {
            amountText.enabled = false;
            timer = 5;
            dont = false;
            StopAllCoroutines();
        }
        */
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();

        timer = 5;

        Debug.Log(collision.gameObject.transform.parent.gameObject.name);

        if (collision.gameObject.transform.parent.gameObject.name == "Player1")
        {
            player1On = true;
        }
        if(collision.gameObject.transform.parent.gameObject.name == "Player2")
        {
            player2On = true;
        }

        // Both
        if(player1On && player2On)
        {
            player = "Together";
            StartCoroutine(StartTimer());
        }
        // Only 1
        else if (player1On || player2On)
        {
            player = "Alone";
            StartCoroutine(StartTimer());
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        StopAllCoroutines();



        timer = 5;

        if (collision.gameObject.transform.parent.gameObject.name == "Player1")
        {
            player1On = false;
        }
        if (collision.gameObject.transform.parent.gameObject.name == "Player2")
        {
            player2On = false;
        }

        // Both
        if (player1On && player2On)
        {
            player = "Together";
            StartCoroutine(StartTimer());
        }
        // Only 1
        else if (player1On || player2On)
        {
            player = "Alone";
            StartCoroutine(StartTimer());
        }
    }

    IEnumerator StartTimer()
    {
        amountText.enabled = true;
        dont = true;
        timer = 5;
        if (amountText.enabled)
        {
            amountText.text = player + ":\nStarting in " + timer;
        }
        if (player == "Alone")
        {
            fade.GetComponent<GoToNextLevel>().playerAmount = 1;
        }
        if (player == "Together")
        {
            fade.GetComponent<GoToNextLevel>().playerAmount = 2;
        }
        yield return new WaitForSeconds(1);
        timer--;
        if (amountText.enabled)
        {
            amountText.text = player + ":\nStarting in " + timer;
        }
        yield return new WaitForSeconds(1);
        timer--;
        if (amountText.enabled)
        {
            amountText.text = player + ":\nStarting in " + timer;
        }
        yield return new WaitForSeconds(1);
        timer--;
        if (amountText.enabled)
        {
            amountText.text = player + ":\nStarting in " + timer;
        }
        yield return new WaitForSeconds(1);
        timer--;
        if (amountText.enabled)
        {
            amountText.text = player + ":\nStarting in " + timer;
        }
        yield return new WaitForSeconds(1);
        timer--;
        if (amountText.enabled)
        {
            amountText.text = player + ":\nStarting in " + timer;
        }
        fade.SetTrigger("Fade");
    }

}
