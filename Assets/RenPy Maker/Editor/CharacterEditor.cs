using UnityEngine;
using UnityEditor;

namespace XNodeEditor
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

            GUIStyle style = new GUIStyle();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            _characterNode.character = EditorGUILayout.TextField("", _characterNode.character, GUILayout.Width(90));
            GUILayout.EndHorizontal();
            
            if (editingTextFlag && !EditorGUIUtility.editingTextField)
            {
                editingTextFlag = false;
                
                // Update the character name of all relevant nodes 
                if (_characterNode.previousName != _characterNode.character)
                {
                    foreach (BaseNode node in _characterNode.graph.nodes)
                    {
                        if (node.GetNodeType() == "DialogueNode" || node.GetNodeType() == "DynamicNode" || node.GetNodeType() == "NarrateNode")
                        {
                            if (node.GetCharacter() == _characterNode.previousName)
                            {
                                node.SetCharacter(_characterNode.character);
                                // Reset their character index variable
                            }
                        }
                    }
                }    

                RenpyMaker tempGraph = _characterNode.graph as RenpyMaker;
                
                tempGraph.ClearCharacterList();

                foreach (BaseNode n in _characterNode.graph.nodes)
                {
                    if (n.GetNodeType() == "CharacterNode")
                    {
                        tempGraph.GetCharacterList().Add(n.GetCharacter()); 
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