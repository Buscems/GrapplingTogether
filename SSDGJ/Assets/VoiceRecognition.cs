using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;

public class VoiceRecognition : MonoBehaviour
{
    private KeywordRecognizer keyWordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    public float difficultyScale = 1f;

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
        //increase difficulty here
        difficultyScale += 1;
    }
}

