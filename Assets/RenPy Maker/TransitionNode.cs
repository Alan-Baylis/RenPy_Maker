using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace RenPy_Maker
{
	[CreateNodeMenu("Nodes/Transition")]
	public class TransitionNode : BaseNode
	{
		[Input] public int entry;

		public bool enabled = true;

		[HideInInspector] public int option;
		[HideInInspector] public List<string> transitions = new List<string> { "None", "Dissolve", "Fade" };
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
			return "TransitionNode";
		}

		public override string GetTransition()
		{
			string pos = "";

			switch (option)
			{
				case 0:
					pos = "None";
					break;
				case 1:
					pos = "Dissolve";
					break;
				case 2:
					pos = "Fade";
					break;
			}

			if (pos != "")
				return pos;
			else
				return null;
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

			foreach (NodePort p in Outputs)
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
}