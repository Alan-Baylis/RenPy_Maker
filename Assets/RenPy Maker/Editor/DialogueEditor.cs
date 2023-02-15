using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XNodeEditor
{
    [CustomNodeEditor(typeof(DialogueNode))]
    public class DialogueEditor : NodeEditor
    {
        private bool _onError = false;
        private DialogueNode _dialogueNode;

        public override void OnCreate()
        {
            base.OnCreate();
            
            if (_dialogueNode == null)
            {
                _dialogueNode = target as DialogueNode;
            }

            if (_dialogueNode.initialized)
                return;
            _dialogueNode.initialized = true;
            
            RenpyMaker graph = _dialogueNode.graph as RenpyMaker;
            List<string> set = graph.GetCharacterList();

            if (_dialogueNode.GetCharacterIndex() < set.Count)
            {
                foreach (BaseNode n in graph.nodes)
                {
                    if (n.GetNodeType() == "CharacterNode" &&
                        set[_dialogueNode.GetCharacterIndex()] == n.GetCharacter())
                    {
                        _dialogueNode.character = n.GetCharacter();
                        _dialogueNode.color = n.GetColor();
                        _dialogueNode.image = n.GetImage();
                        break;
                    }
                }
            }
        }

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_dialogueNode == null)
            {
                _dialogueNode = target as DialogueNode;
            }
            
            serializedObject.Update();

            RenpyMaker graph = _dialogueNode.graph as RenpyMaker;
            List<string> set = graph.GetCharacterList();

            EditorGUI.BeginChangeCheck();
            _dialogueNode.SetCharacterIndex(EditorGUILayout.Popup(_dialogueNode.GetCharacterIndex(), set.ToArray()));
            if (EditorGUI.EndChangeCheck())
            {
                if (_dialogueNode.GetCharacterIndex() < set.Count)
                {
                    foreach (BaseNode n in _dialogueNode.graph.nodes)
                    {
                        if (n.GetNodeType() == "CharacterNode" &&
                            set[_dialogueNode.GetCharacterIndex()] == n.GetCharacter())
                        {
                            _dialogueNode.character = n.GetCharacter();
                            _dialogueNode.color = n.GetColor();
                            _dialogueNode.image = n.GetImage();
                            break;
                        }
                    }
                }
            }
            
            //_dialogueNode.character = EditorGUILayout.TextField("Character", _dialogueNode.character);
            
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("color"));

            SerializedProperty property = serializedObject.FindProperty("image");
            NodeEditorGUILayout.PropertyField(property);
            Rect rect = GUILayoutUtility.GetLastRect();
            
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("dialogue"));

            GUILayout.BeginHorizontal();
            NodeEditorGUILayout.PortField(_dialogueNode.GetInputPort("entry"), GUILayout.MinWidth(0));
            GUILayout.FlexibleSpace();
            NodeEditorGUILayout.PortField(_dialogueNode.GetOutputPort("exit"), GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
            
            if (rect.Contains(Event.current.mousePosition))
            {
                //NodeEditorWindow.current.MoveNodeToTop(_dialogueNode);
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