using Assets.ManusVR.Scripts.ManusInterface;
using UnityEditor;
using UnityEngine;

namespace Assets.ManusVR.VRToolkit
{
    [CustomEditor(typeof(PhysicsButton)), CanEditMultipleObjects]
    public class PhysicsButtonEditor : UnityEditor.Editor
    {
        private SerializedProperty heightLimits;
        private SerializedProperty triggerHeight;

        void OnEnable()
        {
            heightLimits = serializedObject.FindProperty("HeightLimits");
            triggerHeight = serializedObject.FindProperty("TriggerDistance");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            //EditorGUILayout.PropertyField(heightLimits);
            heightLimits.vector2Value = EditorGUILayout.Vector2Field("MinMaxHeight", heightLimits.vector2Value);
            serializedObject.ApplyModifiedProperties();
        }
    }
}