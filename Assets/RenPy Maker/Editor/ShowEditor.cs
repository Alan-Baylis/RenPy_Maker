using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(ShowNode))]
    public class ShowEditor : NodeEditor
    {
        private bool _onError = false;
        private ShowNode _showNode;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_showNode == null)
            {
                _showNode = target as ShowNode;
            }

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 60;
            SerializedProperty property = serializedObject.FindProperty("image");
            
            NodeEditorGUILayout.PropertyField(property);
            Rect rect = GUILayoutUtility.GetLastRect();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Position");
            _showNode.option = EditorGUILayout.Popup(_showNode.option, _showNode.positions.ToArray());
            EditorGUILayout.EndHorizontal();
            
            NodeEditorGUILayout.PortPair(_showNode.GetInputPort("entry"), _showNode.GetOutputPort("exit"));

            serializedObject.ApplyModifiedProperties();

            if (rect.Contains(Event.current.mousePosition))
            {
                GUI.Label(new Rect(4, rect.y + 18, 200, 100),
                    new GUIContent("", (Texture2D)property.objectReferenceValue));
            }
        }
        
        public void SetEnabledState(bool state)
        {
            _showNode.enabled = state;
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