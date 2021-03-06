﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public static float time = 119;

    void Start()
    {
        StartCoundownTimer();
    }

    void StartCoundownTimer()
    {
        if (timerText != null)
        {
            time = 119;
            timerText.text = "Time Left: 2:00";
            InvokeRepeating("UpdateTimer", 0.0f, 0.01667f);
        }
    }

    void UpdateTimer()
    {
        if (timerText != null)
        {
            time -= Time.deltaTime;
            string minutes = Mathf.Floor(time / 60).ToString("0");
            string seconds = (time % 60).ToString("00");
            string fraction = ((time * 100) % 100).ToString("000");
            timerText.text = minutes + ":" + seconds;
        }

        if(time <= 0)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
