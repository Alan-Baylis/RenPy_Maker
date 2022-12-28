using UnityEditor;
using UnityEngine;

namespace XNodeEditor
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

            SerializedProperty property = serializedObject.FindProperty("image");
            NodeEditorGUILayout.PropertyField(property);
            Rect rect = GUILayoutUtility.GetLastRect();

            float originalValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 200;   
            EditorGUILayout.PrefixLabel("Position");
            EditorGUIUtility.labelWidth = originalValue;
            _showNode.option = EditorGUILayout.Popup(_showNode.option, _showNode.positions.ToArray());

            NodeEditorGUILayout.PortPair(_showNode.GetInputPort("entry"), _showNode.GetOutputPort("exit"));

            serializedObject.ApplyModifiedProperties();

            if (rect.Contains(Event.current.mousePosition))
            {
                GUI.Label(new Rect(4, rect.y + 18, 200, 100),
                    new GUIContent("", (Texture2D)property.objectReferenceValue));
            }
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