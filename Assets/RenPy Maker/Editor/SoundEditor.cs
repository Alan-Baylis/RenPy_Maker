using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(SoundNode))]
    public class SoundEditor : NodeEditor
    {
        private bool _onError = false;
        private SoundNode _soundNode;
        
        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_soundNode == null)
            {
                _soundNode = target as SoundNode;
            }
            
            serializedObject.Update();

            EditorGUILayout.PrefixLabel("Audio Source");
            _soundNode.source = (AudioClip)EditorGUILayout.ObjectField(_soundNode.source, typeof(AudioClip), false);

            NodeEditorGUILayout.PortPair(_soundNode.GetInputPort("entry"), _soundNode.GetOutputPort("exit"));

            serializedObject.ApplyModifiedProperties();
        }
        
        public void SetEnabledState(bool state)
        {
            _soundNode.enabled = state;
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