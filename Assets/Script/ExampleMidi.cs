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


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl : MonoBehaviour
// {
//     private MidiFilePlayer midiFilePlayer;
//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f; // Y-axis spacing between tracks

//     void Start()
//     {
//         midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void VisualizeNotes(List<MPTKEvent> events)
//     {
//         foreach (MPTKEvent note in events)
//         {
//             if (note.Command == MPTKCommand.NoteOn)
//             {
//                 int track = (int)note.Track;
//                 float pitch = (float)note.Value;               // Z-axis
//                 float duration = (float)note.Duration / 100f;  // X-axis (width of cube)
//                 float velocity = (float)note.Velocity / 10f;   // Y-size (height of cube)

//                 // Ensure offset tracker
//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 // Ensure queue
//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 // Calculate cube size and position
//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 Vector3 position = new Vector3(
//                     trackXOffsets[track] + duration / 2f, // Center the cube based on duration
//                     track * trackSpacing,                // Y row for the track
//                     pitch                                // Z for pitch
//                 );

//                 // Create cube
//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;

//                 // Color by pitch (optional visual enhancement)
//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 // Add to queue and update offset
//                 trackCubes[track].Enqueue(cube);
//                 trackXOffsets[track] += duration;

//                 // Maintain only 10 most recent cubes
//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     float oldWidth = oldCube.transform.localScale.x;

//                     // Shift remaining cubes left
//                     foreach (GameObject obj in trackCubes[track])
//                     {
//                         Vector3 pos = obj.transform.position;
//                         pos.x -= oldWidth;
//                         obj.transform.position = pos;
//                     }

//                     Destroy(oldCube);
//                     trackXOffsets[track] -= oldWidth;
//                 }
//             }
//         }
//     }
// }

////////////// moving correctly with color ////////////// ////////////// ////////////// 

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl : MonoBehaviour
// {
//     private MidiFilePlayer midiFilePlayer;
//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f; // Y-axis spacing between tracks

//     void Start()
//     {
//         midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void VisualizeNotes(List<MPTKEvent> events)
//     {
//         foreach (MPTKEvent note in events)
//         {
//             if (note.Command == MPTKCommand.NoteOn)
//             {
//                 int track = (int)note.Track;
//                 float pitch = (float)note.Value;               // Z-axis
//                 float duration = (float)note.Duration / 100f;  // X-axis (width of cube)
//                 float velocity = (float)note.Velocity / 10f;   // Y-size (height of cube)

//                 // Ensure offset tracker
//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 // Ensure queue
//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 // Calculate cube size and position
//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 Vector3 position = new Vector3(
//                     trackXOffsets[track] + duration / 2f, // Center the cube based on duration
//                     track * trackSpacing,                // Y row for the track
//                     pitch                                // Z for pitch
//                 );

//                 // Create cube
//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;

//                 // Color by pitch (optional visual enhancement)
//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 // Add to queue and update offset
//                 trackCubes[track].Enqueue(cube);
//                 trackXOffsets[track] += duration;

//                 // Maintain only 10 most recent cubes
//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     float oldWidth = oldCube.transform.localScale.x;

//                     // Shift remaining cubes left
//                     foreach (GameObject obj in trackCubes[track])
//                     {
//                         Vector3 pos = obj.transform.position;
//                         pos.x -= oldWidth;
//                         obj.transform.position = pos;
//                     }

//                     Destroy(oldCube);
//                     trackXOffsets[track] -= oldWidth;
//                 }
//             }
//         }
//     }
// }



////////////// moving with color ////////////// ////////////// ////////////// 
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl : MonoBehaviour
// {
//     private MidiFilePlayer midiFilePlayer;
//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private const int MaxTicks = 10;
//     private float trackSpacing = 6f;
//     private float scrollSpeed = 2f; // Units per second

//     // MIDI pitch range (for color)
//     private float minPitch = 21f;
//     private float maxPitch = 108f;

//     void Start()
//     {
//         midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         // Move all active cubes from left to right over time
//         foreach (var trackQueue in trackCubes.Values)
//         {
//             foreach (var cube in trackQueue)
//             {
//                 if (cube != null)
//                 {
//                     cube.transform.position += Vector3.right * scrollSpeed * Time.deltaTime;
//                 }
//             }
//         }
//     }

//     void VisualizeNotes(List<MPTKEvent> events)
//     {
//         foreach (MPTKEvent note in events)
//         {
//             if (note.Command == MPTKCommand.NoteOn)
//             {
//                 int track = (int)note.Track;
//                 float pitch = (float)note.Value;
//                 float duration = (float)note.Duration / 100f;
//                 float velocity = (float)note.Velocity / 10f;

//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 Vector3 startPosition = new Vector3(
//                     0f,                             // start from left
//                     track * trackSpacing,          // row by track
//                     pitch                          // height is pitch
//                 );

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = startPosition;

//                 // Assign color based on pitch
//                 float hue = Mathf.InverseLerp(minPitch, maxPitch, pitch);
//                 Color color = Color.HSVToRGB(hue, 1f, 1f);
//                 cube.GetComponent<Renderer>().material.color = color;

//                 // Manage history
//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 trackCubes[track].Enqueue(cube);
//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }
// }


////////////// regular  ////////////// ////////////// ////////////// 
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl : MonoBehaviour
// {
//     private MidiFilePlayer midiFilePlayer;
//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private const int MaxTicks = 10;
//     private float trackSpacing = 3f;

//     void Start()
//     {
//         midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void VisualizeNotes(List<MPTKEvent> events)
//     {
//         foreach (MPTKEvent note in events)
//         {
//             if (note.Command == MPTKCommand.NoteOn)
//             {
//                 int track = (int)note.Track;
//                 float pitch = (float)note.Value;               // Z-axis
//                 float duration = (float)note.Duration / 100f;  // X-axis
//                 float velocity = (float)note.Velocity / 10f;   // Y-size

//                 // Calculate position and size
//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 Vector3 position = new Vector3(
//                     duration / 2f,                  // center the cube along X
//                     track * trackSpacing,           // Y: separate rows by track
//                     pitch                           // Z: pitch
//                 );

//                 // Create and place cube
//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;

//                 // Ensure queue for track
//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 // Maintain most recent 10 cubes
//                 trackCubes[track].Enqueue(cube);
//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }
// }

////////////// has cube generation but wrong visual ////////////// ////////////// ////////////// 
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl : MonoBehaviour
// {
//     private MidiFilePlayer midiFilePlayer;
//     private Dictionary<int, List<GameObject>> trackCubes = new Dictionary<int, List<GameObject>>();
//     private const int MaxTicks = 10;

//     void Start()
//     {
//         midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void VisualizeNotes(List<MPTKEvent> events)
//     {
//         foreach (MPTKEvent note in events)
//         {
//             if (note.Command == MPTKCommand.NoteOn)
//             {
//                 int track = (int)note.Track;
//                 float pitch = (float)note.Value;      // for height
//                 float duration = (float)note.Duration / 100f; // for width
//                 float velocity = (float)note.Velocity / 10f;  // for depth

//                 Vector3 cubeSize = new Vector3(duration, pitch / 10f, velocity);
//                 Vector3 position = new Vector3(duration / 2f, pitch / 20f, velocity / 2f) + new Vector3(track * 5, 0, 0); // shift by track

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = cubeSize;
//                 cube.transform.position = position;

//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new List<GameObject>();

//                 trackCubes[track].Add(cube);

//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject toRemove = trackCubes[track][0];
//                     trackCubes[track].RemoveAt(0);
//                     Destroy(toRemove);
//                 }
//             }
//         }
//     }
// }


