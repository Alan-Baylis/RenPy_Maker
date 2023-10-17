using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(SceneNode))]
    public class SceneEditor : NodeEditor
    {
        private bool _onError = false;
        private SceneNode _sceneNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_sceneNode == null)
            {
                _sceneNode = target as SceneNode;
            }
            
            serializedObject.Update();
            SerializedProperty property = serializedObject.FindProperty("image");
            NodeEditorGUILayout.PropertyField(property);
            Rect rect = GUILayoutUtility.GetLastRect();

            NodeEditorGUILayout.PortPair(_sceneNode.GetInputPort("entry"), _sceneNode.GetOutputPort("exit"));
            serializedObject.ApplyModifiedProperties();
            
            if (rect.Contains(Event.current.mousePosition))
            {
                GUI.Label(new Rect(4, rect.y + 18, 200, 100),
                    new GUIContent("", (Texture2D)property.objectReferenceValue));
            }
        }

        public void SetEnabledState(bool state)
        {
            _sceneNode.enabled = state;
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