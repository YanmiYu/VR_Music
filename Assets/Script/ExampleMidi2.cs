using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;

public class ExampleMidi2 : MonoBehaviour
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

