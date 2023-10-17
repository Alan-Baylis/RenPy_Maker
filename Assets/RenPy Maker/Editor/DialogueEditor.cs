using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
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

            GameObject renpymaker = GameObject.Find("RenPy Maker");
            NodeParser nodeParser = renpymaker.GetComponent("NodeParser") as NodeParser;
            List<BaseNode> nodes = new List<BaseNode>();
            nodes = nodeParser.GetNodeList("CharacterNode");
            List<string> set = new List<string>();
            foreach (BaseNode c in nodes)
            {
                if (c.GetNodeType() == "CharacterNode") 
                {
                    CharacterNode characterNode = c as CharacterNode;
                    _dialogueNode.character = characterNode.GetCharacter();
                    _dialogueNode.color = characterNode.GetColor();
                    _dialogueNode.image = characterNode.GetImage();
                    break;
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

            GameObject renpymaker = GameObject.Find("RenPy Maker");
            NodeParser nodeParser = renpymaker.GetComponent("NodeParser") as NodeParser;
            List<BaseNode> nodes = new List<BaseNode>();
            nodes = nodeParser.GetNodeList("CharacterNode");
            List<string> set = new List<string>();
            foreach (BaseNode c in nodes)
            {
                if (c.GetNodeType() == "CharacterNode") 
                {
                    CharacterNode characterNode = c as CharacterNode;
                    set.Add(characterNode.character);
                }
            }

            GUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 65;
            EditorGUILayout.PrefixLabel("Character");
            EditorGUI.BeginChangeCheck();
            _dialogueNode.SetCharacterIndex(EditorGUILayout.Popup(_dialogueNode.GetCharacterIndex(), set.ToArray()));//, GUILayout.Width(100)));
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                if (_dialogueNode.GetCharacterIndex() < set.Count)
                {
                    foreach (BaseNode n in nodes)
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
                //NodeEditorWindow.current.MoveNodeToTop(_dialogNode);
                GUI.Label(new Rect(4, rect.y + 18, 200, 100),
                    new GUIContent("", (Texture2D)property.objectReferenceValue));
            }
        }
        
        public void SetEnabledState(bool state)
        {
            _dialogueNode.enabled = state;
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