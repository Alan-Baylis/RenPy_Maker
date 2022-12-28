using UnityEditor;
using UnityEngine;

namespace XNodeEditor
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