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

    // Start is called before the first frame update
    void Start()
    {
        actions.Add("fuck", MakeHarder);
        actions.Add("fucking", MakeHarder);
        actions.Add("motherfucker", MakeHarder);
        actions.Add("what the fuck", MakeHarder);
        actions.Add("fuck this", MakeHarder);
        actions.Add("fuck off", MakeHarder);
        actions.Add("are you fucking kidding me", MakeHarder);
        actions.Add("come the fuck on", MakeHarder);
        actions.Add("shit", MakeHarder);

        actions.Add("sorry", Apologize);

        keyWordRecognizer = new KeywordRecognizer(actions.Keys.ToArray(), ConfidenceLevel.Low);
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
    }

    private void Apologize()
    {
        if (curseCounter >= 1)
        {
            curseCounter = 0;
            GameTimer.time += 10f;
        }
    }
}

