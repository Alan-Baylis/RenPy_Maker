using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(PauseNode))]
    public class PauseEditor : NodeEditor
    {
        private bool _onError = false;
        private PauseNode _pauseNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_pauseNode == null)
            {
                _pauseNode = target as PauseNode;
            }

            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("seconds"));

            NodeEditorGUILayout.PortPair(_pauseNode.GetInputPort("entry"), _pauseNode.GetOutputPort("exit"));

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