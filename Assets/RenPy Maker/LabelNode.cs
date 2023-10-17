using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace RenPy_Maker
{
	[CreateNodeMenu("Nodes/Label")]
	public class LabelNode : BaseNode
	{
		[Input] public int entry;

		public bool enabled = true;

		public string label;
		[HideInInspector] public bool errorStatus;
		private int jumpIndex;
		private bool evaluated;
		private List<string> labels = new List<string>();

		private int _nodeId;

		public override bool GetEnabledStatus()
		{
			return enabled;
		}

		public override void SetNodeId(int id)
		{
			_nodeId = id;
		}

		public override int GetNodeId()
		{
			return _nodeId;
		}

		private void Reset()
		{
			this.AddDynamicOutput(typeof(int), ConnectionType.Override, TypeConstraint.None, "exit");
		}

		public override string GetNodeType()
		{
			return "LabelNode";
		}

		public override string GetString()
		{
			return label;
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

			// Don't check the input port

			foreach (NodePort p in Outputs)
				if (!p.IsConnected)
					errorStatus = true;

			if (errorStatus)
				return "Unconnected ports";

			if (!RenpyMaker.CheckForAlphaNumeric(label))
			{
				errorStatus = true;
				return "Label must start with a letter and use alphanumeric characters";
			}

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

			if (string.IsNullOrEmpty(label))
			{
				errorStatus = true;
				return "No label name";
			}

			return "No Error";
		}
	}
}