using XNodeEditor;

[CustomNodeGraphEditor(typeof(RenpyMaker), "RenpyMaker.Settings")]
public class RenpyMakerEditor : NodeGraphEditor
{
    public override string GetPortTooltip(XNode.NodePort port)
    {
        return null;
    }
}