//#define MPTK_PRO
using System;
using UnityEngine;

namespace MidiPlayerTK
{
    /// <summary>
    /// A SoundFont contains parameters to apply three kinds of effects: low-pass filter, reverb, chorus.\n
    /// These parameters can be specifics for each instruments and even each voices.\n
    /// Maestro MPTK effects are based on FluidSynth algo effects modules. 
    /// Furthermore, to get more liberty from SoundFont, Maestro can increase or decrease the impact of effects (from the inspector or by script).
    /// To summarize:
    ///     - Effects are applied individually to each voices, yet they are statically defined within the SoundFont.
    ///     - Maestro parameters can be adjusted to increase or decrease the default values set in the SoundFont.
    ///     - These adjustments will be applied across the entire prefab, but the effect will depend on the initial settings defined in the SoundFont preset.
    ///     - Please note that these effects require additional CPU resources.
    /// See more detailed information here https://paxstellar.fr/sound-effects/
    /// @version Maestro Pro 
    /// @note
    ///     - Effects modules are exclusively available with the Maestro MPTK Pro version. 
    ///     - By default, these effects are disabled in Maestro. 
    ///     - To enable them, you’ll need to adjust the settings from the prefab inspector (Synth Parameters / SoundFont Effect) or by script!
    ///     - For enhanced sound quality, it’s often beneficial to add a low-filter effect.
    /// @code
    /// // Find a MPTK Prefab, will works also for MidiStreamPlayer, MidiExternalPlayer ... all classes which inherit from MidiSynth.
    /// MidiFilePlayer fp = FindFirstObjectByType<MidiFilePlayer>();
    /// fp.MPTK_EffectSoundFont.EnableFilter = true;
    /// fp.MPTK_EffectSoundFont.FilterFreqOffset = 500;
    /// @endcode
    /// </summary>
    public partial class MPTKEffectSoundFont : ScriptableObject
    {
        /// <summary>@brief
        /// Apply frequency low-pass filter as defined in the SoundFont.\n 
        /// This effect is processed with the fluidsynth algo independently on each voices but with a decrease of performace.
        /// @version Maestro Pro 
        /// @code
        /// midiFilePlayer.MPTK_EffectSoundFont.EnableFilter = true;
        /// @endcode
        /// </summary>
        public bool EnableFilter { get => applySFFilter; set => applySFFilter = value; }

        /// <summary>
        /// Apply reverberation effect as defined in the SoundFont.\n
        /// This effect is processed with the fluidsynth algo independently on each voices but with a decrease of performace. 
        /// @version Maestro Pro 
        /// @code
        /// midiFilePlayer.MPTK_EffectSoundFont.EnableReverb = true;
        /// @endcode
        /// </summary>
        public bool EnableReverb { get => applySFReverb; set => applySFReverb = value; }

        /// <summary>
        /// Apply chorus effect as defined in the SoundFont.\n
        /// This effect is processed with the fluidsynth algo independently on each voices but with a small decrease of performace. 
        /// @version Maestro Pro 
        /// @code
        /// midiFilePlayer.MPTK_EffectSoundFont.EnableChorus = true;
        /// @endcode
        /// </summary>
        public bool EnableChorus { get => applySFChorus; set => applySFChorus = value; }

        [HideInInspector, SerializeField]
        private bool applySFReverb, applySFChorus, applySFFilter = true;

        //! @cond NODOC

        private MidiSynth synth;

        public void Init(MidiSynth psynth)
        {
            synth = psynth;
            ///* Effects audio buffers */
            /* allocate the reverb module */
            fx_reverb = new float[psynth.FLUID_BUFSIZE];  // FLUID_MAX_BUFSIZE not supported, each frame must have the same length
            reverb = new fluid_revmodel(psynth.OutputRate, psynth.FLUID_BUFSIZE); // FLUID_MAX_BUFSIZE not supported, each frame must have the same length

            fx_chorus = new float[psynth.FLUID_BUFSIZE]; // FLUID_MAX_BUFSIZE not supported, each frame must have the same length
            /* allocate the chorus module */
            chorus = new fluid_chorus(psynth.OutputRate, psynth.FLUID_BUFSIZE); // FLUID_MAX_BUFSIZE not supported, each frame must have the same length
#if MPTK_PRO
            SetParamSfReverb();
            SetParamSfChorus();
#else
            reverb.fluid_revmodel_set(0xFF,
                MidiSynth.FLUID_REVERB_DEFAULT_ROOMSIZE, MidiSynth.FLUID_REVERB_DEFAULT_DAMP, 
                MidiSynth.FLUID_REVERB_DEFAULT_WIDTH, MidiSynth.FLUID_REVERB_DEFAULT_LEVEL);
            chorus.fluid_chorus_set((int)fluid_chorus.fluid_chorus_set_t.FLUID_CHORUS_SET_ALL,
                MidiSynth.FLUID_CHORUS_DEFAULT_N, MidiSynth.FLUID_CHORUS_DEFAULT_LEVEL,
                MidiSynth.FLUID_CHORUS_DEFAULT_SPEED, MidiSynth.FLUID_CHORUS_DEFAULT_DEPTH, 
                fluid_chorus.FLUID_CHORUS_DEFAULT_TYPE, MidiSynth.FLUID_CHORUS_DEFAULT_WIDTH);
#endif
        }

        fluid_revmodel reverb;
        private float[] fx_reverb;
        fluid_chorus chorus;
        private float[] fx_chorus;


        public void PrepareBufferEffect(out float[] reverb_buf, out float[] chorus_buf)
        {
            // Set up the reverb / chorus buffers only, when the effect is enabled on synth level.
            // Nonexisting buffers are detected in theDSP loop. 
            // Not sending the reverb / chorus signal saves some time in that case.
            if (EnableReverb)
            {
                Array.Clear(fx_reverb, 0, synth.FLUID_BUFSIZE);
                reverb_buf = fx_reverb;
            }
            else
                reverb_buf = null;

            if (EnableChorus)
            {
                Array.Clear(fx_chorus, 0, synth.FLUID_BUFSIZE);
                chorus_buf = fx_chorus;
            }
            else
                chorus_buf = null;
        }

        public void ProcessEffect(float[] reverb_buf, float[] chorus_buf, float[] left_buf, float[] right_buf)
        {
            /* send to reverb */
            if (EnableReverb && reverb_buf != null)
            {
                reverb.fluid_revmodel_processmix(reverb_buf, left_buf, right_buf);
            }

            /* send to chorus */
            if (EnableChorus && chorus_buf != null)
            {
                chorus.fluid_chorus_processmix(chorus_buf, left_buf, right_buf);
            }
        }

        //! @endcond

    }
}
