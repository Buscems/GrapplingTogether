using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextLevel : MonoBehaviour
{

    public int playerAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextLevel(int currentLevel)
    {
        switch (currentLevel)
        {
            case 0:
                if(playerAmount > 1)
                {
                    PlayerMovement.playingAlone = false;
                    GrapplingHook.playingAlone = false;
                }
                else
                {
                    PlayerMovement.playingAlone = true;
                    GrapplingHook.playingAlone = true;
                }
                SceneManager.LoadScene("Level1");
                break;
            case 1:
                SceneManager.LoadScene("Level2");
                break;
            case 2:

                break;
        }
    }

}
