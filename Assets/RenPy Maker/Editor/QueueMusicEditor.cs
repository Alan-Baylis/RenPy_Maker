using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(QueueMusicNode))]
    public class QueueMusicEditor : NodeEditor
    {
        private bool _onError = false;
        private QueueMusicNode _queueMusicNode;
        
        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_queueMusicNode == null)
            {
                _queueMusicNode = target as QueueMusicNode;
            }
            
            serializedObject.Update();

            EditorGUILayout.PrefixLabel("Audio Source");
            _queueMusicNode.source = (AudioClip)EditorGUILayout.ObjectField(_queueMusicNode.source, typeof(AudioClip), false);

            NodeEditorGUILayout.PortPair(_queueMusicNode.GetInputPort("entry"), _queueMusicNode.GetOutputPort("exit"));

            serializedObject.ApplyModifiedProperties();
        }
        
        public void SetEnabledState(bool state)
        {
            _queueMusicNode.enabled = state;
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