using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(NarrateNode))]
    public class NarrateEditor : NodeEditor
    {
        private bool _onError = false;
        private NarrateNode _narrateNode;
        
        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }
        
        public override void OnCreate()
        {
            base.OnCreate();
            
            if (_narrateNode == null)
            {
                _narrateNode = target as NarrateNode;
            }

            if (_narrateNode.initialized)
                return;
            _narrateNode.initialized = true;
            
            RenpyMaker graph = _narrateNode.graph as RenpyMaker;
            List<string> set = graph.GetCharacterList();

            if (_narrateNode.GetCharacterIndex() < set.Count)
            {
                foreach (BaseNode n in graph.nodes)
                {
                    if (n.GetNodeType() == "CharacterNode" && set[_narrateNode.GetCharacterIndex()] == n.GetCharacter())
                    {
                        _narrateNode.character = n.GetCharacter();
                        _narrateNode.color = n.GetColor();
                        _narrateNode.image = n.GetImage();
                        break;
                    }
                }
            }
        }
        
        public override void OnBodyGUI()
        {
            if (_narrateNode == null)
            {
                _narrateNode = target as NarrateNode;
            }
            
            serializedObject.Update();

            RenpyMaker graph = _narrateNode.graph as RenpyMaker;
            List<string> set = graph.GetCharacterList();

            EditorGUI.BeginChangeCheck();
            _narrateNode.SetCharacterIndex(EditorGUILayout.Popup(_narrateNode.GetCharacterIndex(), set.ToArray()));
            if (EditorGUI.EndChangeCheck())
            {
                if (_narrateNode.GetCharacterIndex() < set.Count)
                {
                    foreach (BaseNode n in _narrateNode.graph.nodes)
                    {
                        if (n.GetNodeType() == "CharacterNode" &&
                            set[_narrateNode.GetCharacterIndex()] == n.GetCharacter())
                        {
                            _narrateNode.character = n.GetCharacter();
                            _narrateNode.color = n.GetColor();
                            _narrateNode.image = n.GetImage();
                            break;
                        }
                    }
                }
            }

            //_narrateNode.character = EditorGUILayout.TextField("Character", _narrateNode.character);
            
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("color"));

            SerializedProperty property = serializedObject.FindProperty("image");
            NodeEditorGUILayout.PropertyField(property);
            Rect rect = GUILayoutUtility.GetLastRect();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("dialogue"));

            NodeEditorGUILayout.PortPair(_narrateNode.GetInputPort("entry"), _narrateNode.GetOutputPort("exit"));

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