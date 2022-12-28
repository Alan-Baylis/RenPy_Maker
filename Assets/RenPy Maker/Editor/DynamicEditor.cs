using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;
using System.Linq;

[CustomNodeEditor(typeof(DynamicNode))]
public class DynamicEditor : NodeEditor
{
    private DynamicNode _dynamicNode;
    private bool _showNodeSettings = false;
    private string _newDialogueOption = "";
    private string _newDialogueOptionOutput = "";
    private int _currentNodeTab = 0;
    private int _nodePortToDelete = 0;
    private bool _onError = false;

    public override void OnCreate()
    {
        base.OnCreate();
            
        if (_dynamicNode == null)
        {
            _dynamicNode = target as DynamicNode;
        }

        if (_dynamicNode.initialized)
            return;
        _dynamicNode.initialized = true;
            
        RenpyMaker graph = _dynamicNode.graph as RenpyMaker;
        List<string> set = graph.GetCharacterList();

        if (_dynamicNode.GetCharacterIndex() < set.Count)
        {
            foreach (BaseNode n in graph.nodes)
            {
                if (n.GetNodeType() == "CharacterNode" && set[_dynamicNode.GetCharacterIndex()] == n.GetCharacter())
                {
                    _dynamicNode.character = n.GetCharacter();
                    _dynamicNode.color = n.GetColor();
                    _dynamicNode.image = n.GetImage();
                    break;
                }
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
        if (_dynamicNode == null)
        {
            _dynamicNode = target as DynamicNode;
        }

        serializedObject.Update();

        RenpyMaker graph = _dynamicNode.graph as RenpyMaker;
        List<string> set = graph.GetCharacterList();

        EditorGUI.BeginChangeCheck();
        _dynamicNode.SetCharacterIndex(EditorGUILayout.Popup(_dynamicNode.GetCharacterIndex(), set.ToArray()));
        if (EditorGUI.EndChangeCheck())
        {
            if (_dynamicNode.GetCharacterIndex() < set.Count)
            {
                foreach (BaseNode n in _dynamicNode.graph.nodes)
                {
                    if (n.GetNodeType() == "CharacterNode" && set[_dynamicNode.GetCharacterIndex()] == n.GetCharacter())
                    {
                        _dynamicNode.character = n.GetCharacter();
                        _dynamicNode.color = n.GetColor();
                        _dynamicNode.image = n.GetImage();
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
                    _newDialogueOption = EditorGUILayout.TextField(_newDialogueOption);
                    EditorGUILayout.PrefixLabel("Output Name");
                    _newDialogueOptionOutput = EditorGUILayout.TextField(_newDialogueOptionOutput);
                    EditorGUIUtility.labelWidth = originalValue;
                    if (GUILayout.Button("Create New Button"))
                    {
                        bool noDialogue = (_newDialogueOption.Length == 0);
                        bool noOutput = (_newDialogueOptionOutput.Length == 0);
                        bool matchesExistingOutput = false;

                        foreach (NodePort p in _dynamicNode.DynamicOutputs)
                        {
                            if (p.fieldName == _newDialogueOptionOutput)
                            {
                                matchesExistingOutput = true;
                                break;
                            }
                        }

                        if (_dynamicNode.DynamicOutputs.Count() == 10)
                        {
                            EditorUtility.DisplayDialog("Error Creating Button", "Reached maximum number of buttons.", "OK");
                            return;
                        }
                        if (noDialogue)
                        {
                            EditorUtility.DisplayDialog("Error Creating Button", "No button dialogue was given.", "OK");
                            return;
                        }
                        if (noOutput)
                        {
                            EditorUtility.DisplayDialog("Error Creating Button", "No output name given.", "OK");
                            return;
                        }
                        if (matchesExistingOutput)
                        {
                            EditorUtility.DisplayDialog("Error Creating Button", "The requested button already exists.", "OK");
                            return;
                        }

                        //If we got here, it means that we can create the button
                        _dynamicNode.AddDynamicOutput(typeof(int), Node.ConnectionType.Override, Node.TypeConstraint.None, _newDialogueOptionOutput);
                        _dynamicNode.dialogueOptionList.Add(new DynamicNode.DialogueOption(_newDialogueOption, _newDialogueOptionOutput));
                    }
                    break;
                
                case 1:

                    if (_dynamicNode.DynamicOutputs.Count() == 0)
                    {
                        EditorGUILayout.HelpBox("No Buttons To Manage", MessageType.Warning);
                    }
                    else
                    {
                        EditorGUILayout.PrefixLabel("Select Button");

                        List<string> outputs = new List<string>();
                        foreach (NodePort p in _dynamicNode.DynamicOutputs)
                        {
                            outputs.Add(p.fieldName);
                        }

                        _nodePortToDelete = EditorGUILayout.Popup(_nodePortToDelete, outputs.ToArray());

                        if (GUILayout.Button("Delete Selected Button"))
                        {
                            foreach (DynamicNode.DialogueOption d in _dynamicNode.dialogueOptionList)
                            {
                                if (d.option == _dynamicNode.DynamicOutputs.ElementAt(_nodePortToDelete).fieldName)
                                {
                                    _dynamicNode.dialogueOptionList.Remove(d);
                                    break;
                                }
                            }

                            _dynamicNode.RemoveDynamicPort(_dynamicNode.DynamicOutputs.ElementAt(_nodePortToDelete));
                        }
                    }
                    break;
            }

            int number = 1;

            foreach (DynamicNode.DialogueOption d in _dynamicNode.dialogueOptionList)
            {
                EditorGUILayout.PrefixLabel("Button " + number.ToString());
                d.dialogue = EditorGUILayout.TextField(d.dialogue);
                EditorGUILayout.TextField(d.option);
                number++;
            }
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("entry"));

        foreach (NodePort p in _dynamicNode.DynamicOutputs)
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