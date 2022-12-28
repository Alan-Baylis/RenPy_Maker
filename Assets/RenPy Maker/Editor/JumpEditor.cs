using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(JumpNode))]
    public class JumpEditor : NodeEditor
    {
        private bool _onError = false;
        private JumpNode _jumpNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_jumpNode == null)
            {
                _jumpNode = target as JumpNode;
            }

            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("label"));

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