using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class ExampleMidiControl3 : MonoBehaviour
{
    [Header("Maestro MPTK Player")]
    public MidiFilePlayer midiFilePlayer; // ✅ make public so you can assign it via Inspector

    private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
    private const int MaxTicks = 10;
    private float trackSpacing = 5f; // Y-axis spacing between tracks
    private float moveSpeed = 2.0f;  // 

    void Start()
    {
        if (midiFilePlayer == null)
            midiFilePlayer = FindObjectOfType<MidiFilePlayer>();

        midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
    }

    void Update()
    {
        foreach (var trackPair in trackCubes)
        {
            foreach (GameObject cube in trackPair.Value)
            {
                Vector3 pos = cube.transform.position;
                pos.x += moveSpeed * Time.deltaTime;
                cube.transform.position = pos;
            }
        }
    }

    void VisualizeNotes(List<MPTKEvent> events)
    {
        foreach (MPTKEvent note in events)
        {
            if (note.Command == MPTKCommand.NoteOn)
            {
                int track = (int)note.Track;
                float pitch = (float)note.Value;
                float duration = (float)note.Duration / 100f;
                float velocity = (float)note.Velocity / 10f;

                if (!trackXOffsets.ContainsKey(track))
                    trackXOffsets[track] = 0f;

                if (!trackCubes.ContainsKey(track))
                    trackCubes[track] = new Queue<GameObject>();

                Vector3 size = new Vector3(duration, velocity, 1f);
                float zOffset = -15f;
                Vector3 position = new Vector3(
                    0f,
                    track * trackSpacing,
                    pitch + zOffset
                );

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = size;
                cube.transform.position = position;
                CubeInfo cubeInfo = cube.AddComponent<CubeInfo>() as CubeInfo;
                cubeInfo.pitch = pitch.ToString();
                cubeInfo.duration = duration.ToString();
                cubeInfo.volume = velocity.ToString();

                float hue = Mathf.InverseLerp(21f, 108f, pitch);
                cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

                trackCubes[track].Enqueue(cube);

                if (trackCubes[track].Count > MaxTicks)
                {
                    GameObject oldCube = trackCubes[track].Dequeue();
                    Destroy(oldCube);
                }
            }
        }
    }

    // ✅ Public methods for UI buttons
    public void PlaySong1()
    {
        if (midiFilePlayer != null)
        {
            midiFilePlayer.MPTK_Stop();
            midiFilePlayer.MPTK_MidiName = "2 - All Night Long";
            midiFilePlayer.MPTK_Play();
        }
    }

    public void PlaySong2()
    {
        if (midiFilePlayer != null)
        {
            midiFilePlayer.MPTK_MidiName = "6 - Bach - little fugue G minor";
            midiFilePlayer.MPTK_Play();
        }
    }
}
