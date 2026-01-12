using UnityEngine;
using UnityEditor;
using Feature1;

[CustomEditor(typeof(StarGenerator))]
public class StarGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        StarGenerator generator = (StarGenerator)target;
        
        EditorGUILayout.Space();
        
        // Bouton pour générer les étoiles
        if (GUILayout.Button("Générer les étoiles", GUILayout.Height(30)))
        {
            generator.GenerateStars();
        }
    }
}

