using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateNodeMenu("Nodes/Return")]
public class ReturnNode : BaseNode
{
	[Input] public int entry;

	[HideInInspector]
	public bool errorStatus;
	private int jumpIndex;
	private bool evaluated;
	private List<string> labels = new List<string>();
	
	public override string GetNodeType()
	{
		return "ReturnNode";
	}

	public override void SetJumpIndex(int index)
	{
		jumpIndex = index;
	}

	public override int GetJumpIndex()
	{
		return jumpIndex;
	}

	public override void AddToLabelList(string newLabel)
	{
		labels.Add(newLabel);
	}

	public override List<string> GetLabelList()
	{
		return labels;
	}
	
	public override void ClearLabelList()
	{
		labels.Clear();
	}

	public override bool GetEvaluated()
	{
		return evaluated;
	}

	public override void SetEvaluated(bool flag)
	{
		evaluated = flag;
	}

	public override void SetError()
	{
		errorStatus = true;
	}

	public override string GetError()
	{
		errorStatus = false;
		
		foreach (NodePort p in Inputs)
			if (!p.IsConnected)
				errorStatus = true;

		if (errorStatus)
			return "Unconnected ports";

		NodePort port = GetOutputPort("exit");
		if (port != null)
		{
			BaseNode nextNode = port.Connection.node as BaseNode;
			if (nextNode != null && nextNode == this)
			{
				errorStatus = true;
				return "Cannot connect outputs to self";
			}
		}

		return "No Error";
	}
}