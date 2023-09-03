using UnityEditor;
using Com.TheFallenGames.OSA.AdditionalComponents;

namespace Com.TheFallenGames.OSA.Editor.Util
{
	[CustomEditor(typeof(OSAProactiveNavigator), true)]
	public class OSAProactiveNavigatorCustomEditor : UnityEditor.Editor
    {
        SerializedProperty _Selectables;
        SerializedProperty _OnNoSelectableSpecified;
        SerializedProperty _JoystickInputMultiplier;
        SerializedProperty _ArrowsInputMultiplier;
        SerializedProperty _LoopAtExtremity;


        void OnEnable()
        {
            _Selectables = serializedObject.FindProperty("_Selectables");
            _OnNoSelectableSpecified = serializedObject.FindProperty("_OnNoSelectableSpecified");
            _JoystickInputMultiplier = serializedObject.FindProperty("_JoystickInputMultiplier");
            _ArrowsInputMultiplier = serializedObject.FindProperty("_ArrowsInputMultiplier");
            _LoopAtExtremity = serializedObject.FindProperty("_LoopAtExtremity");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_Selectables, true);
            EditorGUILayout.PropertyField(_OnNoSelectableSpecified);
            EditorGUILayout.PropertyField(_JoystickInputMultiplier);
            EditorGUILayout.PropertyField(_ArrowsInputMultiplier);
            EditorGUILayout.PropertyField(_LoopAtExtremity);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
