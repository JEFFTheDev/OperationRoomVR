using Assets.ManusVR.Scripts.Extra;
using UnityEngine;

[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyAttributeDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect rect, UnityEditor.SerializedProperty prop, GUIContent label)
    {
        GUI.enabled = false;
        UnityEditor.EditorGUI.PropertyField(rect, prop);
        GUI.enabled = true;
    }
}