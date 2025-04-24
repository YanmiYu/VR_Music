using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class ExampleMidi1 : MonoBehaviour
{
    public MidiFilePlayer midiFilePlayer;
    private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
    private const int MaxTicks = 10;
    private float trackSpacing = 5f; // Y-axis spacing between tracks
    private float moveSpeed = 2.0f;  // 移动速度

    public float moveStep = 2.0f; 
    public float rotateStep = 15.0f; // Rotation amount in degrees

    public Transform cubeParent; 

    void Start()
    {
        midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
        midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
        
        // Initialize cubeParent if it's not set
        if (cubeParent == null)
        {
            GameObject parent = new GameObject("CubeParent");
            parent.transform.parent = this.transform;
            cubeParent = parent.transform;
        }
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
                
                // Create position in local space (relative to cubeParent)
                Vector3 localPosition = new Vector3(
                    0f,                             // 新立方体从起点开始
                    track * trackSpacing,           // Y row for the track
                    pitch                           // Z for pitch
                );

                // Create cube
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                
                // Parent the cube to cubeParent instead of this.transform
                cube.transform.parent = cubeParent;
                cube.transform.localRotation = Quaternion.identity; // Reset rotation
                
                cube.transform.localScale = size;
                
                // Set the local position instead of world position
                // This will automatically account for parent's rotation
                
                cube.transform.localPosition = cubeParent.position + localPosition;

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

    public void Restart()
    {
        if (midiFilePlayer != null)
        {
            midiFilePlayer.MPTK_Stop();
        }

        ClearAllCubes();

        if (midiFilePlayer != null)
        {
            midiFilePlayer.MPTK_Play();
        }
    }

    public void ClearAllCubes()
    {
        foreach (var trackPair in trackCubes)
        {
            foreach (GameObject cube in trackPair.Value)
            {
                Destroy(cube);
            }
        }
        trackCubes.Clear();
        trackXOffsets.Clear();
    }
    // ✅ Movement Controls — Call from UI Buttons
    public void MoveUp() => MoveVisualization(Vector3.up * moveStep);
    public void MoveDown() => MoveVisualization(Vector3.down * moveStep);
    public void MoveLeft() => MoveVisualization(Vector3.left * moveStep);
    public void MoveRight() => MoveVisualization(Vector3.right * moveStep);
    
    // ✅ New Forward/Backward Movement Controls
    public void MoveForward() => MoveVisualization(Vector3.forward * moveStep);
    public void MoveBackward() => MoveVisualization(Vector3.back * moveStep);
    
    // ✅ New Rotation Controls
    public void RotateLeft() => RotateVisualization(-rotateStep);
    public void RotateRight() => RotateVisualization(rotateStep);

    public void MoveVisualization(Vector3 direction)
    {
        if (cubeParent != null)
        {
            cubeParent.position += direction;
        }
    }
    
    public void RotateVisualization(float angle)
    {
        if (cubeParent != null)
        {
            cubeParent.Rotate(Vector3.up, angle);
        }
    }
    
}

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl1 : MonoBehaviour
// {
//     [Header("Maestro MPTK Player")]
//     public MidiFilePlayer midiFilePlayer; // ✅ make public so you can assign it via Inspector

//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f; // Y-axis spacing between tracks
//     private float moveSpeed = 2.0f;  // 

//     void Start()
//     {
//         if (midiFilePlayer == null)
//             midiFilePlayer = FindObjectOfType<MidiFilePlayer>();

//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Vector3 pos = cube.transform.position;
//                 pos.x += moveSpeed * Time.deltaTime;
//                 cube.transform.position = pos;
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

//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 float zOffset = -15f;
//                 Vector3 position = new Vector3(
//                     0f,
//                     track * trackSpacing,
//                     pitch + zOffset
//                 );

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;
//                 cube.transform.parent = this.transform;

//                 CubeInfo cubeInfo = cube.AddComponent<CubeInfo>() as CubeInfo;
//                 cubeInfo.pitch = pitch.ToString();
//                 cubeInfo.duration = duration.ToString();
//                 cubeInfo.volume = velocity.ToString();

//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 trackCubes[track].Enqueue(cube);

//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }

    
    
//     public void Restart()
//     {
//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Stop();
//         }

//         ClearAllCubes();

//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Play();
//         }
//     }

//     public void ClearAllCubes()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Destroy(cube);
//             }
//         }
//         trackCubes.Clear();
//         trackXOffsets.Clear();
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl1 : MonoBehaviour
// {
//     [Header("Maestro MPTK Player")]
//     public MidiFilePlayer midiFilePlayer; // ✅ make public so you can assign it via Inspector

//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f; // Y-axis spacing between tracks
//     private float moveSpeed = 2.0f;  // 

//     void Start()
//     {
//         if (midiFilePlayer == null)
//             midiFilePlayer = FindObjectOfType<MidiFilePlayer>();

//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Vector3 pos = cube.transform.position;
//                 pos.x += moveSpeed * Time.deltaTime;
//                 cube.transform.position = pos;
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

//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 float zOffset = -15f;
//                 Vector3 position = new Vector3(
//                     0f,
//                     track * trackSpacing,
//                     pitch + zOffset
//                 );

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;
//                 cube.transform.parent = this.transform;

//                 CubeInfo cubeInfo = cube.AddComponent<CubeInfo>() as CubeInfo;
//                 cubeInfo.pitch = pitch.ToString();
//                 cubeInfo.duration = duration.ToString();
//                 cubeInfo.volume = velocity.ToString();

//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 trackCubes[track].Enqueue(cube);

//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }

    
    
//     public void Restart()
//     {
//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Stop();
//         }

//         ClearAllCubes();

//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Play();
//         }
//     }

//     public void ClearAllCubes()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Destroy(cube);
//             }
//         }
//         trackCubes.Clear();
//         trackXOffsets.Clear();
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl1 : MonoBehaviour
// {
//     [Header("Maestro MPTK Player")]
//     public MidiFilePlayer midiFilePlayer; // ✅ make public so you can assign it via Inspector

//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f; // Y-axis spacing between tracks
//     private float moveSpeed = 2.0f;  // 

//     void Start()
//     {
//         if (midiFilePlayer == null)
//             midiFilePlayer = FindObjectOfType<MidiFilePlayer>();

//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Vector3 pos = cube.transform.position;
//                 pos.x += moveSpeed * Time.deltaTime;
//                 cube.transform.position = pos;
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

//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 float zOffset = -15f;
//                 Vector3 position = new Vector3(
//                     0f,
//                     track * trackSpacing,
//                     pitch + zOffset
//                 );

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;
//                 cube.transform.parent = this.transform;

//                 CubeInfo cubeInfo = cube.AddComponent<CubeInfo>() as CubeInfo;
//                 cubeInfo.pitch = pitch.ToString();
//                 cubeInfo.duration = duration.ToString();
//                 cubeInfo.volume = velocity.ToString();

//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 trackCubes[track].Enqueue(cube);

//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }

    
    
//     public void Restart()
//     {
//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Stop();
//         }

//         ClearAllCubes();

//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Play();
//         }
//     }

//     public void ClearAllCubes()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Destroy(cube);
//             }
//         }
//         trackCubes.Clear();
//         trackXOffsets.Clear();
//     }
// }




// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl1 : MonoBehaviour
// {
//     [Header("Maestro MPTK Player")]
//     public MidiFilePlayer midiFilePlayer;
    
//     [Header("Movement Controls")]
//     public float moveStep = 1f; // How much to move with each button press
//     public float moveSpeed = 2.0f; // Movement speed for automatic scrolling

//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>();
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f;
    
//     // Track the overall visualization offset
//     private Vector3 visualizationOffset = Vector3.zero;

//     void Start()
//     {
//         if (midiFilePlayer == null)
//             midiFilePlayer = FindObjectOfType<MidiFilePlayer>();

//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         // Move all cubes based on the automatic scrolling and manual offset
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Vector3 pos = cube.transform.position;
//                 pos.x += moveSpeed * Time.deltaTime; // Automatic scrolling
//                 pos += visualizationOffset; // Apply manual offset
//                 cube.transform.position = pos;
//             }
//         }
        
//         // Reset the offset after applying it (for button presses)
//         visualizationOffset = Vector3.zero;
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

//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 float zOffset = -15f;
//                 Vector3 position = new Vector3(
//                     0f,
//                     track * trackSpacing,
//                     pitch + zOffset
//                 );

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;
//                 cube.transform.parent = this.transform;

//                 CubeInfo cubeInfo = cube.AddComponent<CubeInfo>() as CubeInfo;
//                 cubeInfo.pitch = pitch.ToString();
//                 cubeInfo.duration = duration.ToString();
//                 cubeInfo.volume = velocity.ToString();

//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 trackCubes[track].Enqueue(cube);

//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }

//     // Movement control methods
//     public void MoveUp()
//     {
//         visualizationOffset.y += moveStep;
//     }

//     public void MoveDown()
//     {
//         visualizationOffset.y -= moveStep;
//     }

//     public void MoveLeft()
//     {
//         visualizationOffset.x -= moveStep;
//     }

//     public void MoveRight()
//     {
//         visualizationOffset.x += moveStep;
//     }

//     public void Restart()
//     {
//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Stop();
//         }

//         ClearAllCubes();

//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Play();
//         }
//     }

//     public void ClearAllCubes()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Destroy(cube);
//             }
//         }
//         trackCubes.Clear();
//         trackXOffsets.Clear();
//     }
// }


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl1 : MonoBehaviour
// {
//     [Header("Maestro MPTK Player")]
//     public MidiFilePlayer midiFilePlayer;

//     [Header("Cube Parent")]
//     public Transform cubeParent; // 🟡 Assign this in Inspector to an empty GameObject (e.g. CubeContainer)

//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>();
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f;
//     private float moveSpeed = 2.0f;

//     [Header("Move Step Size")]
//     public float moveStep = 1.0f; // 🟡 How far to move when arrow button is pressed

//     void Start()
//     {
//         if (midiFilePlayer == null)
//             midiFilePlayer = FindObjectOfType<MidiFilePlayer>();

//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Vector3 pos = cube.transform.position;
//                 pos.x += moveSpeed * Time.deltaTime;
//                 cube.transform.position = pos;
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

//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 float zOffset = -15f;
//                 Vector3 position = new Vector3(
//                     0f,
//                     track * trackSpacing,
//                     pitch + zOffset
//                 );

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;

//                 // ✅ Set parent to shared container
//                 if (cubeParent != null)
//                     cube.transform.parent = cubeParent;
//                 else
//                     cube.transform.parent = this.transform;

//                 CubeInfo cubeInfo = cube.AddComponent<CubeInfo>() as CubeInfo;
//                 cubeInfo.pitch = pitch.ToString();
//                 cubeInfo.duration = duration.ToString();
//                 cubeInfo.volume = velocity.ToString();

//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 trackCubes[track].Enqueue(cube);

//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }

//     // ✅ Restart visualization and MIDI
//     public void Restart()
//     {
//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Stop();
//         }

//         ClearAllCubes();

//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Play();
//         }
//     }

//     // ✅ Clear all existing cubes
//     public void ClearAllCubes()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Destroy(cube);
//             }
//         }
//         trackCubes.Clear();
//         trackXOffsets.Clear();
//     }

    // // ✅ Movement Controls — Call from UI Buttons
    // public void MoveUp() => MoveVisualization(Vector3.up * moveStep);
    // public void MoveDown() => MoveVisualization(Vector3.down * moveStep);
    // public void MoveLeft() => MoveVisualization(Vector3.left * moveStep);
    // public void MoveRight() => MoveVisualization(Vector3.right * moveStep);

    // public void MoveVisualization(Vector3 direction)
    // {
    //     if (cubeParent != null)
    //     {
    //         cubeParent.position += direction;
    //     }
    // }
// }


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using MidiPlayerTK;

// public class ExampleMidiControl1 : MonoBehaviour
// {
//     [Header("Maestro MPTK Player")]
//     public MidiFilePlayer midiFilePlayer; // ✅ make public so you can assign it via Inspector

//     private Dictionary<int, Queue<GameObject>> trackCubes = new Dictionary<int, Queue<GameObject>>();
//     private Dictionary<int, float> trackXOffsets = new Dictionary<int, float>(); // track -> x position offset
//     private const int MaxTicks = 10;
//     private float trackSpacing = 5f; // Y-axis spacing between tracks
//     private float moveSpeed = 2.0f;  // 

//     void Start()
//     {
//         if (midiFilePlayer == null)
//             midiFilePlayer = FindObjectOfType<MidiFilePlayer>();

//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Vector3 pos = cube.transform.position;
//                 pos.x += moveSpeed * Time.deltaTime;
//                 cube.transform.position = pos;
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

//                 if (!trackXOffsets.ContainsKey(track))
//                     trackXOffsets[track] = 0f;

//                 if (!trackCubes.ContainsKey(track))
//                     trackCubes[track] = new Queue<GameObject>();

//                 Vector3 size = new Vector3(duration, velocity, 1f);
//                 float zOffset = -15f;
//                 Vector3 position = new Vector3(
//                     0f,
//                     track * trackSpacing,
//                     pitch + zOffset
//                 );

//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;
//                 cube.transform.parent = this.transform;

//                 CubeInfo cubeInfo = cube.AddComponent<CubeInfo>() as CubeInfo;
//                 cubeInfo.pitch = pitch.ToString();
//                 cubeInfo.duration = duration.ToString();
//                 cubeInfo.volume = velocity.ToString();

//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 trackCubes[track].Enqueue(cube);

//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }

    
    
//     public void Restart()
//     {
//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Stop();
//         }

//         ClearAllCubes();

//         if (midiFilePlayer != null)
//         {
//             midiFilePlayer.MPTK_Play();
//         }
//     }

//     public void ClearAllCubes()
//     {
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Destroy(cube);
//             }
//         }
//         trackCubes.Clear();
//         trackXOffsets.Clear();
//     }
// }


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
//     private float moveSpeed = 2.0f;  // 移动速度

//     void Start()
//     {
//         midiFilePlayer = FindObjectOfType<MidiFilePlayer>();
//         midiFilePlayer.OnEventNotesMidi.AddListener(VisualizeNotes);
//     }

//     void Update()
//     {
//         // 在Update中移动所有立方体
//         foreach (var trackPair in trackCubes)
//         {
//             foreach (GameObject cube in trackPair.Value)
//             {
//                 Vector3 pos = cube.transform.position;
//                 pos.x += moveSpeed * Time.deltaTime; // 使用Time.deltaTime使移动与帧率无关
//                 cube.transform.position = pos;
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
//                     0f,                             // 新立方体从起点开始
//                     track * trackSpacing,           // Y row for the track
//                     pitch                           // Z for pitch
//                 );

//                 // Create cube
//                 GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                 cube.transform.localScale = size;
//                 cube.transform.position = position;

//                 // Color by pitch (optional visual enhancement)
//                 float hue = Mathf.InverseLerp(21f, 108f, pitch);
//                 cube.GetComponent<Renderer>().material.color = Color.HSVToRGB(hue, 1f, 1f);

//                 // Add to queue
//                 trackCubes[track].Enqueue(cube);
                
//                 // 仍然保持最多10个立方体的限制
//                 if (trackCubes[track].Count > MaxTicks)
//                 {
//                     GameObject oldCube = trackCubes[track].Dequeue();
//                     Destroy(oldCube);
//                 }
//             }
//         }
//     }
// }


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


