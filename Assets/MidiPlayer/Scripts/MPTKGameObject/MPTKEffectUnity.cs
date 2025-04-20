using System;
using UnityEngine;

namespace MidiPlayerTK
{
    /// <summary>
    /// Unlike SoundFont effects, they applied to the whole player. On the other hand, the Unity effects parameters are rich and, obviously based on Uniy algo!\n
    /// https://docs.unity3d.com/Manual/class-AudioEffectMixer.html\n
    /// Only most important effect are integrated in Maestro: Reverb and Chorus. On need, others effects could be added. 
    /// @version Maestro Pro 
    /// @note
    ///     - Unity effects integration modules are exclusively available with the Maestro MPTK Pro version. 
    ///     - By default, these effects are disabled in Maestro. 
    ///     - To enable them, you’ll need to adjust the settings from the prefab inspector: Synth Parameters / Unity Effect.
    ///     - Each settings are available by script.
    /// @code
    /// // Find a MPTK Prefab, will works also for MidiStreamPlayer, MidiExternalPlayer ... all classes which inherit from MidiSynth.
    /// MidiFilePlayer fp = FindFirstObjectByType<MidiFilePlayer>();
    /// fp.MPTK_EffectUnity.EnableReverb = true;
    /// fp.MPTK_EffectUnity.ReverbDelay= 0.09f;
    /// @endcode
    /// </summary>
    public partial class MPTKEffectUnity : ScriptableObject
    {

    }
}
