using System.Collections;
using System.Collections.Generic;
using HGS.Tone;
using UnityEngine;

public class SynthScript : MonoBehaviour
{

    [SerializeField]
    private ToneSynth _synth;

    // Start is called before the first frame update
    void Start()
    {
        _synth.SetInstrument(MidiInstrumentCode.Acoustic_Grand_Piano);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayNote(Note toPlay)
    {
        if (!toPlay.Equals(Note.GetPause()))
        {
            _synth.TriggerAttackAndRelease(ToneNote.Parse(
                toPlay.ToStringInternational()
            ), duration: 1.5f);
        }
    }

}
