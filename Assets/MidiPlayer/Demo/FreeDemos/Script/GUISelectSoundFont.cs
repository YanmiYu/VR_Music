//#define MPTK_PRO
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiPlayerTK;
using System;

namespace DemoMPTK
{
    public class GUISelectSoundFont : MonoBehaviour
    {
        static public List<MPTKListItem> SoundFonts = null;
        static private PopupListItem PopSoundFont;
        static private int selectedSf;

        static private void SoundFontChanged(object tag, int midiindex, int indexList)
        {
#if MPTK_PRO
            Debug.Log("SoundFontChanged " + midiindex);
            MidiPlayerGlobal.MPTK_SelectSoundFont(MidiPlayerGlobal.MPTK_ListSoundFont[midiindex]);
            selectedSf = midiindex;
#else
            Debug.Log("Can't change of SoundFont with Free version of MPTK");
#endif

            // return true;
        }

        static public void Display(Vector2 scrollerWindow, CustomStyle myStyle, float width)
        {
            SoundFonts = new List<MPTKListItem>();
            if (MidiPlayerGlobal.MPTK_ListSoundFont == null) return;
            foreach (string name in MidiPlayerGlobal.MPTK_ListSoundFont)
            {
                if (MidiPlayerGlobal.CurrentMidiSet != null && MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo != null && name == MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.Name)
                    selectedSf = SoundFonts.Count;
                SoundFonts.Add(new MPTKListItem() { Index = SoundFonts.Count, Label = name });
            }

            if (PopSoundFont == null)
                PopSoundFont = new PopupListItem()
                {
                    Title = "Select A SoundFont",
                    OnSelect = SoundFontChanged,
                    ColCount = 1,
                    ColWidth = 500,
                };

            if (SoundFonts != null)
            {
                PopSoundFont.Draw(SoundFonts, selectedSf, myStyle);
                GUILayout.BeginHorizontal(myStyle.BacgDemosMedium, GUILayout.Width(width));
                //GUILayout.Space(20);
                float height = 30;
                if (MidiPlayerGlobal.ImSFCurrent != null)
                {
                    if (MidiPlayerGlobal.ImSFCurrent.LiveSF)
                        GUILayout.Label("Live SoundFont: " + MidiPlayerGlobal.ImSFCurrent.SoundFontName, myStyle.TitleLabel2, GUILayout.Height(height));
                    else
                    {
                        GUILayout.Label($"Select a SoundFont:", myStyle.TitleLabel3, GUILayout.Width(80));
                        if (GUILayout.Button(MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.Name, GUILayout.Width(300),  GUILayout.Height(height)))
                            PopSoundFont.Show = !PopSoundFont.Show;
                    }
                    GUILayout.Space(10);
                    GUILayout.Label(string.Format("Load Time:{0} s    Samples:{1} s    Count Presets:{2}   Samples:{3}",
                        Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadSoundFont.TotalSeconds, 3),
                        Math.Round(MidiPlayerGlobal.MPTK_TimeToLoadWave.TotalSeconds, 3),
                        MidiPlayerGlobal.MPTK_CountPresetLoaded,
                        MidiPlayerGlobal.MPTK_CountWaveLoaded),
                        myStyle.TitleLabel3, GUILayout.Height(height));
                }
                else
                    GUILayout.Label("No SoundFont loaded", myStyle.TitleLabel2, GUILayout.Height(height));

                GUILayout.EndHorizontal();

                PopSoundFont.PositionWithScroll(ref scrollerWindow);
            }
            else
            {
                GUILayout.Label("No SoundFont found");
            }
        }
    }
}
