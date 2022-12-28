using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(StartNode))]
    public class StartEditor : NodeEditor
    {
        private bool _onError = false;
        private StartNode _startNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_startNode == null)
            {
                _startNode = target as StartNode;
            }

            serializedObject.Update();

            NodeEditorGUILayout.PortField(_startNode.GetOutputPort("exit"));

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