using Assets.ManusVR.Scripts.PhysicalInteraction;
using UnityEditor;

namespace Assets.ManusVR.Editor
{
    [CustomEditor(typeof(SnappableObject), true), CanEditMultipleObjects]
    public class SnappableObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty scaleBehaviour;
        private SerializedProperty customScale;
        
        void OnEnable()
        {
            scaleBehaviour = serializedObject.FindProperty("ScaleBehaviour");
            customScale = serializedObject.FindProperty("_customScale");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            if (scaleBehaviour.enumValueIndex == 2)
            {
                EditorGUILayout.PropertyField(customScale);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}