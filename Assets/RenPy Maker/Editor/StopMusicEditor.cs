using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
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

        public void SetEnabledState(bool state)
        {
            _stopMusicNode.enabled = state;
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