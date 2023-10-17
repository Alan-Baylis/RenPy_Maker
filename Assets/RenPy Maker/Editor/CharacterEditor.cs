using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(CharacterNode))]
    public class CharacterEditor : NodeEditor
    {
        private bool _onError = false;
        private CharacterNode _characterNode;
        private bool editingTextFlag = false;

        public override void OnHeaderGUI()
        {
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_characterNode == null)
            {
                _characterNode = target as CharacterNode;
            }

            serializedObject.Update();

            SerializedProperty property = serializedObject.FindProperty("image");

            EditorGUI.LabelField(new Rect(5, 5, 100, 100), new GUIContent("", (Texture2D)property.objectReferenceValue));
            
            if (EditorGUIUtility.editingTextField && editingTextFlag == false)
            {
                // Make a copy of the original names
                _characterNode.previousName = _characterNode.character;
                
                editingTextFlag = true;
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _characterNode.character = EditorGUILayout.TextField("", _characterNode.character, GUILayout.Width(90));
            GUILayout.EndHorizontal();
            
            if (editingTextFlag && !EditorGUIUtility.editingTextField)
            {
                editingTextFlag = false;

                // Collect all Character nodes
                GameObject renpymaker = GameObject.Find("RenPy Maker");
                NodeParser nodeParser = renpymaker.GetComponent("NodeParser") as NodeParser;
                List<BaseNode> nodes = new List<BaseNode>();
                nodes = nodeParser.GetNodeList("DialogueNode");
                nodes.AddRange(nodeParser.GetNodeList("DynamicNode"));
                nodes.AddRange(nodeParser.GetNodeList("NarrateNode"));

                // Update the character name of all relevant nodes
                if (_characterNode.previousName != _characterNode.character)
                {
                    foreach (BaseNode node in nodes)
                    {
                        if (node.GetCharacter() == _characterNode.previousName)
                        {
                            node.SetCharacter(_characterNode.character);
// Todo: Reset their character index variable
                        }
                    }
                }    
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 35f;
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("color"), GUIContent.none);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUIUtility.labelWidth = 35f;
            NodeEditorGUILayout.PropertyField(property, GUIContent.none);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
        
        public void SetEnabledState(bool state)
        {
            _characterNode.enabled = state;
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