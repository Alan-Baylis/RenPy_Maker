using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(LabelNode))]
    public class LabelEditor : NodeEditor
    {
        private bool _onError = false;
        private LabelNode _labelNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_labelNode == null)
            {
                _labelNode = target as LabelNode;
            }

            serializedObject.Update();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("label"));

            NodeEditorGUILayout.PortPair(_labelNode.GetInputPort("entry"), _labelNode.GetOutputPort("exit"));

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