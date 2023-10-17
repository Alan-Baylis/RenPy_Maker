using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(CallNode))]
    public class CallEditor : NodeEditor
    {
        private bool _onError = false;
        private CallNode _callNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_callNode == null)
            {
                _callNode = target as CallNode;
            }

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 50;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Label");
            _callNode.label = EditorGUILayout.TextField(_callNode.label);
            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("function"));
            EditorGUILayout.EndHorizontal();
            
            NodeEditorGUILayout.PortPair(_callNode.GetInputPort("entry"), _callNode.GetOutputPort("exit"));
            //NodeEditorGUILayout.PortField(_callNode.GetInputPort("entry"), GUILayout.MinWidth(0));
            serializedObject.ApplyModifiedProperties();
        }

        public void SetEnabledState(bool state)
        {
            _callNode.enabled = state;
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