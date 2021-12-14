using UnityEditor;
using UnityEngine;

namespace ExpressoBits.Interactions.Editor
{
    [CustomEditor(typeof(RayCastBasedInteractionsSelector))]
    public class RayCastBasedInteractionsSelectorEditor : UnityEditor.Editor
    {

        private SerializedProperty maxDistanceToSelectProperty;
        private SerializedProperty additionalDistanceToSelectorProperty;
        private SerializedProperty debugLineProperty;
        private SerializedProperty isCustomTransformRayOriginProperty;
        private SerializedProperty cameraTransformProperty;
        private SerializedProperty distanceProperty;

        private void OnEnable()
        {
            maxDistanceToSelectProperty = serializedObject.FindProperty("maxDistanceToSelect");
            additionalDistanceToSelectorProperty = serializedObject.FindProperty("additionalDistanceToSelector");
            debugLineProperty = serializedObject.FindProperty("debugLine");
            isCustomTransformRayOriginProperty = serializedObject.FindProperty("isCustomTransformRayOrigin");
            cameraTransformProperty = serializedObject.FindProperty("cameraTransform");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(maxDistanceToSelectProperty);
            EditorGUILayout.PropertyField(additionalDistanceToSelectorProperty);
            EditorGUILayout.PropertyField(debugLineProperty);
            EditorGUILayout.PropertyField(isCustomTransformRayOriginProperty);
            if(isCustomTransformRayOriginProperty.boolValue)
            {
                EditorGUILayout.PropertyField(cameraTransformProperty);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}

