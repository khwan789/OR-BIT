#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetObjectDistributor))]
public class PlanetObjectDistributorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Reference to the script
        PlanetObjectDistributor distributor = (PlanetObjectDistributor)target;

        // Draw the default inspector
        DrawDefaultInspector();

        // Add a button to the inspector
        if (GUILayout.Button("Place Objects"))
        {
            distributor.PlaceObjects();
            EditorUtility.SetDirty(distributor); // Mark object as changed
            SceneView.RepaintAll(); // Force scene view to refresh
        }
    }
}
#endif
