using System;
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

    public int curseCounter;
    public Image red;
    public float x;

    // Start is called before the first frame update
    void Start()
    {
        actions.Add("fuck", MakeHarder);
        actions.Add("shit", MakeHarder);
        actions.Add("shitting", MakeHarder);
        actions.Add("bitch", MakeHarder);
        actions.Add("ass", MakeHarder);
        actions.Add("cunt", MakeHarder);
        actions.Add("motherfucker", MakeHarder);
        actions.Add("slut", MakeHarder);
        actions.Add("hoe", MakeHarder);
        actions.Add("whore", MakeHarder);
        actions.Add("fucking", MakeHarder);
        actions.Add("twat", MakeHarder);
        actions.Add("pussy", MakeHarder);
        actions.Add("dick", MakeHarder);
        actions.Add("cock", MakeHarder);
        actions.Add("cocksucker", MakeHarder);

        actions.Add("sorry", Apologize);

        keyWordRecognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
        keyWordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keyWordRecognizer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        x = .15f * curseCounter;
        if(x >= .75f)
        {
            x = .75f;
        }
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
        red.color = new Color(1, 0, 0, x);
    }

    private void Apologize()
    {
        curseCounter = 0;
        red.color = new Color(1, 0, 0, 0);
    }
}

