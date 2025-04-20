using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class MidiNoteHandler : MonoBehaviour
{
   private MidiFilePlayer midiFilePlayer;

   // Start is called before the first frame update
   void Start()
   {
       // get midi player object
       midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
       midiFilePlayer.OnEventNotesMidi.AddListener(NoteActions);
   }

   public void NoteActions(List<MPTKEvent> mptkEvents) {
       foreach(MPTKEvent note in mptkEvents) {
           if (note.Command == MPTKCommand.NoteOn) { // if the note is being played
               int noteValue = note.Value; // get the note value
               string noteLabel = HelperNoteLabel.LabelFromMidi(noteValue); // get the note label
               char noteOctave = noteLabel[1]; // get the octave of the note
               GameObject octaveModel = GameObject.Find("Cube" + noteOctave); // get the correct octave gameobject
               float volume = note.Velocity; // get the note velocity
               long duration = note.Duration; // get the note duration
               StartCoroutine(OctaveHeightChanger(octaveModel, duration, volume));
           }
       }
   }

   /// <summary>
   /// this coroutine changes the octave gameobject's height for the duration
   /// + half a second so short notes can be visible to the visualizer
   /// </summary>
   IEnumerator OctaveHeightChanger(GameObject octaveModel, long duration, float volume) {
       octaveModel.transform.localScale = new Vector3(1f, volume / 10f, 1f);
       yield return new WaitForSeconds(duration/1000 + 0.5f);
       octaveModel.transform.localScale = new Vector3(1f, 0.1f, 1f);
   }
}
