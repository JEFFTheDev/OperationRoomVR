using Assets.ManusVR.Scripts.PhysicalInteraction;
using UnityEditor;

namespace ManusVR.PhysicalInteraction
{
    [CustomEditor(typeof(Interactable), true), CanEditMultipleObjects]
    public class InteractableEditor : Editor
    {
        private SerializedProperty _highlightOnImpact;
        private SerializedProperty _isGrabbable;
        private SerializedProperty _highlightWhenGrabbed;
        private SerializedProperty _dropDistance;
        private SerializedProperty _gravityWhenGrabbed;
        private SerializedProperty _gravityWhenReleased;
        private SerializedProperty _kinematicWhenReleased;
        private SerializedProperty _attachHandToItem;
        private SerializedProperty _rotateWithHand;

        void OnEnable()
        {
            _highlightOnImpact = serializedObject.FindProperty("HighlightOnImpact");
            _isGrabbable = serializedObject.FindProperty("IsGrabbable");
            _highlightWhenGrabbed = serializedObject.FindProperty("HighlightWhenGrabbed");
            _dropDistance = serializedObject.FindProperty("DropDistance");
            _gravityWhenGrabbed = serializedObject.FindProperty("GravityWhenGrabbed");
            _gravityWhenReleased = serializedObject.FindProperty("GravityWhenReleased");
            _kinematicWhenReleased = serializedObject.FindProperty("KinematicWhenReleased");
            _attachHandToItem = serializedObject.FindProperty("AttachHandToItem");
            _rotateWithHand = serializedObject.FindProperty("RotateWithHandRotation");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            serializedObject.Update();
            EditorGUILayout.PropertyField(_highlightOnImpact);
            EditorGUILayout.LabelField("Grab Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_isGrabbable);
            if (_isGrabbable.boolValue)
            {
                EditorGUILayout.PropertyField(_highlightWhenGrabbed);
                EditorGUILayout.PropertyField(_dropDistance);
                EditorGUILayout.PropertyField(_gravityWhenGrabbed);
                EditorGUILayout.PropertyField(_gravityWhenReleased);
                EditorGUILayout.PropertyField(_kinematicWhenReleased);
                EditorGUILayout.PropertyField(_attachHandToItem);
                EditorGUILayout.PropertyField(_rotateWithHand);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
