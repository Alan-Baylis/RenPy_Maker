using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;
using System.Linq;

namespace RenPy_Maker
{
    [CustomNodeEditor(typeof(MenuNode))]
    public class MenuEditor : NodeEditor
    {
        private MenuNode _menuNode;
        private bool _showNodeSettings = false;
        private string _newMenuOption = "";
        private string _newMenuOptionOutput = "";
        private int _currentNodeTab = 0;
        private int _nodePortToDelete = 0;
        private bool _onError = false;

        public override void OnCreate()
        {
            base.OnCreate();

            if (_menuNode == null)
            {
                _menuNode = target as MenuNode;
            }

            if (_menuNode.initialized)
                return;

            _menuNode.initialized = true;

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
                    _menuNode.character = characterNode.GetCharacter();
                    _menuNode.color = characterNode.GetColor();
                    _menuNode.image = characterNode.GetImage();
                    break;
                }
            }
        }

        public override void OnHeaderGUI()
        {
/*
        Event e = Event.current;
        switch (Event.current.type)
        {
            case EventType.MouseDown:
                Debug.Log("here");
                break;
        }
*/
            GUILayout.Label(target.name, NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
        }

        public override void OnBodyGUI()
        {
            if (_menuNode == null)
            {
                _menuNode = target as MenuNode;
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
            _menuNode.SetCharacterIndex(EditorGUILayout.Popup(_menuNode.GetCharacterIndex(), set.ToArray()));
            GUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                if (_menuNode.GetCharacterIndex() < set.Count)
                {
                    foreach (BaseNode n in nodes)
                    {
                        if (n.GetNodeType() == "CharacterNode" &&
                            set[_menuNode.GetCharacterIndex()] == n.GetCharacter())
                        {
                            _menuNode.character = n.GetCharacter();
                            _menuNode.color = n.GetColor();
                            _menuNode.image = n.GetImage();
                            break;
                        }
                    }
                }
            }

            //_dynamicNode.character = EditorGUILayout.TextField("Character", _dynamicNode.character);

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("color"));

            SerializedProperty property = serializedObject.FindProperty("image");
            NodeEditorGUILayout.PropertyField(property);
            Rect rect = GUILayoutUtility.GetLastRect();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("dialogue"));

            _showNodeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_showNodeSettings, "Button Settings");

            if (_showNodeSettings)
            {
                _currentNodeTab = GUILayout.Toolbar(_currentNodeTab, new string[] { "Add Button", "Delete Button" });
                switch (_currentNodeTab)
                {
                    case 0:
                        float originalValue = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 200;
                        EditorGUILayout.PrefixLabel("Button Text");
                        _newMenuOption = EditorGUILayout.TextField(_newMenuOption);
                        EditorGUILayout.PrefixLabel("Output Name");
                        _newMenuOptionOutput = EditorGUILayout.TextField(_newMenuOptionOutput);
                        EditorGUIUtility.labelWidth = originalValue;
                        if (GUILayout.Button("Create New Button"))
                        {
                            bool noDialogue = (_newMenuOption.Length == 0);
                            bool noOutput = (_newMenuOptionOutput.Length == 0);
                            bool matchesExistingOutput = false;

                            foreach (NodePort p in _menuNode.DynamicOutputs)
                            {
                                if (p.fieldName == _newMenuOptionOutput)
                                {
                                    matchesExistingOutput = true;
                                    break;
                                }
                            }

                            if (_menuNode.DynamicOutputs.Count() == 10)
                            {
                                EditorUtility.DisplayDialog("Error Creating Button",
                                    "Reached maximum number of buttons.", "OK");
                                return;
                            }

                            if (noDialogue)
                            {
                                EditorUtility.DisplayDialog("Error Creating Button", "No button dialogue was given.",
                                    "OK");
                                return;
                            }

                            if (noOutput)
                            {
                                EditorUtility.DisplayDialog("Error Creating Button", "No output name given.", "OK");
                                return;
                            }

                            if (matchesExistingOutput)
                            {
                                EditorUtility.DisplayDialog("Error Creating Button",
                                    "The requested button already exists.", "OK");
                                return;
                            }

                            //If we got here, it means that we can create the button
                            _menuNode.AddDynamicOutput(typeof(int), Node.ConnectionType.Override,
                                Node.TypeConstraint.None, _newMenuOptionOutput);
                            _menuNode.menuOptionList.Add(new MenuNode.MenuOption(_newMenuOption, _newMenuOptionOutput));
                        }

                        break;

                    case 1:

                        if (_menuNode.DynamicOutputs.Count() == 0)
                        {
                            EditorGUILayout.HelpBox("No Buttons To Manage", MessageType.Warning);
                        }
                        else
                        {
                            EditorGUILayout.PrefixLabel("Select Button");

                            List<string> outputs = new List<string>();
                            foreach (NodePort p in _menuNode.DynamicOutputs)
                            {
                                outputs.Add(p.fieldName);
                            }

                            _nodePortToDelete = EditorGUILayout.Popup(_nodePortToDelete, outputs.ToArray());

                            if (GUILayout.Button("Delete Selected Button"))
                            {
                                foreach (MenuNode.MenuOption d in _menuNode.menuOptionList)
                                {
                                    if (d.option == _menuNode.DynamicOutputs.ElementAt(_nodePortToDelete).fieldName)
                                    {
                                        _menuNode.menuOptionList.Remove(d);
                                        break;
                                    }
                                }

                                _menuNode.RemoveDynamicPort(_menuNode.DynamicOutputs.ElementAt(_nodePortToDelete));
                            }
                        }

                        break;
                }

                int number = 1;

                foreach (MenuNode.MenuOption d in _menuNode.menuOptionList)
                {
                    EditorGUILayout.PrefixLabel("Button " + number.ToString());
                    d.dialogue = EditorGUILayout.TextField(d.dialogue);
                    EditorGUILayout.TextField(d.option);
                    number++;
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("entry"));

            foreach (NodePort p in _menuNode.DynamicOutputs)
            {
                NodeEditorGUILayout.PortField(p);
            }

            serializedObject.ApplyModifiedProperties();

            if (rect.Contains(Event.current.mousePosition))
            {
                //NodeEditorWindow.current.MoveNodeToTop(_dynamicNode);
                GUI.Label(new Rect(4, rect.y + 18, 200, 100),
                    new GUIContent("", (Texture2D)property.objectReferenceValue));
            }
        }

/*    
    public void SetEnabledState(bool state)
    {
        _menuNode.enabled = state;
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
*/
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