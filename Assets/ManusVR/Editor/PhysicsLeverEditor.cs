using Assets.ManusVR.Scripts.ManusInterface;
using UnityEditor;
using UnityEngine;

namespace Assets.ManusVR.VRToolkit
{
    [CustomEditor(typeof(PhysicsLever)), CanEditMultipleObjects]
    public class PhysicsLeverEditor : UnityEditor.Editor
    {
        private SerializedProperty initialValue;
        private SerializedProperty minMaxValue;

        void OnEnable()
        {
            initialValue = serializedObject.FindProperty("_initialValue");
            minMaxValue = serializedObject.FindProperty("MinMaxValue");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();
            //EditorGUILayout.PropertyField(heightLimits);
            EditorGUILayout.Slider(initialValue, minMaxValue.vector2Value.x, minMaxValue.vector2Value.y, "Initial Value");
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }
    }
}