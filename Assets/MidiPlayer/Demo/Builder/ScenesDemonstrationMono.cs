using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace MidiPlayerTK
{
    public class ScenesDemonstrationMono : MonoBehaviour
    {
        private Demonstrator loadedDemos;

        void Start()
        {
            loadedDemos = new Demonstrator();
            loadedDemos.LoadCSV();

            // load the Assets/MidiPlayer/Demo/Builder/Resources/ScenesDemonstration.uxml
            // defined in the componant "UI Document" added to SceneHandler gameObject
            loadedDemos.Root = GetComponent<UIDocument>().rootVisualElement;

            // SceneHandler gameObject can hold only one componant "UI Document"
            // Nedd to load "manually" the template uxml
            loadedDemos.RowTemplate = Resources.Load<VisualTreeAsset>("OneRowDemo");

            /// Find all visual element and apply content or enable callback.
            /// Works the same for Editor and Run mode.
            loadedDemos.FindVisualComponent();


            int index = 1;
            foreach (Demonstrator demo in loadedDemos.Demos)
            {
                // Take only scene defined in builder settings
                if (index == 1 || SceneUtility.GetBuildIndexByScenePath(demo.SceneName) >= 0)
                    loadedDemos.AddRow(demo, index++, 0);
            }
        }
    }
}
