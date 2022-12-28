using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(TransitionNode))]
    public class TransitionEditor : NodeEditor
    {
        private bool _onError = false;
        private TransitionNode _transitionNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_transitionNode == null)
            {
                _transitionNode = target as TransitionNode;
            }

            serializedObject.Update();
            
            float originalValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;   
            EditorGUILayout.PrefixLabel("Select Transition");
            EditorGUIUtility.labelWidth = originalValue;
            _transitionNode.option = EditorGUILayout.Popup(_transitionNode.option, _transitionNode.transitions.ToArray());

            NodeEditorGUILayout.PortPair(_transitionNode.GetInputPort("entry"), _transitionNode.GetOutputPort("exit"));

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