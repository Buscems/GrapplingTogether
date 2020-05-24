using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CheckPlayers : MonoBehaviour
{

    public PlayerMovement player1, player2;

    public TextMeshProUGUI winText;
    public TextMeshProUGUI timer;

    public GameTimer gt;

    public int level;

    //public Animator

    // Start is called before the first frame update
    void Start()
    {
        winText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(player1.ready && player2.ready)
        {
            if (level == 2)
            {
                winText.enabled = true;
                winText.text = "Yin Wou.";
            }
            timer.enabled = false;
            gt.enabled = false;
        }

    }
}
