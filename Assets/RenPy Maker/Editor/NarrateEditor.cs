using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace RenPy_Maker
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
                    _narrateNode.character = characterNode.GetCharacter();
                    _narrateNode.color = characterNode.GetColor();
                    _narrateNode.image = characterNode.GetImage();
                    break;
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
            _narrateNode.SetCharacterIndex(EditorGUILayout.Popup(_narrateNode.GetCharacterIndex(), set.ToArray()));
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                if (_narrateNode.GetCharacterIndex() < set.Count)
                {
                    foreach (BaseNode n in nodes)
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
        
        public void SetEnabledState(bool state)
        {
            _narrateNode.enabled = state;
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