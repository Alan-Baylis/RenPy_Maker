using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(StopMusicNode))]
    public class StopMusicEditor : NodeEditor
    {
        private bool _onError = false;
        private StopMusicNode _stopMusicNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_stopMusicNode == null)
            {
                _stopMusicNode = target as StopMusicNode;
            }

            serializedObject.Update();

            _stopMusicNode.fadeout = EditorGUILayout.FloatField("Fade Out", _stopMusicNode.fadeout);
            if (_stopMusicNode.fadeout < 0)
                _stopMusicNode.fadeout = 0;

            NodeEditorGUILayout.PortPair(_stopMusicNode.GetInputPort("entry"), _stopMusicNode.GetOutputPort("exit"));

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