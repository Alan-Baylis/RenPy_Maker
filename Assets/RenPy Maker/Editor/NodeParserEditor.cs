using UnityEditor;
using XNodeEditor;

namespace RenPy_Maker
{
    [CustomEditor(typeof(NodeParser))]
    public class NodeParserEditor : Editor
    {
        private int _choiceIndex;
        private NodeParser _nodeParser;

        public override void OnInspectorGUI()
        {
            if (_nodeParser == null)
            {
                _nodeParser = target as NodeParser;
            }

            _choiceIndex = _nodeParser.GetResolutionIndex();
            EditorGUILayout.PrefixLabel("Ren'Py Resolution");
            EditorGUI.BeginChangeCheck();
            _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _nodeParser.GetResolutions().ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                _nodeParser.SetResolutonIndex(_choiceIndex);
            }

            DrawDefaultInspector();
        }
    }
}