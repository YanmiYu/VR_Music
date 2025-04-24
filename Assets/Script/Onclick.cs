using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class ExampleMidiControl : MonoBehaviour
{
    private MidiFilePlayer midiFilePlayer;
    private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
    private const int MaxTicks = 10;
    private float trackSpacing = 5f; // Y-axis spacing between tracks
    private float moveSpeed = 2.0f;  // 移动速度

    void Start()
    {
        midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
        midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
    }

    void Update()
    {
        // 在Update中移动所有立方体
        foreach (var trackPair in trackCubes)
        {
            foreach (GameObject cube in trackPair.Value)
            {
                Vector3 pos = cube.transform.position;
                pos.x += moveSpeed * Time.deltaTime; // 使用Time.deltaTime使移动与帧率无关
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
                float pitch = (float)note.Value;               // Z-axis
                float duration = (float)note.Duration / 100f;  // X-axis (width of cube)
                float velocity = (float)note.Velocity / 10f;   // Y-size (height of cube)

                // Ensure offset tracker
                if (!trackXOffsets.ContainsKey(track))
                    trackXOffsets[track] = 0f;

                // Ensure queue
                if (!trackCubes.ContainsKey(track))
                    trackCubes[track] = new Queue<GameObject>();

                // Calculate cube size and position
                Vector3 size = new Vector3(duration, velocity, 1f);
                Vector3 position = new Vector3(
                    0f,                             // 新立方体从起点开始
                    track * trackSpacing,           // Y row for the track
                    pitch                           // Z for pitch
                );

                // Create cube
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = size;
                cube.transform.position = position;

                // Color by pitch (optional visual enhancement)
                float hue = Mathf.InverseLerp(21f, 108f, pitch);
                cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

                // Add to queue
                trackCubes[track].Enqueue(cube);
                
                // 仍然保持最多10个立方体的限制
                if (trackCubes[track].Count > MaxTicks)
                {
                    GameObject oldCube = trackCubes[track].Dequeue();
                    Destroy(oldCube);
                }
            }
        }
    }
}