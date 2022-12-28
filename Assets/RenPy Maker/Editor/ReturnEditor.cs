using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(ReturnNode))]
    public class ReturnEditor : NodeEditor
    {
        private bool _onError = false;
        private ReturnNode _returnNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_returnNode == null)
            {
                _returnNode = target as ReturnNode;
            }

            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("entry"));

            serializedObject.ApplyModifiedProperties();
        }

        public override Color GetTint()
        {
            SerializedProperty errorProp = serializedObject.FindProperty("errorStatus");
            _onError = errorProp.boolValue;

            if (_onError)
                return new Color(0.5f, 0, 0);
            else
                return NodeEditorPreferences.GetSettings().tintColor;
        }
    }
}