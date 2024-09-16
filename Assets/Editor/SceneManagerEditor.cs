using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneManager))]
public class SceneManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Reference to the target script
        SceneManager sceneManager = (SceneManager)target;

        // Add a button in the inspector
        if (GUILayout.Button("Load Scene"))
        {
            sceneManager.LoadScene(1);
        }
    }
}
