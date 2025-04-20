using MidiPlayerTK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! [LoadMidiAndPlay]
public class LoadMidiAndPlay : MonoBehaviour
{
    //  Create a new scene
    //     -   add a MidiFilePLayer prefab
    //     -   create an empty gameobject and add this script

    // MPTK component able to play a Midi file from your list of Midi file. 
    // This PreFab must exist in your scene.
    public MidiFilePlayer midiFilePlayer;
    ///     
    // Start is called before the first frame update
    void Start()
    {
        // You can search the midiFilePlayer with FindFirstObjectByType<MidiFilePlayer>() or set directly in the inspector.
        if (midiFilePlayer == null)
            midiFilePlayer = FindFirstObjectByType<MidiFilePlayer>();

        // Index of the midi from the Midi DB (find it with 'Midi File Setup' from the Maestro menu)
        midiFilePlayer.MPTK_MidiIndex = 0;

        MidiLoad midiloaded = midiFilePlayer.MPTK_Load();

        if (midiloaded != null)
        {
            Debug.Log($"Duration: {midiloaded.MPTK_Duration.TotalSeconds} Tempo: {midiloaded.MPTK_InitialTempo} seconds Count MIDI Events: {midiloaded.MPTK_ReadMidiEvents().Count} ");

            // Now, play!
            midiFilePlayer.MPTK_Play(alreadyLoaded: true);
        }
    }

}
//! [LoadMidiAndPlay]


