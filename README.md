# 🎹 VR Music Visualization with MIDI + Maestro MPTK

This Unity project is a VR/3D music visualization tool that converts MIDI files into real-time moving blocks in 3D space. It is built using the Unity Starter Samples and enhanced with Maestro MPTK for MIDI playback.

Each note is visualized as a cube whose position, size, and color reflect the musical characteristics of pitch, duration, and velocity. The user can interact with the whole visualization using keyboard or UI-based movement and rotation controls.

## 🔧 Built On
- ✅ **Unity Starter Samples** – as the foundation
- ✅ **Maestro MPTK (MIDI Player Toolkit)** – to parse and play MIDI in Unity
  - *Getting Started Tip*: See [Maestro MPTK Setup Guide](https://www.vrwiki.cs.brown.edu/vr-development-software/development-tutorials/unity3d-tutorial/midi-data-visualization-using-maestro-midi-player-tool-kit)

## ✨ Features
- 🔷 **Dynamic 3D cube generation** based on MIDI notes:
  - Pitch → Z-position + Color
  - Velocity → Y-scale (height)
  - Duration → X-scale (width)
- 📦 **Efficient queueing system** to keep only the most recent blocks (max 10 per track)
- 🧭 **Interactive 3D Navigation Controls**:
  - Move and rotate the entire visualization using simple function calls
- 🎵 **Song Selection System**:
  - Use building blocks to change songs (e.g., Button A for MidiFilePlayer1, Button B for MidiFilePlayer2)

## 🕹️ Movement & Rotation Controls
You can bind these functions to UI buttons or keyboard shortcuts:

| Function        | Action                                  |
|-----------------|-----------------------------------------|
| `MoveUp()`      | Move the visualization upward          |
| `MoveDown()`    | Move it downward                       |
| `MoveLeft()`    | Move it left                           |
| `MoveRight()`   | Move it right                          |
| `MoveForward()` | Move it forward (Z+)                   |
| `MoveBackward()`| Move it backward (Z−)                  |
| `RotateLeft()`  | Rotate the visualization left          |
| `RotateRight()` | Rotate the visualization right         |

## 📁 How to Use
1. Clone this repo:
   ```bash
   git clone https://github.com/YanmiYu/VR_Music.git
2. Open in Unity (Unity 2021.3+ recommended)
3. Import Maestro MPTK from the Unity Asset Store
4. In the scene:
- Attach the ExampleMidi1 script to an empty GameObject
- Assign the MidiFilePlayer (where you select the MIDI file/music)
- Assign the MidiPlay component (handles the visualization)
- (Optional) Assign a cubeParent to group all generated cubes for transformation
- Note: All scripts can be found in Assets/Scripts

## 🧠 Mapping Summary

MIDI Parameter	Mapped To
note.Value	Z-position (pitch) + color
note.Velocity	Y-scale (volume/velocity)
note.Duration	X-scale (note duration)

## 🧹 Utility Functions

Restart() – Stops and restarts the MIDI playback and visualization
ClearAllCubes() – Clears all existing note cubes from the scene

## 🪪 License

MIT License. See LICENSE for details.
