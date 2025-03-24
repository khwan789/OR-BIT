#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PooledObject))]
public class PooledObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Begin property
        EditorGUI.BeginProperty(position, label, property);

        // Get the SerializedProperties for `prefab`, `weight`, and `poolSize`
        SerializedProperty prefabProp = property.FindPropertyRelative("prefab");
        SerializedProperty weightProp = property.FindPropertyRelative("weight");
        SerializedProperty poolSizeProp = property.FindPropertyRelative("poolSize");

        // Calculate widths
        float spacing = 5f;
        float prefabWidth = position.width * 0.4f;  // 40% for Prefab
        float weightWidth = position.width * 0.25f; // 25% for Weight
        float poolSizeWidth = position.width * 0.25f; // 25% for Pool Size

        // Draw fields side by side with proper spacing
        Rect prefabRect = new Rect(position.x, position.y, prefabWidth, position.height);
        Rect weightRect = new Rect(position.x + prefabWidth + spacing, position.y, weightWidth, position.height);
        Rect poolSizeRect = new Rect(position.x + prefabWidth + weightWidth + (spacing * 2), position.y, poolSizeWidth, position.height);

        EditorGUI.PropertyField(prefabRect, prefabProp, GUIContent.none);
        EditorGUI.PropertyField(weightRect, weightProp, GUIContent.none);
        EditorGUI.PropertyField(poolSizeRect, poolSizeProp, GUIContent.none);

        // End property
        EditorGUI.EndProperty();
    }
}
#endif