using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
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

        public void SetEnabledState(bool state)
        {
            _transitionNode.enabled = state;
        }

        public override void AddContextMenuItems(GenericMenu menu)
        {
            SerializedProperty enabledProp = serializedObject.FindProperty("enabled");
            bool enabled = enabledProp.boolValue;

            if (enabled)
                menu.AddItem(new GUIContent("Disable"), false, () => SetEnabledState(false));
            else
                menu.AddItem(new GUIContent("Enable"), false, () => SetEnabledState(true));

            base.AddContextMenuItems(menu);
        }

        public override Color GetTint()
        {
            SerializedProperty enabledProp = serializedObject.FindProperty("enabled");
            bool enabled = enabledProp.boolValue;

            if (enabled)
            {
                SerializedProperty errorProp = serializedObject.FindProperty("errorStatus");
                _onError = errorProp.boolValue;

                if (_onError)
                    return new Color(0.5f, 0, 0);
                else
                    return NodeEditorPreferences.GetSettings().tintColor;
            }
            else
            {
                return new Color(0.1f, 0.1f, 0.1f);
            }
        }
    }
}