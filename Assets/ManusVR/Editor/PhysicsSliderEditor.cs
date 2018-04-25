using Assets.ManusVR.Scripts.ManusInterface;
using UnityEditor;
using UnityEngine;

namespace Assets.ManusVR.VRToolkit
{
    [CustomEditor(typeof(PhysicsSlider)), CanEditMultipleObjects]
    public class PhysicsSliderEditor : UnityEditor.Editor
    {
        private SerializedProperty initialValue;
        private SerializedProperty minMaxValue;
        private SerializedProperty minMaxMovement;
        private SerializedProperty valueChangedEvent;
        private SerializedProperty movementMin, movementMax;

        void OnEnable()
        {
            initialValue = serializedObject.FindProperty("InitialValue");
            minMaxValue = serializedObject.FindProperty("MinMaxValue");

            valueChangedEvent = serializedObject.FindProperty("ValueChangedEvent");
            minMaxMovement = serializedObject.FindProperty("MinMaxMovement");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();

            //EditorGUILayout.PropertyField(heightLimits);
            EditorGUILayout.PropertyField(valueChangedEvent);
            EditorGUILayout.PropertyField(minMaxValue);
            EditorGUILayout.Slider(initialValue, minMaxValue.vector2Value.x, minMaxValue.vector2Value.y, "Initial Value");
            EditorGUILayout.PropertyField(minMaxMovement);
            //movementLimit.vector2Value = EditorGUILayout.Vector2Field("Min and Max Z value for slider handle", movementLimit.vector2Value);
            serializedObject.ApplyModifiedProperties();
        }
    }
}