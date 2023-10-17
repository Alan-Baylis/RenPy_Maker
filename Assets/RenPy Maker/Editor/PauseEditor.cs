using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
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

        public void SetEnabledState(bool state)
        {
            _pauseNode.enabled = state;
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