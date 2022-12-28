using UnityEditor;
using UnityEngine;

namespace XNodeEditor
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