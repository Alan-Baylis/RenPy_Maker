using System.Collections.Generic;
using UnityEngine;
using XNode;

public abstract class BaseNode : Node
{
	public virtual float GetFadein()
	{
		return 1.0f;
	}

	public virtual float GetFadeout()
	{
		return 1.0f;
	}

	public virtual AudioClip GetAudioSource()
	{
		return null;
	}
	
	public virtual string GetTransition()
	{
		return null;
	}

	public virtual string GetPosition()
	{
		return null;
	}

	public virtual string GetNodeType()
	{
		return null;
	}
	
	public virtual string GetCharacter()
	{
		return null;
	}

	public virtual void SetCharacter(string characterName)
	{
	}

	public virtual string GetString()
	{
		return null;
	}

	public virtual List<DynamicNode.DialogueOption> GetDynamicOptions()
	{
		return null;
	}

	public virtual Texture2D GetImage()
	{
		return null;
	}
	
	public virtual string GetDialogue()
	{
		return null;
	}

	public virtual Color GetColor()
	{
		return Color.white;
	}

	public virtual int GetSeconds()
	{
		return 1;
	}

	public virtual Sprite GetSprite()
	{
		return null;
	}

	public virtual void SetJumpIndex(int newIndex)
	{
	}

	public virtual int GetJumpIndex()
	{
		return 1;
	}

	public virtual void AddToLabelList(string label)
	{
	}

	public virtual List<string> GetLabelList()
	{
		return null;
	}
	
	public virtual void ClearLabelList()
	{
	}

	public virtual bool GetEvaluated()
	{
		return false;
	}

	public virtual void SetEvaluated(bool flag)
	{
	}
	
	public virtual string GetError()
	{
		return null;
	}

	public virtual void SetError()
	{
	}

	public override object GetValue(NodePort port)
	{
		return null;
	}
}