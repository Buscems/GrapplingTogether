﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEngine.UI;

public class VoiceRecognition : MonoBehaviour
{
    private KeywordRecognizer keyWordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    public Death[] deaths;

    public int curseCounter;

    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {

        audio = GetComponent<AudioSource>();

        actions.Add("fuck", MakeHarder);
        actions.Add("fucking", MakeHarder);
        actions.Add("motherfucker", MakeHarder);
        actions.Add("what the fuck", MakeHarder);
        actions.Add("fuck this", MakeHarder);
        actions.Add("fuck off", MakeHarder);
        actions.Add("are you fucking kidding me", MakeHarder);
        actions.Add("come the fuck on", MakeHarder);
        actions.Add("fuck man", MakeHarder);
        actions.Add("shit", MakeHarder);
        actions.Add("bitch", MakeHarder);
        actions.Add("slut", MakeHarder);
        actions.Add("whore", MakeHarder);


        actions.Add("sorry", Apologize);

        keyWordRecognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Medium);
        keyWordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keyWordRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void RecognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void MakeHarder()
    {
        curseCounter += 1;
        ShakeBehavior.shakeDuration = .5f;
        GameTimer.time -= 10f;
        deaths[0].speed += .05f;
        deaths[1].speed += .05f;
        audio.Play();
    }

    private void Apologize()
    {
        if (curseCounter >= 1)
        {
            curseCounter = 0;
            GameTimer.time += 10f;
            deaths[0].speed = .05f;
            deaths[1].speed = .05f;
        }
    }
}

