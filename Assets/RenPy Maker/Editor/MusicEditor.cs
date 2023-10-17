using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(MusicNode))]
    public class MusicEditor : NodeEditor
    {
        private bool _onError = false;
        private MusicNode _musicNode;
        
        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_musicNode == null)
            {
                _musicNode = target as MusicNode;
            }
            
            serializedObject.Update();

            EditorGUILayout.PrefixLabel("Audio Source");
            _musicNode.source = (AudioClip)EditorGUILayout.ObjectField(_musicNode.source, typeof(AudioClip), false);
            _musicNode.fadein = EditorGUILayout.FloatField("Fade In", _musicNode.fadein);
            if (_musicNode.fadein < 0)
                _musicNode.fadein = 0;
            _musicNode.fadeout = EditorGUILayout.FloatField("Fade Out", _musicNode.fadeout);
            if (_musicNode.fadeout < 0)
                _musicNode.fadeout = 0;

            NodeEditorGUILayout.PortPair(_musicNode.GetInputPort("entry"), _musicNode.GetOutputPort("exit"));

            serializedObject.ApplyModifiedProperties();
        }
        
        public void SetEnabledState(bool state)
        {
            _musicNode.enabled = state;
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