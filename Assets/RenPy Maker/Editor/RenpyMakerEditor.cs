using XNodeEditor;

namespace RenPy_Maker
{
    [CustomNodeGraphEditor(typeof(RenpyMaker), "RenpyMaker.Settings")]
    public class RenpyMakerEditor : NodeGraphEditor
    {
        public override string GetPortTooltip(XNode.NodePort port)
        {
            return null;
        }

        public override void OnDropObjects(UnityEngine.Object[] objects)
        {

        }
    }
}